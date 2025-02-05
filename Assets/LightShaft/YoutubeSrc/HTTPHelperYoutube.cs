﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace YoutubeLight
{
    internal static class HTTPHelperYoutube
    {
        public static string HtmlDecode(string value)
        {
            return DecodeHtmlChars(value);
        }

        public static string DecodeHtmlChars(this string source)
        {
            string[] parts = source.Split(new string[] { "&#x" }, StringSplitOptions.None);
            for (int i = 1; i < parts.Length; i++)
            {
                int n = parts[i].IndexOf(';');
                string number = parts[i].Substring(0, n);
                try
                {
                    int unicode = Convert.ToInt32(number, 16);
                    parts[i] = ((char)unicode) + parts[i].Substring(n + 1);
                }
                catch { }
            }
            return String.Join("", parts);
        }

        public static IDictionary<string, string> ParseQueryString(string s)
        {
            // remove anything other than query string from url
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
            }
            //Debug.Log("ADDAAP "+ s);

            var dictionary = new Dictionary<string, string>();

            foreach (string vp in Regex.Split(s, "&"))
            {
                string[] strings = Regex.Split(vp, "=");
                dictionary.Add(strings[0], strings.Length == 2 ? UrlDecode(strings[1]) : string.Empty);
            }

            return dictionary;
        }

        public static string ReplaceQueryStringParameter(string currentPageUrl, string paramToReplace, string newValue)
        {
            //Debug.Log(currentPageUrl);
            var query = ParseQueryString(currentPageUrl);

            query[paramToReplace] = newValue;

            var resultQuery = new StringBuilder();
            bool isFirst = true;

            foreach (KeyValuePair<string, string> pair in query)
            {
                if (!isFirst)
                {
                    resultQuery.Append("&");
                }

                resultQuery.Append(pair.Key);
                resultQuery.Append("=");
                resultQuery.Append(pair.Value);
                //Debug.Log("r: " + resultQuery.ToString());

                isFirst = false;
            }

            //Debug.Log(resultQuery.ToString());
            //Debug.Log(currentPageUrl);

            var uriBuilder = new UriBuilder(currentPageUrl)
            {
                Query = resultQuery.ToString()
            };

            //Debug.Log(resultQuery.ToString());
            //Debug.Log(uriBuilder.ToString());

            return uriBuilder.ToString();
        }

        public static string UrlDecode(string url)
        {

            return WWW.UnEscapeURL(url);
        }
    }
}
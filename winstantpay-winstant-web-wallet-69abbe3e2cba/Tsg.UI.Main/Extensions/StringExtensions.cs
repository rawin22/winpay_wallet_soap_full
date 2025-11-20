using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ServiceResponseData = Tsg.Business.Model.TsgGPWebService.ServiceResponseData;

namespace Tsg.UI.Main.Extensions
{
    public static class StringExtensions
    {
        public static String AppendAll(this IEnumerable<String> collection, String seperator)
        {
            using (var enumerator = collection.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return String.Empty;
                }

                var builder = new StringBuilder().Append(enumerator.Current);

                while (enumerator.MoveNext())
                {
                    builder.Append(seperator).Append(enumerator.Current);
                }

                return builder.ToString();
            }
        }
        public static string TruncateLongString(this string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static string StripHTML(this string input)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            input = Regex.Replace(input, "<.*?>", String.Empty);
            input = regex.Replace(input, " ");
            return System.Net.WebUtility.HtmlDecode(input);
        }
        public static string ConvertServiceResponseToSingleString(ServiceResponseData[] response)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var serviceResponseData in response)
            {
                sb.AppendLine($"[{serviceResponseData.ResponseType.ToString()}] : {serviceResponseData.Message}");
                sb.AppendLine($"[Details] : {serviceResponseData.MessageDetails}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
        //public static string StripHTML(this string html)
        //{
        //    const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
        //    const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
        //    const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
        //    var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
        //    var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
        //    var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

        //    var text = html;
        //    //Decode html specific characters
        //    text = System.Net.WebUtility.HtmlDecode(text);
        //    //Remove tag whitespace/line breaks
        //    text = tagWhiteSpaceRegex.Replace(text, "><");
        //    //Replace <br /> with line breaks
        //    text = lineBreakRegex.Replace(text, Environment.NewLine);
        //    //Strip formatting
        //    text = stripFormattingRegex.Replace(text, string.Empty);
        //    return text;
        //}
        public static string Decrypt(this string text, string publicTokenKey, string privateTokenKey)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            byte[] publicKey = Encoding.Unicode.GetBytes(publicTokenKey).Take(8).ToArray();
            byte[] privateKey = Encoding.Unicode.GetBytes(privateTokenKey).Take(8).ToArray();
            ICryptoTransform transform = algorithm.CreateDecryptor(publicKey, privateKey);
            byte[] inputbuffer = Convert.FromBase64String(text.Replace(" ", "+"));
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }

    }
}
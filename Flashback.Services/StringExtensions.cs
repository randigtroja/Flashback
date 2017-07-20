using System;
using System.Net;
using System.Text;

namespace Flashback.Services
{
    public static class StringExtensions
    {        
        public static string FixaRadbrytningar(this String str)
        {
            return string.IsNullOrWhiteSpace(str) ? str : str.Replace("&nbsp;", "").Replace("\t", "").Replace("\n", "");
        }

        public static string GetCleanId(this String str, bool removeForumAndThreadIndicator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            else
            {
                str = str.GetCleanIdFirstPage().Replace("p1", "");

                if (removeForumAndThreadIndicator)
                    str = str.Replace("f", "").Replace("t", "");

                return str;
            }
        }

        public static string GetCleanIdForPage(this String str, int pageNumber)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            else
            {
                str = str.GetCleanId(false) + "p" + pageNumber;

                return str;                
            }
        }

        public static string GetCleanIdFirstPage(this String str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            else
            {                                
                str = str.Replace("s", "");
                str = str + "p1";

                int posP = str.IndexOf("p", System.StringComparison.Ordinal);
                if (posP != -1)
                {
                    str = str.Substring(0, posP) + "p1";
                }

                return str;
            }
        }

        public static string GetCleanIdLastPage(this String str, int maxPages)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            else
            {                
                str = str.Replace("s", "");
                str = str + ("p" + maxPages);
                int posP = str.IndexOf("p", System.StringComparison.Ordinal);
                if (posP != -1)
                {
                    str = str.Substring(0, posP) + "p" + maxPages;
                }

                return str;
            }
        }

        public static string GetCleanIdPreviousPage(this String str, int currentPage)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            else
            {
                str = str.Replace("s", "");                
                str = str + "p" + (currentPage-1);

                int posP = str.IndexOf("p", System.StringComparison.Ordinal);
                if (posP != -1)
                {
                    str = str.Substring(0, posP) + "p" + (currentPage-1);
                }

                return str;
            }
        }

        public static string GetCleanIdNextPage(this String str, int currentPage)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            else
            {
                str = str.Replace("s", "");
                str = str + "p" + (currentPage + 1);

                int posP = str.IndexOf("p", System.StringComparison.Ordinal);
                if (posP != -1)
                {
                    str = str.Substring(0, posP) + "p" + (currentPage + 1);
                }

                return str;
            }
        }
        
        public static string FormatToEncodedPostable(this String str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var output = WebUtility.HtmlEncode(str).Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<").Replace("&quot;", "\"");

            var destinationEncoding = Encoding.GetEncoding("ISO-8859-1");            
            var sourceEncoding = Encoding.UTF8;
            
            var sourceBytes = sourceEncoding.GetBytes(output);
            var destinationBytes = Encoding.Convert(sourceEncoding, destinationEncoding, sourceBytes);            

            var encodesBytes = WebUtility.UrlEncodeToBytes(destinationBytes, 0, destinationBytes.Length);
            var encodedString = destinationEncoding.GetString(encodesBytes, 0, encodesBytes.Length);

            return encodedString;            
        }
    }
}

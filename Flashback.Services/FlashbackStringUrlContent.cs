using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Flashback.Services
{
    public class FlashbackStringUrlContent: StringContent
    {
        public FlashbackStringUrlContent(string content) : base(content)
        {
        }

        public FlashbackStringUrlContent(string content, Encoding encoding) : base(content, encoding)
        {
        }

        public FlashbackStringUrlContent(string content, Encoding encoding, string mediaType) : base(content, encoding, mediaType)
        {
        }

        public FlashbackStringUrlContent(IEnumerable<KeyValuePair<string, string>> formData) : base(GetPostData(formData))
        {
            base.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        private static string GetPostData(IEnumerable<KeyValuePair<string, string>> formData)
        {
            if (formData == null)
            {
                throw new ArgumentNullException("formData");
            }

            var stringBuilder = new StringBuilder();

            foreach (var data in formData)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append('&');
                }

                stringBuilder.Append(data.Key.FormatToEncodedPostable());
                stringBuilder.Append('=');
                stringBuilder.Append(data.Value.FormatToEncodedPostable());
            }

            return stringBuilder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Flashback.Services.Auth
{
    public class AuthService
    {
        private readonly FlashbackHttpClient _httpClient;

        public AuthService(CookieContainer container)
        {
            _httpClient = new FlashbackHttpClient(container, false);
        }

        public string BuildMd5HashForLogin(string password)
        {
            var iso = Encoding.GetEncoding("ISO-8859-1");
            var utf8 = Encoding.UTF8;

            byte[] utfBytesPass = utf8.GetBytes(password);
            byte[] isoBytesPass = Encoding.Convert(utf8, iso, utfBytesPass);

            var md5 = MD5.Create();
            var hash =  md5.ComputeHash(isoBytesPass);

            var result = new StringBuilder(hash.Length * 2);

            foreach (byte t in hash)
                result.Append(t.ToString("x2"));

            return result.ToString();
        }

        public async Task Logout(string sessionhash)
        {           
            var response = await _httpClient.GetAsync("https://www.flashback.org/login.php?do=logout&logouthash=" + sessionhash);
        }

        /// <summary>
        /// Loggar in
        /// </summary>
        /// <param name="username">Användarnamn</param>
        /// <param name="password">Lösenord</param>        
        /// <returns></returns>
        public async Task<bool> TryLogin(string username, string password)
        {            
            var postData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("vb_login_username", username),
                    new KeyValuePair<string, string>("vb_login_password", ""),
                    new KeyValuePair<string, string>("do", "login"),
                    new KeyValuePair<string, string>("cookieuser", "1"),
                    new KeyValuePair<string, string>("vb_login_md5password", BuildMd5HashForLogin(password)),
                    new KeyValuePair<string, string>("vb_login_md5password_utf", BuildMd5HashForLogin(password))
                };

            var postContent = new FlashbackStringUrlContent(postData);

            var response = await _httpClient.PostAsync("https://www.flashback.org/login.php", postContent);

            var pageUri = response.RequestMessage.RequestUri;

            var cookieContainer = new CookieContainer();
            IEnumerable<string> cookies;
            if (response.Headers.TryGetValues("set-cookie", out cookies))
            {
                foreach (var c in cookies)
                {
                    cookieContainer.SetCookies(pageUri, c);
                }
            }
            
            var loginCheck = cookieContainer.GetCookies(new Uri("https://flashback.org/"))
                .Cast<Cookie>()
                .FirstOrDefault(x => x.Name == "vbscanuserid");

            return loginCheck != null;            
        }
    }
}

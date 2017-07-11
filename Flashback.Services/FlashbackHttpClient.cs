using System.Net;
using System.Net.Http;

namespace Flashback.Services
{
    public class FlashbackHttpClient : HttpClient
    {
        public FlashbackHttpClient(CookieContainer container, bool allowRedirect = true): base(new HttpClientHandler()
        {
            CookieContainer = container,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = allowRedirect,
            UseCookies = true
        })
        {            
              
        }
    }
}
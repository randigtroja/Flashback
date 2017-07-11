using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Flashback.Model;

namespace Flashback.Services.News
{
    public class NewsService
    {
        private readonly FlashbackHttpClient _httpClient;

        public NewsService()
        {
            _httpClient = new FlashbackHttpClient(new CookieContainer()); // vi behvöer inte använda oss av cookies mot rss-feeden som är på annan domän    
        }

        public async Task<List<FbRssItem>> GetNyheter()
        {
            var result = await _httpClient.GetStringAsync("http://www.flashback.se/rss");
            var nyheter = ParseNyheter(result);

            return nyheter;
        }

        private List<FbRssItem> ParseNyheter(string result)
        {
            // TODO: Fixa den här smutsen sen
            var rssData = from rss in XElement.Parse(result).Descendants("item")
                          let xElement = rss.Element("title")
                          where xElement != null
                          let element = rss.Element("pubDate")
                          where element != null
                          let xElement1 = rss.Element("link")
                          where xElement1 != null
                          let element1 = rss.Element("description")
                          where element1 != null
                          select new FbRssItem
                          {
                              Name = xElement.Value,
                              Date = DateTime.Parse(element.Value).ToString("yyyy-MM-dd"),
                              Description = WebUtility.HtmlDecode(element1.Value),
                              Link = xElement1.Value
                          };

            return rssData.ToList();
        }
    }
}

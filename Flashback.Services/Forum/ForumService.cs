using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Flashback.Model;

namespace Flashback.Services.Forum
{
    public class ForumService
    {
        private readonly FlashbackHttpClient _httpClient;        

        public ForumService(CookieContainer container)
        {
            _httpClient = new FlashbackHttpClient(container);            
        }

        public async Task<ForumList> GetMainForumlist()
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/");
            var forumList = await ParseMainForumlist(result);
            forumList.Id = "mainlist";

            return forumList;
        }

        public async Task<ForumList> GetForums(string forumId)
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/" + forumId);
            var forumList = await ParseForumlist(result);
            forumList.Id = forumId;

            return forumList;
        }

        private async Task<ForumList> ParseForumlist(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                return new ForumList(){Items = new ObservableCollection<FbItem>()};

            var document = await new HtmlParser().ParseAsync(result);

            var forumTitle = WebUtility.HtmlDecode(document.QuerySelector("title").TextContent.Replace(" - Flashback Forum", ""));
            
            // Börjar med själva underformuen
            var forumsParsed = document.QuerySelectorAll("table.forumslist tr:not(.tr_subforum)");

            var forum = new List<FbItem>();
            foreach (var f in forumsParsed)
            {
                var titel = f.QuerySelector("td:first-child a");
                var senastepost = f.QuerySelector("div:last-child td.forum-lastpostinfo div div:last-child");
               
                var fbForum = new FbItem()
                {
                    Id = titel.Attributes["href"].Value,
                    Name = titel.TextContent,
                    Description = WebUtility.HtmlDecode(senastepost.TextContent.FixaRadbrytningar()),
                    ShowForumColor = false,
                    ShowPostCount = false,
                    Type = FbItemType.Forum
                };

                forum.Add(fbForum);
            }

            // Därefter hämtar vi ut eventuella trådar
            var threads = new List<FbItem>();
            var threadsParsed = document.QuerySelectorAll("table#threadslist tr");

            foreach (var t in threadsParsed)
            {
                var threadTitle = t.QuerySelector("td:nth-child(2) a");
                if(threadTitle == null)
                    continue;

                var postCountCheck = t.QuerySelector("td:nth-child(3) div:nth-child(1)");
                int postCount = 0;

                if (postCountCheck != null)
                {                    
                    if (!int.TryParse(postCountCheck.TextContent.FixaRadbrytningar().Replace(" ", "").Replace("svar", ""), out postCount))
                    {
                        continue;
                    }
                }

                var lastPostCheck = t.QuerySelector("td:nth-child(4)");
                string lastPost = "";

                if (lastPostCheck != null)
                {
                    lastPost = lastPostCheck.TextContent.FixaRadbrytningar().Replace("av", " av");
                }

                if(string.IsNullOrWhiteSpace(lastPost))
                    continue;

                bool isSticky = false;

                var stickyCheck = t.QuerySelector("i.fa-thumb-tack");
                if (stickyCheck != null)
                    isSticky = true;
                
                var fbThread = new FbItem()
                {
                    Name = WebUtility.HtmlDecode(threadTitle.TextContent),
                    Id = threadTitle.Attributes["href"].Value.Replace("/",""),
                    ShowPostCount = true,
                    PostCount = postCount,
                    Description = lastPost,
                    IsSticky = isSticky,
                    Type = FbItemType.Thread                    
                };

                threads.Add(fbThread);
            }

            forum.AddRange(threads);

            var pagesCheck = document.QuerySelector("div.row-forum-toolbar ul li span");
            
            Regex regex = new Regex(@"Sidan ([\d]+) av ([\d]+)");
            bool showNavigation = false;
            int currentPage = 0;
            int totalPages = 0;

            if (pagesCheck != null)
            {
                showNavigation = true;

                foreach (Match match in regex.Matches(pagesCheck.FirstChild.TextContent))
                {
                    currentPage = int.Parse(match.Groups[1].Value);
                    totalPages = int.Parse(match.Groups[2].Value);
                }
            }            
            
            return new ForumList()
            {
                Items = new ObservableCollection<FbItem>(forum),
                Title = forumTitle,
                ShowNavigation = showNavigation,
                CurrentPage = currentPage,
                MaxPages = totalPages
            };
        }

        private async Task<ForumList> ParseMainForumlist(string result)
        {
            var document = await new HtmlParser().ParseAsync(result);

            var documentParsed = document.QuerySelectorAll("div.navbar-forum a.forum-title");

            var forum = new List<FbItem>();

            foreach (var f in documentParsed)
            {
#if RELEASE
                 // Detta är "förbjudet" för bajsnödiga MS
                if (f.TextContent.Trim() == "Droger" || f.TextContent.Trim() == "Sex")
                    continue;
#endif

                var fbForum = new FbItem()
                {
                    Id = f.Attributes["href"].Value.Replace("/",""),
                    Name = f.TextContent,
                    ShowForumColor = true,
                    ShowPostCount = false
                };

                forum.Add(fbForum);
            }

            return new ForumList()
            {
                Items = new ObservableCollection<FbItem>(forum),
                Title = "Kategorier"
            };
        }
    }
}

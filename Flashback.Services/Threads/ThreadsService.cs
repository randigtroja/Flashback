using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Flashback.Model;

namespace Flashback.Services.Threads
{
    public class ThreadsService
    {
        private readonly FlashbackHttpClient _httpClient;
        private readonly HtmlRenderOptions _options;
        
        public ThreadsService(CookieContainer container, HtmlRenderOptions options)
        {
            _httpClient = new FlashbackHttpClient(container);
            _options = options;
        }

        /// <summary>
        /// Hämtar aktuella ämnen
        /// </summary>
        /// <returns>Lista med trådar</returns>
        public async Task<List<FbItem>> GetHotTopics()
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/aktuella-amnen/");
            var forum = await ParseHotTopics(result);

            return forum;
        }

        private async Task<List<FbItem>> ParseHotTopics(string result)
        {                        
            var parser = new HtmlParser();

            var document = await parser.ParseAsync(result);

            var threadsParsed = document.QuerySelectorAll("table.table-threads tr");

            List<FbItem> hotTopics = new List<FbItem>();

            foreach (var t in threadsParsed)
            {
                var titleCheck = t.QuerySelector("td:nth-child(2) a");
                string title;

                if (titleCheck != null)
                    title = WebUtility.HtmlDecode(titleCheck.TextContent.FixaRadbrytningar());
                else
                    continue;
                
                int postCount = 0;
                var postCountCheck = t.QuerySelector("td:nth-child(3) div:nth-child(2)");

                if (postCountCheck != null)
                {
                    int pos = postCountCheck.TextContent.IndexOf("•", StringComparison.Ordinal);

                    if (pos != -1)
                    {
                        // fyfan vilket piss
                        var textNumber = postCountCheck.TextContent.Substring(pos + 1, postCountCheck.TextContent.Length - pos - 1);
                        textNumber = textNumber.Replace(" ", "").Replace("&nbsp;", "").Replace("\n", "").Replace("\t", "").Replace("svar", "");

                        int.TryParse(textNumber, out postCount);
                    }
                }

                var descriptionCheck = t.QuerySelector("td:nth-child(2) div:nth-child(2)");
                string description = "";

                if (descriptionCheck != null)
                    description = descriptionCheck.TextContent.FixaRadbrytningar();

                var fbTopic = new FbItem()
                {
                    Id = titleCheck.Attributes["href"].Value.Replace("/",""),
                    Name = title,
                    Type = FbItemType.Thread,
                    Description = description,
                    ShowPostCount = true,
                    ShowForumColor = false,
                    PostCount = postCount
                };

                hotTopics.Add(fbTopic);
            }

            return hotTopics;
        }

        /// <summary>
        /// Hämtar en tråd
        /// </summary>
        /// <param name="id">Id till tråden. Kan skickas in med vilken sida man vill hämta på, eller "s" på slutet för sista sidan</param>
        /// <returns>En modell över en forumtråd. Bygger html utefter inskickad HtmlRenderOptions till ThreadService</returns>
        public async Task<ForumThread> GetForumThread(string id)
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/" + id);
            var forum = await ParseThread(result);
            
            return forum;
        }

        private async Task<ForumThread> ParseThread(string result)
        {
            var parser = new HtmlParser();

            var thread = await parser.ParseAsync(result);
            
            var pageNameCheck = thread.QuerySelector("title");
            string pageName = "";

            if (pageNameCheck != null)
            {
                int pos = pageNameCheck.TextContent.IndexOf(" - Sidan ", StringComparison.Ordinal);
                if (pos != -1)
                {
                    string tmp = pageNameCheck.TextContent.Substring(0, pos);
                    pageName = WebUtility.HtmlDecode(tmp);
                }
                else
                {
                    pageName = WebUtility.HtmlDecode(pageNameCheck.TextContent).Replace(" - Flashback Forum", "");
                }
            }

            int currentPage = 1;
            int maxPages = 1;

            Regex regex = new Regex(@"Sidan ([\d]+) av ([\d]+)");

            var pagesCheck = thread.QuerySelector("div.row-forum-toolbar ul.pagination span");
            bool showNavigation = false;

            if (pagesCheck != null)
            {
                showNavigation = true;

                foreach (Match match in regex.Matches(pagesCheck.TextContent))
                {
                    currentPage = int.Parse(match.Groups[1].Value);
                    maxPages = int.Parse(match.Groups[2].Value);
                }
            }

           
            var realId = thread.QuerySelector("head meta[property='og:url']").Attributes["content"].Value;
            realId = realId.Replace("https://www.flashback.org/","") + "p" + currentPage;


            var parentCheck = thread.QuerySelector("div.list-forum-title ol li:last-child a");
            string parentId = "";

            if (parentCheck != null)
            {
                parentId = parentCheck.Attributes["href"].Value.Replace("/","");                
            }

            var poster = thread.QuerySelectorAll("div#posts div.post");

            var replyId ="";
            foreach (var post in poster)
            {
                replyId = post.Attributes["id"].Value;
                replyId = replyId.Replace("post", "");

                if (!string.IsNullOrWhiteSpace(replyId))                
                    break;                
            }

            string html = _options.GetHtmlHeaders() +  BuildHtmlForForumThreads(poster) + _options.GetHtmlFooter();           

            return new ForumThread()
            {
                Title = pageName,
                CurrentPage = currentPage,
                ShowNavigation = showNavigation,
                MaxPages = maxPages,
                ParentId = parentId,
                Html = html,
                Id = realId,
                ReplyId = replyId
            };
        }

        /// <summary>
        /// Bygger upp html för enbart alla inlägg i en tråd        
        /// </summary>
        /// <param name="poster"></param>
        /// <returns></returns>
        private string BuildHtmlForForumThreads(IHtmlCollection<IElement> poster)
        {
            if (poster == null)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (var post in poster)
            {
                var userNameCheck = post.QuerySelector("a.post-user-username");
                string userName = "";

                if (userNameCheck != null)
                    userName = userNameCheck.TextContent.FixaRadbrytningar();
                
                var userOnlineCheck = post.QuerySelector("div.post-user-title i[title='online']");
                string onlinestatus;

                if (userOnlineCheck != null)
                {
                    onlinestatus = "<span style=\"color:rgb(92, 140, 111);font-size:50%\">&#9632; </span>";
                }
                else
                {
                    onlinestatus = "<span style=\"color:rgb(213, 89, 89);font-size:50%\">&#9632; </span>";
                }

                userName = onlinestatus + userName;

                var avatarCheck = post.QuerySelector("a.post-user-avatar img");
                string avatar = "";
                if (avatarCheck != null && _options.ShowAvatars)
                {
                    avatar = "<img src=\"" + avatarCheck.Attributes["src"].Value + "\" class=\"avatar\" OnError=\"this.style.display = 'none'\">";
                }

                var postDateCheck = post.QuerySelector("div.post-heading");
                var postLink = "";
                if (postDateCheck != null)
                {
                    userName += " - " + postDateCheck.TextContent.FixaRadbrytningar();

                    var anchorCheck = postDateCheck.QuerySelector("a.jumptarget");
                    if (anchorCheck != null)
                    {
                        postLink = anchorCheck.Attributes["name"].Value;
                    }
                }

                var postMessageCheck = post.QuerySelector("div.post_message");
                string postMessage;
                if (postMessageCheck != null)
                {
                    postMessage = postMessageCheck.InnerHtml;
                    postMessage = postMessage.Replace("/leave.php?u=", "");
                }
                else
                {
                    continue;
                }
                
                if (_options.RenderEmoticons)
                {
                    postMessage = _options.ReplaceSmileys(postMessage);
                }

                sb.AppendLine("<a id=\"" + postLink + "\" href=\"#" + postLink +  "\"></a>");
                sb.AppendLine("<p>");
                sb.AppendLine(avatar);
                sb.AppendLine("<strong>");
                sb.AppendLine(userName);
                sb.AppendLine("</strong>");
                sb.AppendLine("</p>");
                sb.AppendLine(postMessage);
                sb.AppendLine("<hr noshade>");
                sb.AppendLine("<br>");
            }
           
            return sb.ToString();
        }

        /// <summary>
        /// Hämtar nya trådar
        /// </summary>
        /// <returns>Lista med trådar</returns>
        public async Task<List<FbItem>> GetNewTopics()
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/nya-amnen");
            var topics = await ParseNewTopics(result);
            
            return topics;
        }

        private async Task<List<FbItem>> ParseNewTopics(string result)
        {
            var parser = new HtmlParser();

            var topics = await parser.ParseAsync(result);            
            var postsCheck = topics.QuerySelectorAll("table.table-threads tr");

            var newTopics = new List<FbItem>();

            if (postsCheck != null)
            {
                foreach (var post in postsCheck)
                {
                    var item = new FbItem()
                    {
                        ShowPostCount = true, ShowForumColor = false, Type = FbItemType.Thread                        
                    };

                    var titleCheck = post.QuerySelector("a.thread-title");
                    if (titleCheck != null)
                    {
                        item.Id = titleCheck.Attributes["href"].Value.Replace("/", "");
                        item.Name = WebUtility.HtmlDecode(titleCheck.TextContent).FixaRadbrytningar();
                    }
                    else
                    {
                        continue;
                    }

                    var subforumCheck = post.QuerySelector("td:nth-child(2) div:nth-child(2) a");
                    if (subforumCheck != null)
                    {
                        item.Description = WebUtility.HtmlDecode(subforumCheck.TextContent).FixaRadbrytningar();
                    }

                    var countCheck = post.QuerySelector("td:nth-child(3) div:nth-child(2)");
                    int number = 0;

                    if (countCheck != null)
                    {
                        int pos = countCheck.TextContent.IndexOf("•", StringComparison.Ordinal);

                        if (pos != -1)
                        {
                            // fyfan vilket piss
                            var textNumber = countCheck.TextContent.Substring(pos + 1, countCheck.TextContent.Length - pos - 1);
                            textNumber = textNumber.FixaRadbrytningar().Replace(" ", "").Replace("svar", "");

                            int.TryParse(textNumber, out number);
                        }
                    }

                    item.PostCount = number;

                    newTopics.Add(item);
                }

                return newTopics;
            }
            else
            {
                return new List<FbItem>();
            }
        }

        /// <summary>
        /// Hämtar dom senaste inläggen på forumet
        /// </summary>
        /// <returns>Lista med trådar</returns>
        public async Task<List<FbItem>> GetNewPosts()
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/nya-inlagg");
            var topics = await ParseNewTopics(result); // funkar att använda samma metod för nyainlägg. Bryt ut om det börjar diffa

            return topics;
        }

        /// <summary>
        /// Returnerar användarens prenumerationer
        /// </summary>
        /// <returns>Lista med trådar</returns>
        public async Task<List<FbFavourite>> GetFavourites()
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/subscription.php");
            var favourites = await ParseFavourites(result);

            return favourites;           
        }

        private async Task<List<FbFavourite>> ParseFavourites(string result)
        {
            var parser = new HtmlParser();

            var resultFavourites = new List<FbFavourite>();

            var favourites = await parser.ParseAsync(result);

            // börjar först med favoriter som är a typen forum            
            var favouritesForumCheck = favourites.QuerySelectorAll("table.forumslist tr.tr_subforum");
            if (favouritesForumCheck != null)
            {
                foreach (var forum in favouritesForumCheck)
                {


                    FbFavourite item = new FbFavourite() { Type = FbItemType.Forum};

                    var titleCheck = forum.QuerySelector("td:nth-child(2) div a:nth-child(1)");
                    string title = "";
                    if (titleCheck != null)
                    {
                        item.Name = WebUtility.HtmlDecode(titleCheck.TextContent).FixaRadbrytningar();
                        item.Id = titleCheck.Attributes["href"].Value.Replace("/","").Replace("n","");

                        if (titleCheck.Parent != null && titleCheck.Parent.NodeName == "STRONG")
                        {
                            item.IsUnread = true;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    var senasteCheck = forum.QuerySelector("td:nth-child(1) div:nth-child(2) div a");
                    if (senasteCheck != null)
                    {
                        item.LastPost = WebUtility.HtmlDecode(senasteCheck.TextContent.FixaRadbrytningar().Replace("av", " av").Trim());
                    }

                    var idCheck = forum.QuerySelector("td:nth-child(3) input");
                    if (idCheck != null)
                    {
                        item.FbId = idCheck.Attributes[1].Value;
                    }                    

                    resultFavourites.Add(item);                    
                }
            }

            // därefter tar vi vanliga favoriter
            Regex regex = new Regex(@"Svar: ([\d]+)");
            var favouritesThreadsCheck = favourites.QuerySelectorAll("table#threadslist tr");
            if (favouritesThreadsCheck != null)
            {
                foreach (var thread in favouritesThreadsCheck)
                {
                    var item = new FbFavourite()
                    {
                        Type = FbItemType.Thread, ShowPostCount = true, ShowForumColor = false                        
                    };

                    var titleCheck = thread.QuerySelector("td:nth-child(2) div a");

                    if (titleCheck != null)
                    {
                        item.Name = WebUtility.HtmlDecode(titleCheck.TextContent).FixaRadbrytningar();
                        item.Id = titleCheck.Attributes["href"].Value.Replace("/", "");

                        if (titleCheck.Parent != null && titleCheck.Parent.NodeName == "STRONG")
                        {
                            item.IsUnread = true;
                        }
                    }
                    else
                    {
                        continue;
                    }
                    
                    var usernameCheck = thread.QuerySelector("td:nth-child(3) div:nth-child(2) a:nth-child(1)");
                    string username = "";
                    if (usernameCheck != null)
                    {
                        username = WebUtility.HtmlDecode(usernameCheck.TextContent.FixaRadbrytningar());
                    }

                    var lastPostCheck = thread.QuerySelector("td:nth-child(3) div");
                    if (lastPostCheck != null)
                    {
                        item.LastPost = WebUtility.HtmlDecode(lastPostCheck.TextContent.FixaRadbrytningar().Replace("av", " av").Trim()) + " av " + username;
                    }

                    var countCheck = thread.QuerySelector("td:nth-child(3)");
                    if (countCheck != null)
                    {
                        foreach (Match match in regex.Matches(countCheck.Attributes[1].Value.Replace(" ", "").FixaRadbrytningar()))
                        {
                            item.PostCount = int.Parse(match.Groups[1].Value);
                        }
                    }

                    var favourteIdCheck = thread.QuerySelector("td:nth-child(4) input");
                    if (favourteIdCheck != null)
                    {
                        item.FbId = favourteIdCheck.Attributes[1].Value;
                    }

                    resultFavourites.Add(item);
                }
            }

            return resultFavourites;
        }

        /// <summary>
        /// Hämtar ut alla startade trådar för en viss användare
        /// </summary>
        /// <param name="userId">användarens Id</param>
        /// <returns>Lista med trådar</returns>
        public async Task<List<FbItem>> GetMyStartedThreads(string userId)
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/find_threads_by_user.php?userid=" + userId);
            var favourites = await ParseMyStartedThreads(result);

            return favourites;            
        }

        private async Task<List<FbItem>> ParseMyStartedThreads(string result)
        {

            var parser = new HtmlParser();

            var threads = await parser.ParseAsync(result);
            var threadsCheck = threads.QuerySelectorAll("table#threadslist tr");

            var resultThreads = new List<FbItem>();
            
            if (threadsCheck != null)
            {
                foreach (var thread in threadsCheck)
                {
                    var item = new FbItem()
                    {
                        Type = FbItemType.Thread,
                        ShowPostCount = true,
                        ShowForumColor = false
                    };

                    var titleCheck = thread.QuerySelector("td:nth-child(2) div a");

                    if (titleCheck != null)
                    {
                        item.Name = WebUtility.HtmlDecode(titleCheck.TextContent).FixaRadbrytningar();
                        item.Id = titleCheck.Attributes["href"].Value.Replace("/", "");                        
                    }
                    else
                    {
                        continue;
                    }

                    var subforumCheck = thread.QuerySelector("td:nth-child(2) a.thread-forum-title");
                    if (subforumCheck != null)
                    {
                        item.Description = WebUtility.HtmlDecode(subforumCheck.TextContent).FixaRadbrytningar();
                    }

                    var countCheck = thread.QuerySelector("td:nth-child(3) div:nth-child(1) a");
                    if (countCheck != null)
                    {
                        
                        item.PostCount = int.Parse(countCheck.TextContent.Replace("&nbsp;", "").Replace("svar", ""));                        
                    }

                    resultThreads.Add(item);
                }
            }

            return resultThreads;
        }

        /// <summary>
        /// Lägger till en tråd till favoriterna.
        /// </summary>
        /// <param name="forumThreadId">Id till tråden. Ska skickas in utan "t"</param>
        /// <returns>Om anropet gick bra eller ej</returns>
        public async Task<bool> AddThreadToFavourites(string forumThreadId)
        {
            var postData = new List<KeyValuePair<string, string>>
                {                    
                    new KeyValuePair<string, string>("do", "doaddsubscription"),
                    new KeyValuePair<string, string>("threadid", forumThreadId),
                    new KeyValuePair<string, string>("url", "https://www.flashback.org/t" + forumThreadId),
                    new KeyValuePair<string, string>("folderid", "0"),  // todo: styr till vilken mapp, pressentera något? Eller bara skita i det som tidigare?
                    new KeyValuePair<string, string>("emailupdate", "0") // todo: läs ut användarens inställningar eller bara skita i det som tidigare?
                };

            var postContent = new FlashbackStringUrlContent(postData);
          
            var response = await _httpClient.PostAsync("https://www.flashback.org/subscription.php", postContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<FbItem>> GetMyQuotedPosts(string userId)
        {
            // här kan vi göra på två sätt.
            // 1) spara ner användarnamnet vid inloggningen som behövs för att kunna anropa rätt länk med citerade inlägg
            // 2) göra ett extra request till siten för att plocka ut namnet från FBs genererade sida och anropa med det.

            // alt.1 har fördelen att det går snabbare, men innebär mer kod samt att vi behöver spara ner ytterligare saker till
            // disk som har med användaren att göra. Jag väljer för stunden alt. 2
            
            var userName = await GetLoggedInUserName(userId);
            //todo: tror vi måste encoda användarnamnet för att klara användare med exotiska namn

            var result = await _httpClient.GetStringAsync("https://www.flashback.org/sok/quote=" + userName + "?sp=1&so=d");
            var quotes = await ParseMyQuotedPosts(result);

            return quotes;
        }

        private async Task<List<FbItem>> ParseMyQuotedPosts(string result)
        {
            var parser = new HtmlParser();

            var quotedPosts = await parser.ParseAsync(result);

            var quotedPostsList = new List<FbItem>();

            // var quotes = root.SelectNodes("/html/body/div[@id='site']/div[@id='site-content']//div[@id='site-left']/div[@id='posts']/div");
            var postsCheck = quotedPosts.QuerySelectorAll("div#site-left div#posts div.post");

            if (postsCheck != null)
            {
                foreach (var post in postsCheck)
                {

                    var item = new FbItem() {Type = FbItemType.Thread};

                    var titleCheck = post.QuerySelector("div.post-body a strong");
                    if (titleCheck != null)
                    {
                        item.Name = WebUtility.HtmlDecode(titleCheck.TextContent.FixaRadbrytningar());
                    }

                    //var idCheck = post.QuerySelector("div.post-body a");
                    var idCheck = post.QuerySelector("div.post-body div:nth-child(2) a");
                    if (idCheck != null)
                    {
                        string id = idCheck.Attributes["href"].Value;
                        id = id.Replace("/", "");
                        item.Id = id;
                    }

                    
                    string date = "";
                    var dateCheck = post.QuerySelector("div.post-heading").LastChild;
                    if (dateCheck != null)
                    {
                        date = WebUtility.HtmlDecode(dateCheck.TextContent.FixaRadbrytningar());
                    }

                    string username = "";
                    var usercheck = post.QuerySelector("div.post-body small a");
                    if (usercheck != null)
                    {
                        username = usercheck.TextContent.FixaRadbrytningar();
                    }

                    item.Description = date + " - " + username;
                                        
                    quotedPostsList.Add(item);

                }
            }

            return quotedPostsList;
        }

        private async Task<string> GetLoggedInUserName(string userId)
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/u" + userId);

            var parser = new HtmlParser();

            var userProfilePage = await parser.ParseAsync(result);

            var userNameCheck = userProfilePage.QuerySelector("div.panel-heading span.panel-title");

            if (userNameCheck != null)
            {
                return WebUtility.HtmlDecode(userNameCheck.TextContent.FixaRadbrytningar());
            }
            else
            {
                throw new Exception("hittade inte användarnamnet");
            }
        }

        /// <summary>
        /// Söker fram trådar på forumet.
        /// </summary>
        /// <param name="searchTerm">Det man söker på</param>
        /// <param name="forumId">Eventuellt forum och dess underforum som man söker i, null = rubbet</param>
        /// <returns></returns>
        public async Task<List<FbItem>> SearchThreads(string searchTerm, string forumId)
        {
            string searchUrl;
            if (string.IsNullOrWhiteSpace(forumId))
            {                
                searchUrl = "https://www.flashback.org/sok/" + searchTerm;
            }
            else
            {                
                searchUrl = "https://www.flashback.org/sok/" + searchTerm + "?f=" + forumId.Replace("/","").Replace("f","");
            }

            var result = await _httpClient.GetStringAsync(searchUrl);

            return await ParseSearchResult(result);            
        }

        private async Task<List<FbItem>> ParseSearchResult(string result)
        {
            var parser = new HtmlParser();

            var quotedPosts = await parser.ParseAsync(result);

            var searchResult = new List<FbItem>();

            Regex regex = new Regex(@"Svar: ([\d]+)");

            var searchCheck = quotedPosts.QuerySelectorAll("table#threadslist tr");
            if (searchCheck != null)
            {
                foreach (var searchRow in searchCheck)
                {
                    var item = new FbItem(){Type = FbItemType.Thread, ShowPostCount = true};

                    var titleCheck = searchRow.QuerySelector("td:nth-child(2) div a");
                    if (titleCheck != null)
                    {
                        item.Name = WebUtility.HtmlDecode(titleCheck.TextContent);
                        string id = titleCheck.Attributes["href"].Value;
                        id = id.Replace("/", "");                        
                        item.Id = id;
                    }
                    else
                    {
                        continue;
                    }

                    var descriptionCheck = searchRow.QuerySelector("td:nth-child(2) a.thread-forum-title");
                    if (descriptionCheck != null)
                    {
                        item.Description = descriptionCheck.TextContent;
                    }

                    var countCheck = searchRow.QuerySelector("td:nth-child(4)");
                    if (countCheck != null)
                    {

                        foreach (Match match in regex.Matches(countCheck.Attributes[1].Value.FixaRadbrytningar()))
                        {
                            item.PostCount = int.Parse(match.Groups[1].Value);
                        }
                    }

                    searchResult.Add(item);

                }
            }

            return searchResult;

        }

        public async Task<PostReplyModel> GetReply(string id, bool isQuote)
        {
            string url;

            if (isQuote)
            {
                url = "https://www.flashback.org/newreply.php?do=newreply&p=" + id;
            }
            else
            {
                url = "https://www.flashback.org/newreply.php?do=newreply&noquote=1&p=" + id;
            }

            var result = await _httpClient.GetStringAsync(url);

            var reply = await ParsePostReply(result);

            return reply;
        }

        private async Task<PostReplyModel> ParsePostReply(string result)
        {
            var model = new PostReplyModel();

            var parser = new HtmlParser();

            var reply = await parser.ParseAsync(result);

            var titleCheck = reply.QuerySelector("div.page-title h1 a");
            model.Title = titleCheck != null ? titleCheck.TextContent : "";

            var userCheck = reply.QuerySelector("input[name='loggedinuser']");
            if (userCheck != null)
            {
                model.UserId = userCheck.Attributes["value"].Value;
            }

            var threadIdCheck = reply.QuerySelector("input[name='t']");
            if (threadIdCheck != null)
            {
                model.ThreadId = threadIdCheck.Attributes["value"].Value;
            }

            var subscriptionTypeCheck = reply.QuerySelector("select[name='emailupdate'] option[selected]");
            if (subscriptionTypeCheck != null)
            {
                model.SubscriptionType = subscriptionTypeCheck.Attributes["value"].Value;
            }
            else
            {
                model.SubscriptionType = "9999";
            }

            var messageCheck = reply.QuerySelector("textarea");
            if (messageCheck != null)
            {
                model.Message = WebUtility.HtmlDecode(messageCheck.InnerHtml);
            }
            
            return model;
        }
    }
}

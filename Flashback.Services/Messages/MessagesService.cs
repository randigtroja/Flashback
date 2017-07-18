using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Flashback.Model;

namespace Flashback.Services.Messages
{
    public class MessagesService
    {
        private readonly FlashbackHttpClient _httpClient;

        public MessagesService(CookieContainer container)
        {
            _httpClient = new FlashbackHttpClient(container);
        }

        public async Task<List<PrivateMessage>> GetPrivateMessages()
        {
            var result = await _httpClient.GetStringAsync("https://www.flashback.org/private.php");
            var messages = await ParsePrivateMessages(result);
            
            return messages;
        }

        private async Task<List<PrivateMessage>> ParsePrivateMessages(string result)
        {
            var document = await new HtmlParser().ParseAsync(result);
            List<PrivateMessage> privateMessages = new List<PrivateMessage>();

            var messagesCheck = document.QuerySelectorAll("table tbody[id] tr");
            if (messagesCheck != null)
            {
                foreach (var message in messagesCheck)
                {
                    var item = new PrivateMessage();

                    var title = message.QuerySelector("td:nth-child(1) div div:nth-child(1) div:nth-child(1) a");

                    if (title != null)
                    {
                        item.Name = WebUtility.HtmlDecode(title.TextContent.FixaRadbrytningar());
                    }
                    else
                    {
                        continue;    
                    }

                    var fromCheck = message.QuerySelector("td:nth-child(1) div div:nth-child(1) div:nth-child(2) a");
                    if (fromCheck != null)
                    {
                        item.FromName = WebUtility.HtmlDecode(fromCheck.TextContent);

                        if (fromCheck.Parent != null && fromCheck.Parent.NodeName == "STRONG")
                        {
                            item.IsUnread = true;
                        }
                    }

                    
                    
                    Uri uri = new Uri("http://www.flashback.org" + "/" + title.Attributes["href"].Value.Replace("&amp;", "&"));

                    var parameterValue = uri.Query.Split('&')
                                        .Where(s => s.Split('=')[0] == "pmid")
                                        .Select(s => s.Split('=')[1])
                                        .FirstOrDefault();

                    item.Id = parameterValue;


                        // Verkar inte behövas längre?

                        //var parameterValue2 = uri.Query.Split('&')
                        //                    .Where(s => s.Split('=')[0] == "token")
                        //                    .Select(s => s.Split('=')[1])
                        //                    .FirstOrDefault();

                        //item.Token = parameterValue2;



                    

                    privateMessages.Add(item);
                }
            }

            return privateMessages;
        }

        public async Task<ComposePrivateMessageModel> NewPrivateMessage(string id)
        {
            string pmUrl = "";

            if (string.IsNullOrWhiteSpace(id))
            {
                pmUrl = "https://www.flashback.org/private.php?do=newpm";
            }
            else
            {
                pmUrl = "https://www.flashback.org/private.php?do=newpm&pmid=" + id;
            }

            var result = await _httpClient.GetStringAsync(pmUrl);

            var messageModel = await ParseNewPrivateMessage(result);

            return messageModel;

        }

        private async Task<ComposePrivateMessageModel> ParseNewPrivateMessage(string result)
        {
            var document = await new HtmlParser().ParseAsync(result);

            var model = new ComposePrivateMessageModel();


            var textCheck = document.QuerySelector("textarea[name='message']");
            if (textCheck != null)
            {
                model.Message = WebUtility.HtmlDecode(textCheck.InnerHtml);
            }

            var tokenCheck = document.QuerySelector("input[name='csrftoken']");
            if (tokenCheck != null)
            {
                model.PostToken = tokenCheck.Attributes["value"].Value;
            }

            var subjectCheck = document.QuerySelector("input[name='title']");

            if (subjectCheck != null)
            {
                model.Subject = WebUtility.HtmlDecode(subjectCheck.Attributes["value"].Value);
            }

            var toCheck = document.QuerySelector("select[name='recipients[]'] option");

            if (toCheck != null)
            {
                model.To = WebUtility.HtmlDecode(toCheck.TextContent);
            }
        
            return model;
        }
    }
}

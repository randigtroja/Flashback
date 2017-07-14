using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
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
            throw new NotImplementedException();
        }
    }
}

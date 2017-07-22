using System;
using System.Collections.Generic;
using System.Text;

namespace Flashback.Model
{
    public class PostReplyModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string ThreadId { get; set; }
        public string SubscriptionType { get; set; }
    }
}

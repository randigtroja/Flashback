﻿namespace Flashback.Model
{
    public class ForumThread
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool ShowNavigation { get; set; }
        public int CurrentPage { get; set; }
        public int MaxPages { get; set; }
        public string PagenumberText => string.Format("{0}/{1}", CurrentPage, MaxPages);
        public string ParentId { get; set; }
        public string Html { get; set; }
        public string ReplyId { get; set; }
        public int? UnreadMessagesCount { get; set; }
    }
}

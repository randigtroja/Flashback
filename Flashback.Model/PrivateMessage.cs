using System;

namespace Flashback.Model
{
    public class PrivateMessage
    {                  
        public string Id { get; set; }            
        public string Token { get; set; }            
        public string Name { get; set; }            
        public string FromName { get; set; }            
        public DateTime Date { get; set; }            
        public string Message { get; set; }         
        public bool IsUnread { get; set; }
        public string FolderId { get; set; }
    }
}

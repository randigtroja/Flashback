using System.Collections.ObjectModel;

namespace Flashback.Model
{
    public class ForumList
    {
        public string Title { get; set; }
        public bool ShowNavigation { get; set; }
        public int CurrentPage { get; set; }
        public int MaxPages { get; set; }   
        public ObservableCollection<FbItem> Items { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string PagenumberText => string.Format("{0}/{1}", CurrentPage, MaxPages);
    }
}

namespace Flashback.Model
{
    public class FbItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public FbItemType Type { get; set; }
        public int PostCount { get; set; }
        public string LastPost { get; set; }
        public bool IsSticky { get; set; }        
        public string Description { get; set; }        
        public bool ShowForumColor { get; set; }
        public bool ShowPostCount { get; set; }
        public string XamlCode
        {
            get
            {
                if (IsSticky)
                    return "\uE141";

                return Type == FbItemType.Forum ? "\uE8B7" : "\uE7C3";
            }
        }

        public string PostCountString => ShowPostCount ? PostCount.ToString() : string.Empty;
    }
}

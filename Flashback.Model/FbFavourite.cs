using System;
using System.Collections.Generic;
using System.Text;

namespace Flashback.Model
{
    public class FbFavourite : FbItem
    {
        public bool IsUnread { get; set; }
        public string FbId { get; set; }
    }
}

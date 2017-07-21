namespace Flashback.Model
{
    /// <summary>
    /// Används för att styra hur html-ska byggas upp för forumtrådar och privata meddelanden
    /// Värden kommer från appens egna inställningar
    /// </summary>
    public class HtmlRenderOptions
    {
        public bool IsDarkThemed { get; set; }
        public string AccentColor { get; set; }
        public bool ShowAvatars { get; set; }
        public string FontSize { get; set; }
        public bool RenderEmoticons { get; set; }
   
        public string GetForeColor()
        {
            if (this.IsDarkThemed)
                return "#ffffff";
            else
                return "#000000";
        }

        public string GetBackgroundColor()
        {
            if (IsDarkThemed)
                return "#000000";
            else
                return "#ffffff";
        }

        public string GetQuotesBackground()
        {
            if (IsDarkThemed)
                return "#3D3D3D";
            else
                return "#DEDEDE";
        }

        public string GetQuotesBorder()
        {
            if (IsDarkThemed)
                return "#ffffff";
            else
                return "#000000";
        }

        public string ReplaceSmileys(string postMessage)
        {
            // Todo: Fyll på med fler replace
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/wink.gif\" border=\"0\" alt=\"\" title=\"Whink\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128521;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/sad.gif\" border=\"0\" alt=\"\" title=\"Sad\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128543;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/noexpression.gif\" border=\"0\" alt=\"\" title=\"Noexpression\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128529;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/cool2.gif\" border=\"0\" alt=\"\" title=\"Cool\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128526;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/skamsen.gif\" border=\"0\" alt=\"\" title=\"Skamsen\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128563;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/sad44.gif\" border=\"0\" alt=\"\" title=\"Sad44\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128557;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/unsure.gif\" border=\"0\" alt=\"\" title=\"Unsure\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128533;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/grin.gif\" border=\"0\" alt=\"\" title=\"Grin\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128512;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/tongue.gif\" border=\"0\" alt=\"\" title=\"Tongue\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128539;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/smile1.gif\" border=\"0\" alt=\"\" title=\"Smile\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#x1F642;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/laugh.gif\" border=\"0\" alt=\"\" title=\"Laugh\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128516;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/cry.gif\" border=\"0\" alt=\"\" title=\"Cry\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128546;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/ohmy.gif\" border=\"0\" alt=\"\" title=\"Ohmy\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128559;</span>");
            postMessage = postMessage.Replace("<img src=\"https://static.flashback.org/img/smilies2/evilgrin39.gif\" border=\"0\" alt=\"\" title=\"Evilgrin39\" class=\"inlineimg\">", "<span style=\"font-family: Segoe UI Emoji;\">&#128520;</span>");
            
            return postMessage;           
        }
    }
}

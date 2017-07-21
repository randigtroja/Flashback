using System;
using System.Text;

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

        private string GetForeColor()
        {
            if (this.IsDarkThemed)
                return "#ffffff";
            else
                return "#000000";
        }

        private string GetBackgroundColor()
        {
            if (IsDarkThemed)
                return "#000000";
            else
                return "#ffffff";
        }

        private string GetQuotesBackground()
        {
            if (IsDarkThemed)
                return "#3D3D3D";
            else
                return "#DEDEDE";
        }

        private string GetQuotesBorder()
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

        /// <summary>
        /// Returnerar en standardblaffa för att få enhetlig html och funktioner på samtliga ställen där vi använder oss av en WebView (just nu trådar och privata meddelanden)
        /// </summary>
        /// <returns>Ger oss början på html ner tills bodyn börjar</returns>
        public string GetHtmlHeaders()
        {
            var foreColor = GetForeColor();
            var quotesBackground = GetQuotesBackground();
            var quotesBorder = GetQuotesBorder();
            var fontSize = FontSize;
            var accentColor = AccentColor;
            var backgroundColor = GetBackgroundColor();
            
            var htmlHeaders = 
                            "<meta name=\"viewport\" content=\"width=device-width, user-scalable=no\" />" +
                                "<style TYPE=\"text/css\">" +
                                    "div.post-bbcode-quote {color:" + foreColor + ";background:" + quotesBackground + ";border-style:solid;border-color:" + quotesBorder + ";border-width:1px;font-size:" + fontSize + "; };" +
                                    "td.post-quote img {border: 0px;}" +
                                    "a {color:" + accentColor + ";} " +
                                    "div.post-bbcode-quote-wrapper { width:95%;} " +
                                    "table{table-layout:fixed;}  " +
                                    "blockquote { width: 95%;margin-left:20px; background:" + quotesBackground + ";border-style:solid;border-color:" + quotesBorder + ";border-width:1px;} " +
                                    "i { width:90%;} " +
                                    "img.avatar { width:40px;vertical-align:text-bottom; margin-right:12px; }" +
                                    ".hidden {display: none;}" +
                                    ".post-bbcode-spoiler {color: " + foreColor + "; background:" + quotesBackground + ";border-style:solid;border-color:" + quotesBorder + ";" +"border-width:1px; " +"font-size:" + fontSize + "; };" +
                                "</style>";


            var sb = new StringBuilder();
            
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine(htmlHeaders);
            sb.AppendLine(
                "<script type=\"text/javascript\">" +
                    "window.onload = function() { " +
                        "document.addEventListener(\'touchstart\', handleTouchStart, false);" +
                        "document.addEventListener(\'touchmove\', handleTouchMove, false);" +
                         "var xDown = null;" +
                         "var yDown = null;" +
                    
                         "function handleTouchStart(evt) {" +
                            "xDown = evt.touches[0].clientX;" +
                            "yDown = evt.touches[0].clientY;" +
                         "};" +
                    
                         "function handleTouchMove(evt) {" +
                            "if ( ! xDown || ! yDown ) {" +
                                "return;" +
                         "}" +
                    
                         "var xUp = evt.touches[0].clientX;" +
                    
                         "var yUp = evt.touches[0].clientY;" +                    
                         "var xDiff = xDown - xUp;" +                       
                         "var yDiff = yDown - yUp;" +
                        "if ( Math.abs( xDiff ) > Math.abs( yDiff ) ) {" +
                            "if ( xDiff > 0 ) {" +
                                "window.external.notify('left');" +
                            "} else {" +
                                "window.external.notify('right');" +
                            "} " +
                        "} else {" +
                            "if ( yDiff > 0 ) {" +
                        "} else {" +"}" +
                        "}" +
                        
                    "xDown = null;" +
                    "yDown = null;" +
                "}};</script>");

            sb.AppendLine("</head>");
            sb.AppendLine("<body style=\"margin:0px;font-family:'Segoe UI';background-color:" + backgroundColor + ";font-size: " + fontSize + ";\">");
            sb.AppendLine("<div id=\"pageWrapper\" style=\"width:100%; color:" + foreColor + ";word-wrap: break-word\">");
            sb.AppendLine("<div style=\"display:none\">" + Guid.NewGuid() + "</div>"); // verkar inte alltid slängas loadedeventet annars. Nån cache?

            return sb.ToString();
        }

        /// <summary>
        /// Returnerar sluttampen som behövs för att rendera en korrekt html-vy.
        /// </summary>
        /// <returns></returns>
        public string GetHtmlFooter()
        {
            StringBuilder sb = new StringBuilder();

            var spoilerScript = "<script>" +
                                        "var elements = document.querySelectorAll('[data-toggle=\"hidden\"]');" +
                                            "Array.prototype.forEach.call(elements, function(el, i) {el.onclick = function() {" +
                                                "el.nextElementSibling.classList.toggle(\"hidden\");" +
                                        "}});" +
                                    "</script>";

            sb.AppendLine(spoilerScript);
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }
}

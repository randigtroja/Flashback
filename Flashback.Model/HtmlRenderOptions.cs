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
        public bool ShowSignatures { get; set; }

        private string GetForeColor() => IsDarkThemed ? "#ffffff" : "#000000";
        private string GetBackgroundColor() => IsDarkThemed ? "#000000" : "#ffffff";
        private string GetQuotesBackground() => IsDarkThemed ? "#3d3d3d" : "#dedede";
        private string GetQuotesBorder() => IsDarkThemed ? "#ffffff" : "#000000";

        public string ReplaceSmileys(string postMessage)
        {
            void _replace(string fileName, string title, string emoticon)
            {
                postMessage = postMessage.Replace(
                    $"<img src=\"https://static.flashback.org/img/smilies2/{fileName}\" border=\"0\" alt=\"\" title=\"{title}\" class=\"inlineimg\">",
                    $"<span style='font-family: Segoe UI Emoji;'>{emoticon}</span>");
            }

            // Todo: Fyll på med fler replace
            _replace("wink.gif", "Whink", "&#128521;");
            _replace("sad.gif", "Sad", "&#128543;");
            _replace("noexpression.gif", "Noexpression", "&#128529;");
            _replace("cool2.gif", "Cool", "&#128526;");
            _replace("skamsen.gif", "Skamsen", "&#128563;");
            _replace("sad44.gif", "Sad44", "&#128557;");
            _replace("unsure.gif", "Unsure", "&#128533;");
            _replace("grin.gif", "Grin", "&#128512;");
            _replace("tongue.gif", "Tongue", "&#128539;");
            _replace("smile1.gif", "Smile", "&#x1F642;");
            _replace("laugh.gif", "Laugh", "&#128516;");
            _replace("cry.gif", "Cry", "&#128546;");
            _replace("ohmy.gif", "Ohmy", "&#128559;");
            _replace("evilgrin39.gif", "Evilgrin39", "&#128520;");

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
                                    "div.post-bbcode-quote {color:" + foreColor + ";background:" + quotesBackground + ";border-style:solid;padding:12px;border-color:" + quotesBorder + ";border-width:1px;font-size:" + fontSize + "; };" +
                                    "td.post-quote img {border: 0px;}" +
                                    "a {color:" + accentColor + "; text-decoration:none;} " +
                                    "div.post-bbcode-quote-wrapper { width:95%;} " +
                                    "table{table-layout:fixed;}  " +
                                    "blockquote { width: 95%;margin-left:20px; background:" + quotesBackground + ";border-style:solid;border-color:" + quotesBorder + ";border-width:1px;} " +
                                    "i { width:90%;} " +
                                    "img.avatar { width:40px;vertical-align:text-bottom; margin-right:12px; }" +
                                    ".hidden {display: none;}" +
                                    ".post-bbcode-spoiler {color: " + foreColor + "; background:" + quotesBackground + ";border-style:solid;border-color:" + quotesBorder + ";" +"border-width:1px; " +"font-size:" + fontSize + "; }" +
                                    ".signature {color: #666;}" +
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


            sb.AppendLine("<script type=\"text/javascript\">" + 
                            "function prepareQuote(i) {" +
                                "window.external.notify(i);" +
                            "};" +
                          "</script>");

            sb.AppendLine("</head>");
            sb.AppendLine("<body style=\"margin:12px;font-family:'Segoe UI';background-color:" + backgroundColor + ";font-size: " + fontSize + ";\">");
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

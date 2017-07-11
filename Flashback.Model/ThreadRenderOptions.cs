namespace Flashback.Model
{
    /// <summary>
    /// Används för att styra hur html-ska byggas upp för forumtrådar
    /// Värden kommer från appens egna inställningar
    /// </summary>
    public class ThreadRenderOptions
    {
        public bool IsDarkThemed { get; set; }
        public string AccentColor { get; set; }
        public bool ShowAvatars { get; set; }
        public string FontSize { get; set; }
   
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
    }
}

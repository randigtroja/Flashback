using System.Net;
using Flashback.Services.Forum;
using Xunit;

namespace Flashback.Test.Integration.Forum
{    
    public class ForumListTest
    {
        [Fact]
        public void Ska_kunna_bygga_upp_en_korrekt_modell_för_hockeyforumet()
        {
            var forumService = new ForumService(new CookieContainer());

            var forumlist = forumService.GetForums("/f202").Result;

            Assert.True(forumlist.MaxPages > 0);
            Assert.True(forumlist.Title == "Hockey");
            Assert.True(forumlist.Id == "/f202");
            Assert.True(forumlist.Items.Count > 0);
            Assert.True(forumlist.ShowNavigation = true);
        }
    }
}

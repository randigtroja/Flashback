using System;
using System.Diagnostics;
using System.Net;
using Flashback.Model;
using Flashback.Services.Forum;
using Xunit;

namespace Flashback.Test.Integration.Forum
{    
    public class ForumMainListTest
    {
        [Fact]
        public void Ska_kunna_hämta_huvudlistan()
        {
            var forumService = new ForumService(new CookieContainer());

            Stopwatch sw = new Stopwatch();
            sw.Start();
            //for (int i = 0; i < 20; i++)
            //{
                var forum = forumService.GetMainForumlist().Result; // går ej med async i testprojekt?
            //}

            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);

            //var forum = forumService.GetMainForumlist().Result; // går ej med async i testprojekt?

            Assert.True(forum.Items.Count > 5);

            foreach (var fbItem in forum.Items)
            {
                Assert.True(fbItem.Type == FbItemType.Forum);
                Assert.True(!string.IsNullOrWhiteSpace(fbItem.Id));
                Assert.True(!string.IsNullOrWhiteSpace(fbItem.Name));

                Console.WriteLine($"{fbItem.Name} - {fbItem.Id}");
            }

        }
    }
}

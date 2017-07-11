using System;
using System.Net;
using Flashback.Model;
using Flashback.Services.Threads;
using Xunit;

namespace Flashback.Test.Integration.Threads
{
   
    public class AktuellaÄmnenTest
    {
        [Fact]
        public void Ska_kunna_hämta_aktuella_ämnen()
        {
            var threadService = new ThreadsService(new CookieContainer(), null);

            var aktuellaÄmnen = threadService.GetHotTopics().Result;

            Assert.NotNull(aktuellaÄmnen);
            Assert.True(aktuellaÄmnen.Count > 25);

            foreach (var ämne in aktuellaÄmnen)
            {
                Assert.True(ämne.Type == FbItemType.Thread);
                Assert.True(!string.IsNullOrWhiteSpace(ämne.Id));
                Assert.True(!string.IsNullOrWhiteSpace(ämne.Name));

                Console.WriteLine($"{ämne.Name} - {ämne.Id}");
            }
        }
    }
}

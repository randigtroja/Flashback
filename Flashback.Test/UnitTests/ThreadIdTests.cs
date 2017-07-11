using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flashback.Services;
using Xunit;

namespace Flashback.Test.UnitTests
{   
    public class ThreadIdTests
    {
        [Fact]
        public void Ska_kunna_navigera_bakåt()
        {
            string currentId = "t12345p5";

            var newId = currentId.GetCleanIdPreviousPage(5);
            Assert.Equal("t12345p4", newId);
            var newId2 = newId.GetCleanIdPreviousPage(4);
            Assert.Equal("t12345p3", newId2);
        }
    }
}

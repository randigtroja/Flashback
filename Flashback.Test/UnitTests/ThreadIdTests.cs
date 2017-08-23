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

        [Fact]
        public void Ska_kunna_navigera_framåt()
        {
            string currentId = "t12345p5";

            var newId = currentId.GetCleanIdNextPage(5);
            Assert.Equal("t12345p6", newId);            
        }

        [Fact]
        public void Ska_kunna_navigera_till_sista_sidan()
        {
            string currentId = "t12345p5";

            var newId = currentId.GetCleanIdLastPage(50);
            Assert.Equal("t12345p50", newId);            
        }

        [Fact]
        public void Ska_kunna_navigera_till_första_sidan()
        {
            string currentId = "t12345p50";

            var newId = currentId.GetCleanIdFirstPage();
            Assert.Equal("t12345p1", newId);            
        }

        [Fact]
        public void Ska_kunna_räkna_fram_ett_rent_Id()
        {
            string currentId = "t12345p5";
            string currentId2 = "t12345s";

            var newId = currentId.GetCleanId(false);
            var newId2 = currentId2.GetCleanId(false);

            Assert.Equal("t12345", newId);
            Assert.Equal("t12345", newId2);
        }

        [Fact]
        public void Ska_kunna_räkna_fram_ett_rent_Id_samt_ta_bort_typindikatorn()
        {
            string currentId = "t12345p5";
            string currentId2 = "f12p2";

            var newId = currentId.GetCleanId(true);
            var newId2 = currentId2.GetCleanId(true);

            Assert.Equal("12345", newId);
            Assert.Equal("12", newId2);
        }

    }
}

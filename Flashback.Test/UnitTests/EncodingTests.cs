using Flashback.Services;
using Xunit;

namespace Flashback.Test.UnitTests
{
    public class EncodingTests
    {
        [Fact]
        public void Ska_kunna_bygga_en_korrekt_stäng_för_att_posta_som_innehåller_htmltecken()
        {            
            string input = "& <> HEJ \" på dig";

            var output = input.FormatToEncodedPostable();

            Assert.Equal("%26amp%3B+%26lt%3B%26gt%3B+HEJ+%26quot%3B+p%26%23229%3B+dig",output);
            
        }

        [Fact]
        public void Ska_kunna_bygga_en_korrekt_sträng_för_att_posta_som_innehåller_emojis()
        {
            string input = "😎 😀 😍 👶";

            var output = input.FormatToEncodedPostable();

            Assert.Equal("%26%23128526%3B+%26%23128512%3B+%26%23128525%3B+%26%23128118%3B", output);            
        }
    }
}

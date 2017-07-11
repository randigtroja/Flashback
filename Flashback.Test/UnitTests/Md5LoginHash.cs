using System.Net;
using Flashback.Services.Auth;
using Xunit;

namespace Flashback.Test.UnitTests
{    
    public class Md5LoginHash
    {
        [Fact]
        public void Ska_kunna_beräkna_korrekt_md5hash()
        {
            string pass = "KalleKula123åäö";

            var service = new AuthService(new CookieContainer());
            var result = service.BuildMd5HashForLogin(pass);

            Assert.Equal("a7683df724f528ae85ea63ce8f8fd5b8", result);
        }
    }
}

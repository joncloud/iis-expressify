using System.Threading.Tasks;
using Xunit;

namespace IISExpressify.Tests
{
    public class HttpTests
    {
        static HttpIisExpressOptions CreateHttpOptions() =>
            IisExpress.Http().PhysicalPath(Hosting.GetHostingDirectory()).Port(8080);

        [Fact]
        public async Task Http_ShouldDownloadTestFile()
        {
            var iisExpress = CreateHttpOptions().Start();
            var http = iisExpress.CreateHttpClient();

            var contents = await http.GetStringAsync("/Test.txt");
            Assert.Equal("Lorem ipsum", contents);
        }
    }
}

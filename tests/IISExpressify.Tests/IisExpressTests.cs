using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace IISExpressify.Tests
{
    public class IisExpressTests
    {
        static string GetHostingDirectory() =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [Fact]
        public async Task Http_ShouldDownloadTestFile()
        {
            using (var iisExpress = IisExpress.Http().PhysicalPath(GetHostingDirectory()).Port(8080).Start())
            using (var http = new HttpClient() { BaseAddress = iisExpress.BaseUri })
            {
                var contents = await http.GetStringAsync("/Test.txt");
                Assert.Equal("Lorem ipsum", contents);
            }
        }

        [Fact]
        public async Task Https_ShouldDownloadTestFile()
        {
            using (var iisExpress = IisExpress.Https().PhysicalPath(GetHostingDirectory()).Port(44300).Start())
            using (var http = new HttpClient() { BaseAddress = iisExpress.BaseUri })
            {
                var contents = await http.GetStringAsync("/Test.txt");
                Assert.Equal("Lorem ipsum", contents);
            }
        }
    }
}

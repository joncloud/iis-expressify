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
            using (var http = iisExpress.CreateHttpClient())
            {
                var contents = await http.GetStringAsync("/Test.txt");
                Assert.Equal("Lorem ipsum", contents);
            }
        }

        static HttpsIisExpressOptions CreateHttpsOptions() =>
            IisExpress.Https().PhysicalPath(GetHostingDirectory()).Port(44300);

        [Fact]
        public async Task Https_ShouldDownloadTestFile()
        {
            using (var iisExpress = CreateHttpsOptions().Start())
            using (var http = iisExpress.CreateHttpClient())
            {
                var contents = await http.GetStringAsync("/Test.txt");
                Assert.Equal("Lorem ipsum", contents);
            }
        }

        [Fact]
        public async Task Https_ShouldRenderAsp_GivenClassic20Pipeline()
        {
            using (var iisExpress = CreateHttpsOptions().SetPipelineClassic20().Start())
            using (var http = iisExpress.CreateHttpClient())
            {
                var contents = await http.GetStringAsync("/Classic.asp");
                Assert.Equal("1/10/2019", contents);
            }
        }

        [Fact]
        public async Task Https_ShouldRenderAsp_GivenClassic40Pipeline()
        {
            using (var iisExpress = CreateHttpsOptions().SetPipelineClassic40().Start())
            using (var http = iisExpress.CreateHttpClient())
            {
                var contents = await http.GetStringAsync("/Classic.asp");
                Assert.Equal("1/10/2019", contents);
            }
        }

        [Fact]
        public async Task Https_ShouldRenderAspx_GivenClassic20Pipeline()
        {
            using (var iisExpress = CreateHttpsOptions().SetPipelineClassic20().Start())
            using (var http = iisExpress.CreateHttpClient())
            {
                var contents = await http.GetStringAsync("/WebForm.aspx");
                Assert.Equal("2019-01-10", contents);
            }
        }

        [Fact]
        public async Task Https_ShouldRenderAspx_GivenClassic40Pipeline()
        {
            using (var iisExpress = CreateHttpsOptions().SetPipelineClassic40().Start())
            using (var http = iisExpress.CreateHttpClient())
            {
                var contents = await http.GetStringAsync("/WebForm.aspx");
                Assert.Equal("2019-01-10", contents);
            }
        }
    }
}

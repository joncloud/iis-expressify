using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace IISExpressify.Tests
{
    public class ReadmeTests : IClassFixture<CsprojFixture>, IClassFixture<ReadmeFixture>
    {
        readonly CsprojFixture _httpsCsprojFixture;
        readonly ReadmeFixture _readmeFixture;
        public ReadmeTests(CsprojFixture httpsCsprojFixture, ReadmeFixture readmeFixture)
        {
            _httpsCsprojFixture = httpsCsprojFixture;
            _readmeFixture = readmeFixture;
        }

        [Fact]
        public void Installation_ShouldListSameVersionAsCsproj()
        {
            var versionPrefixElement = _httpsCsprojFixture.Document
                .Root
                .Elements("PropertyGroup")
                .Elements("VersionPrefix")
                .FirstOrDefault();

            Assert.NotNull(versionPrefixElement);

            var expected = versionPrefixElement.Value;
            var actual = _readmeFixture.Readme.InstallationVersion;

            Assert.Equal(expected, actual);
        }
    }
}

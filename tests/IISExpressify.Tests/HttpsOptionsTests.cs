using System;
using System.IO;
using Xunit;

namespace IISExpressify.Tests
{
    public class HttpsOptionsTests
    {
        [Fact]
        public void Https_ShouldThrowGivenNotFoundPhysicalPath()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
            Assert.Throws<DirectoryNotFoundException>(() => IisExpress.Https().PhysicalPath(path));
        }

        [InlineData(44299)]
        [InlineData(44399)]
        [Theory]
        public void Https_ShouldThrowGivenOutOfRangeHttpsPort(ushort port)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => IisExpress.Https().Port(port));
        }

        [Fact]
        public void Start_ShouldThrowGivenUnsetPhysicalPath()
        {
            Assert.Throws<InvalidOperationException>(() => IisExpress.Https().Start());
        }

        [Fact]
        public void Start_ShouldThrowGivenUnsetPort()
        {
            Assert.Throws<InvalidOperationException>(() => IisExpress.Https().PhysicalPath(Path.GetTempPath()).Start());
        }
    }
}

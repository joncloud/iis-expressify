using System;
using System.IO;
using Xunit;

namespace IISExpressify.Tests
{
    public class HttpOptionsTests
    {
        [Fact]
        public void Http_ShouldThrowGivenNotFoundPhysicalPath()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
            Assert.Throws<DirectoryNotFoundException>(() => IisExpress.Http().PhysicalPath(path));
        }

        [Fact]
        public void Http_ShouldThrowGivenOutOfRangeHttpPort()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => IisExpress.Http().Port(0));
        }

        [Fact]
        public void Start_ShouldThrowGivenUnsetPhysicalPath()
        {
            Assert.Throws<InvalidOperationException>(() => IisExpress.Http().Start());
        }

        [Fact]
        public void Start_ShouldThrowGivenUnsetPort()
        {
            Assert.Throws<InvalidOperationException>(() => IisExpress.Http().PhysicalPath(Path.GetTempPath()).Start());
        }
    }
}

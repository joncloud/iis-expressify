using System;
using System.IO;

namespace IISExpressify
{
    public class HttpIisExpressOptions
    {
        readonly string _physicalPath;
        readonly ushort _port;
        readonly bool _systray;
        readonly StartIisExpress _start;

        internal HttpIisExpressOptions(StartIisExpress start)
        {
            _physicalPath = null;
            _port = 0;
            _systray = false;
            _start = start;
        }

        HttpIisExpressOptions(StartIisExpress start, string physicalPath, ushort port, bool systray)
        {
            _start = start;
            _physicalPath = physicalPath;
            _port = port;
            _systray = systray;
        }

        public HttpIisExpressOptions PhysicalPath(string path) =>
            new HttpIisExpressOptions(_start, ValidatePath(path), _port, _systray);

        static string ValidatePath(string path)
        {
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException(path);
            return path;
        }

        public HttpIisExpressOptions Port(ushort port) =>
            new HttpIisExpressOptions(_start, _physicalPath, ValidatePort(port), _systray);

        static ushort ValidatePort(ushort port)
        {
            if (port == 0) throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1-65535");
            return port;
        }

        public HttpIisExpressOptions HideSystray() =>
            new HttpIisExpressOptions(_start, _physicalPath, _port, systray: false);

        public HttpIisExpressOptions ShowSystray() =>
            new HttpIisExpressOptions(_start, _physicalPath, _port, systray: true);

        string EnsurePhysicalPathSet()
        {
            if (_physicalPath == null) throw new InvalidOperationException("PhysicalPath must be set");
            return _physicalPath;
        }

        ushort EnsurePortSet()
        {
            if (_port == 0) throw new InvalidOperationException("Port must be set");
            return _port;
        }

        string GetProcessArguments() =>
            $@"""/path:{EnsurePhysicalPathSet()}"" /port:{EnsurePortSet()} /systray:{_systray.ToString().ToLower()}";

        public IisExpress Start() => _start(Uri.UriSchemeHttp, _port, GetProcessArguments());
    }
}

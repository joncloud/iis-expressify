using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace IISExpressify
{
    public class HttpsIisExpressOptions
    {
        readonly string _physicalPath;
        readonly ushort _port;
        readonly bool _systray;
        readonly ApplicationPoolPipelineMode _pipelineMode;
        readonly StartIisExpress _start;

        internal HttpsIisExpressOptions(StartIisExpress start)
        {
            _physicalPath = null;
            _port = 0;
            _systray = false;
            _start = start;
            _pipelineMode = ApplicationPoolPipelineMode.Default;
        }

        HttpsIisExpressOptions(StartIisExpress start, string physicalPath, ushort port, ApplicationPoolPipelineMode pipelineMode, bool systray)
        {
            _physicalPath = physicalPath;
            _port = port;
            _systray = systray;
            _start = start;
            _pipelineMode = pipelineMode;
        }

        public HttpsIisExpressOptions PhysicalPath(string path) =>
            new HttpsIisExpressOptions(_start, ValidatePath(path), _port, _pipelineMode, _systray);

        static string ValidatePath(string path)
        {
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException(path);
            return path;
        }

        public HttpsIisExpressOptions Port(ushort port) =>
            new HttpsIisExpressOptions(_start, _physicalPath, ValidatePort(port), _pipelineMode, _systray);

        static ushort ValidatePort(ushort port)
        {
            // https://stackoverflow.com/a/24957146/4027768
            if (port < 44300 || port > 44398) throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 44300-44398");
            return port;
        }

        public HttpsIisExpressOptions HideSystray() =>
            new HttpsIisExpressOptions(_start, _physicalPath, _port, _pipelineMode, systray: false);

        public HttpsIisExpressOptions ShowSystray() =>
            new HttpsIisExpressOptions(_start, _physicalPath, _port, _pipelineMode, systray: true);

        public HttpsIisExpressOptions SetPipelineClassic20() =>
            new HttpsIisExpressOptions(_start, _physicalPath, _port, ApplicationPoolPipelineMode.Classic20, _systray);

        public HttpsIisExpressOptions SetPipelineClassic40() =>
            new HttpsIisExpressOptions(_start, _physicalPath, _port, ApplicationPoolPipelineMode.Classic40, _systray);

        public HttpsIisExpressOptions SetPipelineIntegrated20() =>
            new HttpsIisExpressOptions(_start, _physicalPath, _port, ApplicationPoolPipelineMode.Integrated20, _systray);

        public HttpsIisExpressOptions SetPipelineIntegrated40() =>
            new HttpsIisExpressOptions(_start, _physicalPath, _port, ApplicationPoolPipelineMode.Integrated40, _systray);

        public HttpsIisExpressOptions SetPipelineDefault() =>
            new HttpsIisExpressOptions(_start, _physicalPath, _port, ApplicationPoolPipelineMode.Default, _systray);

        public HttpsIisExpressOptions SetPipelineUnmanaged() =>
            new HttpsIisExpressOptions(_start, _physicalPath, _port, ApplicationPoolPipelineMode.Unmanaged, _systray);

        static string GetTemplateConfig() =>
            Path.Combine(
                Environment.Is64BitOperatingSystem
                    ? Environment.GetEnvironmentVariable("PROGRAMFILES(X86)")
                    : Environment.GetEnvironmentVariable("PROGRAMFILES"),
                "IIS Express",
                "AppServer",
                "applicationhost.config"
            );

        ushort EnsurePortSet()
        {
            if (_port == 0) throw new InvalidOperationException("Port must be set");
            return _port;
        }

        void ReplaceBinding(XDocument document)
        {
            var bindings = document.Descendants("binding");

            var root = bindings.First();
            root.Attribute("protocol").Value = Uri.UriSchemeHttps;
            root.Attribute("bindingInformation").Value = $":{EnsurePortSet()}:localhost";

            foreach (var ignore in bindings.Skip(1))
            {
                ignore.Remove();
            }
        }

        string EnsurePhysicalPathSet()
        {
            if (_physicalPath == null) throw new InvalidOperationException("PhysicalPath must be set");
            return _physicalPath;
        }

        void ReplaceVirtualDirectory(XDocument document)
        {
            var virtualDirectories = document.Descendants("virtualDirectory");

            var root = virtualDirectories.First();
            root.Attribute("path").Value = "/";
            root.Attribute("physicalPath").Value = EnsurePhysicalPathSet();

            foreach (var ignore in virtualDirectories.Skip(1))
            {
                ignore.Remove();
            }
        }

        void ReplaceApplicationPool(XDocument document)
        {
            var applicationPoolName = GetPredefinedApplicationPoolName(_pipelineMode);

            var applicationPoolExists = document.Descendants("applicationPools")
                .Descendants("add")
                .Select(add => add.Attribute("name").Value)
                .Contains(applicationPoolName);
            if (!applicationPoolExists)
            {
                var message = $"Application Pool {applicationPoolName} is not available in applicationhost.config";
                throw new InvalidOperationException(message);
            }

            var applicationDefaults = document.Descendants("applicationDefaults");

            var root = applicationDefaults.First();
            root.Attribute("applicationPool").Value = applicationPoolName;
        }

        static string GetPredefinedApplicationPoolName(ApplicationPoolPipelineMode pipelineMode)
        {
            switch (pipelineMode)
            {
                case ApplicationPoolPipelineMode.Classic20:
                    return "Clr2ClassicAppPool";
                case ApplicationPoolPipelineMode.Classic40:
                    return "Clr4ClassicAppPool";
                case ApplicationPoolPipelineMode.Integrated20:
                    return "Clr2IntegratedAppPool";
                case ApplicationPoolPipelineMode.Integrated40:
                    return "Clr4IntegratedAppPool";
                case ApplicationPoolPipelineMode.Unmanaged:
                    return "UnmanagedClassicAppPool";
                default:
                    return "IISExpressAppPool";
            }
        }

        string CreateConfigFile()
        {
            string templateFile = GetTemplateConfig();
            string configFile = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString()
            );

            File.Copy(templateFile, configFile);

            var document = XDocument.Load(configFile);
            ReplaceBinding(document);
            ReplaceVirtualDirectory(document);
            ReplaceApplicationPool(document);
            document.Save(configFile);

            return configFile;
        }

        string GetProcessArguments() =>
            $@"""/config:{CreateConfigFile()}"" /siteid:1 /systray:{_systray.ToString().ToLower()}";

        public IisExpress Start() => _start(Uri.UriSchemeHttps, _port, GetProcessArguments());
    }
}

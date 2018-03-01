namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.Remote;
    using System;
    using System.IO;
    using System.Linq;

    public class FirefoxWebDriverConfig : WebDriverManagerConfig
    {
        public string ProfileDirectoryPath { get; set; } = null;

        public bool FromDefaultProfile { get; set; } = false;

        public FirefoxWebDriverConfig()
        {
            DriverType = WebDriverType.Firefox;
        }

        public override IWebDriver CreateLocalDriver()
        {

            FirefoxDriverService driverService;
            string path = Environment.GetEnvironmentVariable("webdriver.gecko.driver", EnvironmentVariableTarget.Machine);
            if (path != null)
                driverService = FirefoxDriverService.CreateDefaultService(path);
            else
                driverService = FirefoxDriverService.CreateDefaultService();

            driverService.HideCommandPromptWindow = true;

            var ops = new FirefoxOptions();
            ops.SetPreference("security.enterprise_roots.enabled", true);
          
            ops.Profile = CreateProfile();
            var driver = new FirefoxDriver(driverService, ops, CommandTimeout);
            ProcessID = driverService.ProcessId;
            return driver;
        }
        public override IWebDriver CreateRemoteDriver()
        {
            var ops = new FirefoxOptions();
            ops.SetPreference("security.enterprise_roots.enabled", true);
            ops.Profile = CreateProfile();
            return new RemoteWebDriver(new Uri(GridUri), ops.ToCapabilities(), CommandTimeout);
        }

        private FirefoxProfile CreateProfile()
        {
            FirefoxProfile profile = null;

            //if (ProfileDirectoryPath != null)
            //    profile = new FirefoxProfile(ProfileDirectoryPath);
            //else if (FromDefaultProfile)
            //{
            //    var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //    var path = Path.Combine(appData, @"Mozilla\Firefox\Profiles");
            //    var dir = new DirectoryInfo(path);
            //    var folder = dir.GetDirectories("*.default").First();

            //    profile = new FirefoxProfile(folder.FullName);
            //}
            //else
            profile = new FirefoxProfile
            {
                EnableNativeEvents = true,
                DeleteAfterUse = true,
                AcceptUntrustedCertificates = true
            };

            var proxy = GetProxy();
            if (proxy != null)
                profile.SetProxyPreferences(proxy);

            return profile;
        }
    }
}
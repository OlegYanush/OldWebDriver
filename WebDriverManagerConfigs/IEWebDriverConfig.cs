namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.Remote;
    using System;

    public class IEWebDriverConfig : WebDriverManagerConfig
    {
        public string IEDriverServer { get; set; } = null;

        public override string Version { get; set; } = "11";

        public IEWebDriverConfig()
        {
            DriverType = WebDriverType.IE;
        }

        public override IWebDriver CreateLocalDriver()
        {
            InternetExplorerDriverService service;
            if (IEDriverServer != null)
                service = InternetExplorerDriverService.CreateDefaultService(IEDriverServer);
            else
            {
                string path = Environment.GetEnvironmentVariable("webdriver.ie.driver", EnvironmentVariableTarget.Machine);
                if (path != null)
                    service = InternetExplorerDriverService.CreateDefaultService(path);
                else
                    service = InternetExplorerDriverService.CreateDefaultService();
            }

            InternetExplorerOptions options = GetOptions();

            var driver = new InternetExplorerDriver(service, options, CommandTimeout);
            ProcessID = service.ProcessId;
            return driver;
        }
        public override IWebDriver CreateRemoteDriver()
        {
            var options = GetOptions();
            var capabilities = options.ToCapabilities() as DesiredCapabilities;
            SetCapabilities(capabilities);
            return new RemoteWebDriver(new Uri(GridUri), capabilities, CommandTimeout);
        }

        public InternetExplorerOptions GetOptions()
        {
            InternetExplorerOptions options = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                InitialBrowserUrl = "about:blank",
                EnableNativeEvents = true,
                EnsureCleanSession = true,
                EnablePersistentHover = false,
                PageLoadStrategy = InternetExplorerPageLoadStrategy.Normal
            };

            switch (Version)
            {
                case "9":
                case "10":
                    options.UsePerProcessProxy = false;
                    break;
                case "11":
                    options.UsePerProcessProxy = true;
                    break;
            }

            options.Proxy = GetProxy();
            return options;
        }
    }
}

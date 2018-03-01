namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;
    using System;

    public class ChromeMobileEmulationConfig 
    {
        public bool EnableTouchEvents { get; set; } = false;

        public long Height { get; set; } = 0;

        public long Width { get; set; } = 0;

        public double PixelRatio { get; set; }

        public string UserAgent { get; set; } = null;
    }

    public class ChromeWebDriverConfig : WebDriverManagerConfig
    {
        public ChromeMobileEmulationConfig ChromeMobileEmulationConfig { get; set; } = null;

        public ChromeWebDriverConfig()
        {
            DriverType = WebDriverType.Chrome;
        }

        public override IWebDriver CreateLocalDriver()
        {
            ChromeDriverService driverService;
            string path = Environment.GetEnvironmentVariable("webdriver.chrome.driver", EnvironmentVariableTarget.Machine);
            if (path != null)
                driverService = ChromeDriverService.CreateDefaultService(path);
            else
                driverService = ChromeDriverService.CreateDefaultService();

            driverService.EnableVerboseLogging = true;
            driverService.HideCommandPromptWindow = true;

            ChromeOptions options = GetOptions();

            var driver = new ChromeDriver(driverService, options, CommandTimeout);
            ProcessID = driverService.ProcessId;
            return driver;
        }
        public override IWebDriver CreateRemoteDriver()
        {
            ChromeOptions options = GetOptions();
            var capabilities = options.ToCapabilities() as DesiredCapabilities;
            SetCapabilities(capabilities);
            return new RemoteWebDriver(new Uri(GridUri), capabilities, CommandTimeout);
        }

        public ChromeOptions GetOptions()
        {
            ChromeOptions options = new ChromeOptions();
            if (ChromeMobileEmulationConfig != null)
            {
                ChromeMobileEmulationDeviceSettings emulation = new ChromeMobileEmulationDeviceSettings()
                {
                    EnableTouchEvents = ChromeMobileEmulationConfig.EnableTouchEvents,
                    Height = ChromeMobileEmulationConfig.Height,
                    Width = ChromeMobileEmulationConfig.Width,
                    PixelRatio = ChromeMobileEmulationConfig.PixelRatio,
                    UserAgent = ChromeMobileEmulationConfig.UserAgent
                };
                options.EnableMobileEmulation(emulation);
            }
            options.AddUserProfilePreference("download.prompt_for_download", true);
            options.AddUserProfilePreference("download.default_directory", "NULL");
            options.AddArgument("disable-infobars");
            options.AddArgument("disable-blink-features=BlockCredentialedSubresources");

            options.Proxy = GetProxy();
            return options;
        }
    }
}

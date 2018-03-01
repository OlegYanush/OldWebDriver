namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using System;
    using OpenQA.Selenium;
    using System.Collections.Generic;
    using OpenQA.Selenium.Remote;

    public abstract class WebDriverManagerConfig
    {
        public int ProcessID { get; set; } = 0;

        public WebDriverType DriverType { get; set; }

        public int Height { get; set; } = -1;

        public int Width { get; set; } = -1;

        public virtual string Version { get; set; } = "any";

        public bool IsJavaScriptEnabled { get; set; } = true;

        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(65);

        public TimeSpan SearchTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public TimeSpan PageLoadTimeout { get; set; } = TimeSpan.FromSeconds(120);

        public TimeSpan JavaScriptTimeout { get; set; } = TimeSpan.FromSeconds(60);

        public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(60);

        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(2);

        public bool MakeScreenshotOnFail { get; set; } = true;

        public bool IsGrid { get; set; } = false;

        public string GridUri { get; set; } = null;

        public bool IsHighlight { get; set; } = false;

        public List<CapabilityProperty> CapabilityProperties { get; set; } = new List<CapabilityProperty>();

        public string Proxy { get; set; } = null;

        public string ProxyAutoConfigUrl { get; set; } = null;

        public bool CreateInstancePerThread { get; set; } = false;

        public IWebDriver CreateDriver()
        {
            IWebDriver driver = IsGrid
                ? CreateRemoteDriver()
                : CreateLocalDriver();

            if (Width == -1 && Height == -1)
            {
                if (DriverType == WebDriverType.Firefox)
                {
                    driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
                    driver.Manage().Window.Position = new System.Drawing.Point(0, 0);
                }
                else
                    driver.Manage().Window.Maximize();
            }
            else if (Width > 0 && Height > 0)
            {
                driver.Manage().Window.Size = new System.Drawing.Size(Width, Height);
                driver.Manage().Window.Position = new System.Drawing.Point(0, 0);
            }

            driver.Manage().Timeouts().ImplicitWait = SearchTimeout;
            driver.Manage().Timeouts().PageLoad = PageLoadTimeout;

            if (IsJavaScriptEnabled)
                driver.Manage().Timeouts().AsynchronousJavaScript = JavaScriptTimeout;

            return driver;
        }

        public abstract IWebDriver CreateRemoteDriver();
        public abstract IWebDriver CreateLocalDriver();


        public void SetCapabilities(DesiredCapabilities capabilities)
        {
            foreach (var capability in CapabilityProperties)
            {
                capabilities.SetCapability(capability.Name, capability.Value);
            }
        }

        public Proxy GetProxy()
        {
            if (Proxy != null || ProxyAutoConfigUrl != null)
            {
                var proxy = new Proxy();
                proxy.AddBypassAddresses("localhost", "127.0.0.1");

                if (ProxyAutoConfigUrl != null)
                {
                    proxy.Kind = ProxyKind.ProxyAutoConfigure;
                    proxy.ProxyAutoConfigUrl = ProxyAutoConfigUrl;
                }
                if (Proxy != null)
                {
                    proxy.Kind = ProxyKind.Manual;
                    proxy.HttpProxy = Proxy;
                    proxy.SslProxy = Proxy;
                }
                return proxy;
            }
            return null;
        }

        public override string ToString()
        {
            if (Width == -1 && Height == -1)
                return $"{DriverType}";
            else return $"{DriverType} {Width}x{Height}";
        }
    }
}

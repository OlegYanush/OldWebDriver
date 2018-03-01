namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using QA.AutomatedMagic.LogMagic;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using WebElements;

    public partial class WebDriverManager : IWebDriverManager
    {
        private IWebDriver _driver { get; set; } = null;
        private IJavaScriptExecutor _jsExecutor { get; set; } = null;
        private WebDriverWait _wait { get; set; } = null;

        public WebDriverManagerConfig Config { get; set; }
        public IWebDriver Driver
        {
            get
            {
                if (_driver == null)
                {
                    _driver = Config.CreateDriver();
                }
                return _driver;
            }
        }
        public IJavaScriptExecutor JavaScriptExecutor
        {
            get
            {
                if (_jsExecutor == null)
                {
                    _jsExecutor = Driver as IJavaScriptExecutor;
                }
                return _jsExecutor;
            }
        }
        public WebDriverWait Wait
        {
            get
            {
                if (_wait == null)
                {
                    _wait = new WebDriverWait(Driver, Config.WaitTimeout);
                    _wait.PollingInterval = Config.PollingInterval;
                }
                return _wait;
            }
        }



        private FrameWebElement _currentFrame = null;
        public IWebElement FindElement(WebElement element, ILogger log)
        {
            var commandMessage = $"Find element: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(FindElement), commandMessage);

            try
            {
                var locator = element.GetLocator();
                if (locator == null) return null;

                IWebElement parentElement = null;
                var optimizedParent = element.GetOptimizedParent();

                var mbFrame = optimizedParent as FrameWebElement;
                if (mbFrame != null)
                {
                    if (_currentFrame != mbFrame)
                        SwitchToFrame(mbFrame, log);
                }
                else
                {
                    if (locator.IsRelative && optimizedParent != null)
                        parentElement = FindElement(optimizedParent, log);
                    else
                    {
                        for (var cur = element.ParentElement; cur != null; cur = cur.ParentElement)
                        {
                            mbFrame = cur as FrameWebElement;
                            if (mbFrame != null)
                                break;
                        }
                        if (mbFrame != null)
                        {
                            if (_currentFrame != mbFrame)
                                SwitchToFrame(mbFrame, log);
                        }
                        else
                            SwitchToDefaultContent(log);
                    }
                }


                IWebElement searchedElement = null;
                if (parentElement == null)
                    searchedElement = Driver.FindElement(locator.Get());
                else
                    searchedElement = parentElement.FindElement(locator.Get());

                return searchedElement;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                if (log != null)
                    TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }
        public IWebElement FindElement(By locator, ILogger log)
        {
            var commandMessage = $"Find element with locator {locator}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(FindElement), commandMessage);

            try
            {
                return Driver.FindElement(locator);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                if (log != null)
                    TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }
        public List<IWebElement> FindElements(WebElement element, ILogger log)
        {
            var commandMessage = $"Find elements: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(FindElement), commandMessage);

            try
            {
                var locator = element.GetLocator();
                if (locator == null) return null;

                List<IWebElement> parentElements = null;
                var optimizedParent = element.GetOptimizedParent();

                var mbFrame = optimizedParent as FrameWebElement;
                if (mbFrame != null)
                {
                    if (_currentFrame != mbFrame)
                        SwitchToFrame(mbFrame, log);
                }
                else
                {
                    if (locator.IsRelative && optimizedParent != null)
                        parentElements = FindElements(optimizedParent, log);
                    else
                    {
                        for (var cur = element.ParentElement; cur != null; cur = cur.ParentElement)
                        {
                            mbFrame = cur as FrameWebElement;
                            if (mbFrame != null)
                                break;
                        }
                        if (mbFrame != null)
                        {
                            if (_currentFrame != mbFrame)
                                SwitchToFrame(mbFrame, log);
                        }
                        else
                            SwitchToDefaultContent(log);
                    }
                }


                List<IWebElement> searchedElements = null;
                if (parentElements == null)
                    searchedElements = Driver.FindElements(locator.Get()).ToList();
                else
                {
                    searchedElements = new List<IWebElement>();
                    foreach (var parentElement in parentElements)
                    {
                        var batch = parentElement.FindElements(locator.Get());
                        searchedElements.AddRange(batch);
                    }
                }

                return searchedElements;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                if (log != null)
                    TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }
        public List<IWebElement> FindElements(By locator, ILogger log)
        {
            var commandMessage = $"Find elements with locator {locator}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(FindElement), commandMessage);

            try
            {
                List<IWebElement> searchedElements = Driver.FindElements(locator).ToList();
                return searchedElements;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                if (log != null)
                    TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }

        public IWebElement FindElementInParent(IWebElement parentElement, WebElement element, ILogger log)
        {
            var commandMessage = $"Find element: {element.Description} in parent";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(FindElementInParent), commandMessage);

            try
            {
                IWebElement searchedElement = null;

                var locator = element.Locator;

                if (!locator.IsRelative)
                    searchedElement = Driver.FindElement(locator.Get());
                else
                    searchedElement = parentElement.FindElement(locator.Get());

                return searchedElement;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public IWebElement FindElementInParent(IWebElement parentElement, By locator, ILogger log)
        {
            var commandMessage = $"Find element with locator {locator} in parent";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(FindElementInParent), commandMessage);

            try
            {
                IWebElement searchedElement = parentElement.FindElement(locator);
                return searchedElement;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public List<IWebElement> FindElementsInParent(IWebElement parentElement, WebElement element, ILogger log)
        {
            var commandMessage = $"Find elements in parent: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(FindElement), commandMessage);

            try
            {
                List<IWebElement> searchedElements = null;
                var locator = element.Locator;
                if (!locator.IsRelative)
                {
                    searchedElements = Driver.FindElements(locator.Get()).ToList();
                }
                else
                {
                    searchedElements = parentElement.FindElements(locator.Get()).ToList();
                }

                return searchedElements;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public List<IWebElement> FindElementsInParent(IWebElement parentElement, By locator, ILogger log)
        {
            var commandMessage = $"Find elements in parent with locator {locator}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(FindElement), commandMessage);

            try
            {
                List<IWebElement> searchedElements = parentElement.FindElements(locator).ToList();
                return searchedElements;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public IWebElement GetParentElement(IWebElement element, ILogger log)
        {
            var commandMessage = $"Get parent for element: {element}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetParentElement), commandMessage);

            try
            {
                var parent = element.FindElement(By.XPath("./.."));
                return parent;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public bool IsElementPresent(WebElement element, ILogger log, int timeoutInSec = 5)
        {
            var commandMessage = $"Is element present: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(IsElementPresent), commandMessage);

            try
            {
                var el = TryFindElementWithTimeFrame(element, timeoutInSec);
                log?.INFO($"Element {(el == null ? "is not" : "is")} present");
                return el != null;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public bool IsElementPresentInParent(IWebElement parentElement, WebElement element, ILogger log, int timeoutInSec = 5)
        {
            var commandMessage = $"Is element present in parent: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(IsElementPresent), commandMessage);

            try
            {
                var el = TryFindElementInParentWithTimeFrame(parentElement, element.Locator.Get(), timeoutInSec);
                log?.INFO($"Element {(el == null ? "is not" : "is")} present");
                return el != null;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public bool IsElementVisible(WebElement element, ILogger log, int timeoutInSec = 5)
        {
            var commandMessage = $"Is element visible: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(IsElementVisible), commandMessage);

            try
            {
                var el = TryFindElementWithTimeFrame(element, timeoutInSec);
                if (el == null) throw new Exception("Element is not visible");

                log?.INFO($"Element {(el.Displayed ? "is" : "is not")} visible");
                return el.Displayed;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public bool IsElementEnabled(WebElement element, ILogger log, int timeoutInSec = 5)
        {
            var commandMessage = $"Is element enabled: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(IsElementEnabled), commandMessage);

            try
            {
                var el = TryFindElementWithTimeFrame(element, timeoutInSec);
                if (el == null) throw new Exception("Element is not enabled");

                log?.INFO($"Element {(el.Enabled ? "is" : "is not")} enabled");
                return el.Enabled;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public WebElementState GetElementState(WebElement element, ILogger log, int timeoutInSec = 5)
        {
            var commandMessage = $"Get state for element: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetElementState), commandMessage);

            try
            {
                var state = WebElementState.None;
                var el = TryFindElementWithTimeFrame(element, timeoutInSec);
                if (el == null)
                {
                    state = WebElementState.NotPresent;
                }
                else
                {
                    state = WebElementState.Present;
                    if (el.Displayed) state |= WebElementState.Visible;
                    if (el.Enabled) state |= WebElementState.Enabled;
                }
                log?.INFO($"Element state is: {state}");
                return state;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }


        public Size GetScreenSize(ILogger log)
        {
            var commandMessage = "Get screen size";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetScreenSize), commandMessage);

            try
            {
                var height = int.Parse(JSExecutor("return screen.height", log).ToString());
                var width = int.Parse(JSExecutor("return screen.width", log).ToString());
                var res = new Size(width, height);
                return res;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void SetProxy(string proxyString, ILogger log)
        {
            Config.Proxy = proxyString;
        }

        public void Click(WebElement element, ILogger log)
        {
            var commandMessage = $"Click on element: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(Click), commandMessage);

            try
            {
                var el = WaitForElementState(element, WebElementState.ReadyForAction, log);
                el.Click();
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void SendKeys(WebElement element, string value, ILogger log)
        {
            var commandMessage = $"Send text '{value}' to element: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SendKeys), commandMessage);

            try
            {
                var el = WaitForElementState(element, WebElementState.ReadyForAction, log);

                el.Clear();
                el.SendKeys(value);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void SendChars(WebElement element, string value, ILogger log)
        {
            var commandMessage = $"Send text '{value}' by chars to element: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SendChars), commandMessage);

            try
            {
                log?.DEBUG($"Element {element}");

                var el = WaitForElementState(element, WebElementState.ReadyForAction, log);

                el.Click();
                el.Clear();
                Thread.Sleep(500);
                el.SendKeys(Keys.Delete);

                for (int i = 0; i < value.Length; i++)
                {
                    el.SendKeys(value[i].ToString());
                    el.SendKeys(Keys.Delete);
                }

                Thread.Sleep(500);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }

        public string GetValue(WebElement element, ILogger log)
        {
            var commandMessage = $"Get text for element: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetValue), commandMessage);
            try
            {
                var el = FindElement(element, log);
                string text = el.Text.Trim();
                log?.INFO($"Text: '{text}'");
                return text;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public string GetInnerHtml(WebElement element, ILogger log)
        {
            var commandMessage = $"Get text for element: {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetInnerHtml), commandMessage);
            try
            {
                var el = FindElement(element, log);
                string text = el.GetAttribute("innerHTML");
                log?.INFO($"Text: '{text}'");
                return text;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public string GetInnerHtml(IWebElement element, ILogger log)
        {
            var commandMessage = $"Get text for element: {element}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetInnerHtml), commandMessage);
            try
            {
                string text = element.GetAttribute("innerHTML");
                log?.INFO($"Text: '{text}'");
                return text;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public string GetAttribute(WebElement element, string attribute, ILogger log)
        {
            var commandMessage = $"Get value from attribute '{attribute}' for element {element.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetAttribute), commandMessage);
            try
            {
                var el = FindElement(element, log);
                string value = el.GetAttribute(attribute);

                log?.INFO($"Attribute value '{attribute}' = '{value}'");
                return value;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public void SwitchToDefaultContent(ILogger log, bool justFrameRefresh = false)
        {
            try
            {
                if (justFrameRefresh)
                    _currentFrame = null;
                if (_currentFrame != null)
                {
                    Driver.SwitchTo().DefaultContent();
                    _currentFrame = null;
                }
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during switching to default content", ex);
                throw;
            }
        }
        public void SwitchToFrame(FrameWebElement frameWebElement, ILogger log)
        {
            var commandMessage = $"Switch to frame: {frameWebElement.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SwitchToFrame), commandMessage);

            try
            {
                if (_currentFrame != frameWebElement)
                {
                    var needToDefault = true;
                    var frameChain = new Stack<FrameWebElement>();
                    for (WebElement cur = frameWebElement.ParentElement; cur != null; cur = cur.ParentElement)
                    {
                        var mbCurFrame = cur as FrameWebElement;
                        if (mbCurFrame != null)
                        {
                            if (mbCurFrame == _currentFrame)
                            {
                                needToDefault = false;
                                break;
                            }
                            frameChain.Push(mbCurFrame);
                        }
                    }

                    if (needToDefault && _currentFrame != null)
                        SwitchToDefaultContent(log);

                    while (frameChain.Count > 0)
                    {
                        var curFrame = frameChain.Pop();
                        SwitchToFrame(curFrame, log);
                    }

                    switch (frameWebElement.FrameType)
                    {
                        case FrameWebElement.FrameLocatorType.Id:
                            SwitchToFrameById(frameWebElement.FrameValue, log, frameWebElement);
                            break;
                        case FrameWebElement.FrameLocatorType.Index:
                            var index = int.Parse(frameWebElement.FrameValue);
                            SwitchToFrameByIndex(index, log, frameWebElement);
                            break;
                        case FrameWebElement.FrameLocatorType.Locator:
                            SwitchToFrameByLocator(frameWebElement, log);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void SwitchToFrameByIndex(int index, ILogger log, FrameWebElement frame = null)
        {
            var commandMessage = $"Switch to frame by index: '{index}'";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SwitchToFrameByIndex), commandMessage);

            try
            {
                Driver.SwitchTo().Frame(index);
                _currentFrame = frame ?? new FrameWebElement()
                {
                    Name = "Temp frame",
                    Description = "Switch by index",
                    FrameType = FrameWebElement.FrameLocatorType.Index,
                    FrameValue = index.ToString()
                };
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void SwitchToFrameById(string id, ILogger log, FrameWebElement frame = null)
        {
            var commandMessage = $"Switch to frame by id/name: '{id}'";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SwitchToFrameById), commandMessage);

            try
            {
                Driver.SwitchTo().Frame(id);
                _currentFrame = frame ?? new FrameWebElement()
                {
                    Name = "Temp frame",
                    Description = "Switch by id",
                    FrameType = FrameWebElement.FrameLocatorType.Id,
                    FrameValue = id
                };
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void SwitchToFrameByLocator(FrameWebElement frameWebElement, ILogger log)
        {
            var commandMessage = $"Switch to frame by locator";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SwitchToFrameByLocator), commandMessage);

            try
            {
                var wElem = FindElement(frameWebElement, log);
                Driver.SwitchTo().Frame(wElem);
                _currentFrame = frameWebElement;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public void TakeScreenshotOnFail(ILogger log, bool fullScreen = false)
        {
            if (!Config.MakeScreenshotOnFail) return;
            try
            {
                Driver.SwitchTo().Window(Driver.CurrentWindowHandle);

                var path = log.GetLoggedFilesFolder();
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path += "\\" + Guid.NewGuid() + ".png";

                var screen = MakeScreenshot(log, fullScreen);
                screen.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                log?.LOGITEM(
                    new LogFile
                    {
                        DateTimeStemp = DateTime.UtcNow,
                        Level = AutomatedMagic.LogLevel.ERROR,
                        Message = "Screenshot of browser window on fail",
                        FilePath = path,
                        FileType = LoggedFileType.PNG
                    }
                );
            }
            catch (Exception ex)
            {
                log?.WARN("Error occurred during creating fail screenshot", ex);
            }
        }

        public void ResizeInnerBound(int width, int height, ILogger log)
        {
            int _try = 1;
            while (_try-- > 0)
            {
                long clientHeight = (long)JSExecutor("return document.body.clientHeight", null);
                long clientWidth = (long)JSExecutor("return document.body.clientWidth", null);
                long scrollHeight = (long)JSExecutor("return document.body.scrollHeight", null);
                log?.DEBUG("clientHeight = " + clientHeight);
                log?.DEBUG("clientWidth = " + clientWidth);
                log?.DEBUG("scrollHeight = " + scrollHeight);


                long innerHeight = (long)JSExecutor("return window.innerHeight", null);
                long innerWidth = (long)JSExecutor("return window.innerWidth", null);
                log?.DEBUG("innerHeight = " + innerHeight);
                log?.DEBUG("innerWidth = " + innerWidth);
                int dw = width - (int)innerWidth;
                int dh = height - (int)innerHeight;

                var curSize = Driver.Manage().Window.Size;
                this.Driver.Manage().Window.Size = new Size(curSize.Width + dw, curSize.Height + dh);

                innerHeight = (long)JSExecutor("return window.innerHeight", null);
                innerWidth = (long)JSExecutor("return window.innerWidth", null);
                log?.DEBUG("innerHeight = " + innerHeight);
                log?.DEBUG("innerWidth = " + innerWidth);

                clientHeight = (long)JSExecutor("return document.body.clientHeight", null);
                clientWidth = (long)JSExecutor("return document.body.clientWidth", null);
                scrollHeight = (long)JSExecutor("return document.body.scrollHeight", null);
                log?.DEBUG("clientHeight = " + clientHeight);
                log?.DEBUG("clientWidth = " + clientWidth);
                log?.DEBUG("scrollHeight = " + scrollHeight);

                if (innerWidth >= width)
                    break;

                log?.DEBUG("");
                log?.DEBUG("");
            }
        }
        private Bitmap AddCropImage(Bitmap addToBitmap, Image source, Rectangle section)
        {
            int height = addToBitmap?.Height ?? 0;
            int width = addToBitmap?.Width ?? 0;
            if (width != 0 && width != section.Width)
                throw new Exception("Section width not equal to addToBitmap width");

            Bitmap bmp = new Bitmap(section.Width, section.Height + height);
            Graphics g = Graphics.FromImage(bmp);
            if (height > 0 && width > 0)
                g.DrawImage(addToBitmap, 0, 0, new Rectangle(0, 0, addToBitmap.Width, addToBitmap.Height), GraphicsUnit.Pixel);
            g.DrawImage(source, 0, height, section, GraphicsUnit.Pixel);

            return bmp;
        }
        public Bitmap MakeScreenshot(ILogger log, bool fullScreen = true)
        {
            Func<Bitmap> getScreenshot = () =>
            {
                Screenshot screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                Bitmap bmp = null;
                using (var ms = new MemoryStream(screenshot.AsByteArray))
                    bmp = new Bitmap(ms);
                return bmp;
            };
            Bitmap res = null;
            if (Config.DriverType == WebDriverType.Firefox || Config.DriverType == WebDriverType.IE)
            {
                res = getScreenshot();
            }
            else
            {
                if (!fullScreen)
                {
                    res = getScreenshot();
                }
                else
                {
                    long clientHeight = (long)JSExecutor("return document.body.clientHeight", null);
                    long clientWidth = (long)JSExecutor("return document.body.clientWidth", null);
                    long scrollHeight = (long)JSExecutor("return document.body.scrollHeight", null);
                    log?.DEBUG("clientHeight = " + clientHeight);
                    log?.DEBUG("clientWidth = " + clientWidth);
                    log?.DEBUG("scrollHeight = " + scrollHeight);

                    for (long cur = 0; cur < scrollHeight; cur += clientHeight)
                    {
                        Rectangle section;
                        if (cur > scrollHeight - clientHeight)
                        {
                            long height = scrollHeight - cur + 1;
                            section = new Rectangle(0, (int)(clientHeight - height + 1), (int)clientWidth, (int)height - 1);
                            height = cur - (scrollHeight - clientHeight);
                            section = new Rectangle(0, (int)(height), (int)clientWidth, (int)(clientHeight - height));
                        }
                        else
                        {
                            section = new Rectangle(0, 0, (int)clientWidth, (int)clientHeight);
                        }

                        JSExecutor($"document.body.scrollTop={cur}", null);
                        //var top = (long)wdm.JSExecutor("return document.body.scrollTop", null);
                        Thread.Sleep(3000);

                        using (Bitmap bmp = getScreenshot())
                        {
                            log?.DEBUG($"SCREENSHOT SIZE = {bmp.Width}x{bmp.Height}");
                            //string name = $"{id++}.png";
                            //var path = $"Screenshots\\{name}.png";
                            //bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                            //log?.ERROR("Test", LoggedFileType.PNG, path);
                            //bmp.Save(@"C:\Users\C5244500\Desktop\Git\Tests\SapAutomation.Tests\SapAutomation.Tests.Test\bin\Debug\Screenshots\DG\" + name, System.Drawing.Imaging.ImageFormat.Png);
                            log?.DEBUG($"Section . Size = {section.Width}x{section.Height}  x =  {section.X}  y = {section.Y}");
                            res = AddCropImage(res, bmp, section);
                        }
                    }
                }
            }

            return res;
        }
        public Bitmap CaptureElementScreenShot(IWebElement element, ILogger log)
        {
            var commandMessage = "Capture element screen shot";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(CaptureElementScreenShot), commandMessage);
            string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");

            try
            {
                Screenshot screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);

                Image img = Image.FromFile(filePath);
                Rectangle rectangle = new Rectangle();

                if (element != null)
                {
                    int width = element.Size.Width;
                    int height = element.Size.Height;

                    Point p = element.Location;
                    rectangle = new Rectangle(p.X, p.Y, width, height);
                }

                Bitmap bitmap = new Bitmap(img);
                var cropedImage = bitmap.Clone(rectangle, bitmap.PixelFormat);

                return cropedImage;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
            finally { if (filePath != null) File.Delete(filePath); }
        }

        public CookieContainer GetCookieContainer()
        {
            var cookieContainer = new CookieContainer();

            foreach (var cookie in Driver.Manage().Cookies.AllCookies)
            {
                try
                {
                    cookieContainer.Add(new System.Net.Cookie
                    {
                        Name = cookie.Name,
                        Value = cookie.Value,
                        Domain = cookie.Domain,
                        Path = cookie.Path
                    });
                }
                catch { }
            }
            return cookieContainer;
        }

        public OpenQA.Selenium.Cookie GetCookieNamed(string cookieName, ILogger log)
        {
            var commandMessage = $"Get a cookie with the specified name. Cookie name is: '{cookieName}'.";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetCookieNamed), commandMessage);
            try
            {
                var cookie = Driver.Manage().Cookies.GetCookieNamed(cookieName);
                return cookie;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public OpenQA.Selenium.Cookie NewCookie(string name, string value, string domain, string path, DateTime expiry, ILogger log)
        {
            var commandMessage = $"Initialize new cookie. Cookie name is: '{name}'.";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(NewCookie), commandMessage);
            try
            {
                var cookie = new OpenQA.Selenium.Cookie(name, value, domain, path, expiry);
                return cookie;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public OpenQA.Selenium.Cookie NewCookie(string name, string value, ILogger log)
        {
            var commandMessage = $"Initialize new cookie. Cookie name is: '{name}'.";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(NewCookie), commandMessage);
            try
            {
                var cookie = new OpenQA.Selenium.Cookie(name, value);
                return cookie;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void DeleteCookie(string name, string value, string domain, string path, DateTime expiry, ILogger log)
        {
            var cookie = NewCookie(name, value, domain, path, expiry, log);
            var commandMessage = $"Delete the specified cookie from the page. Cookie name is: '{name}'.";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(DeleteCookie), commandMessage);
            try
            {
                Driver.Manage().Cookies.DeleteCookie(cookie);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void DeleteCookieByName(string cookieName, ILogger log)
        {
            var commandMessage = $"Delete the cookie with the specified name from the page. Cookie name is: '{cookieName}'.";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(DeleteCookieByName), commandMessage);
            try
            {
                Driver.Manage().Cookies.DeleteCookieNamed(cookieName);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public void AddCookie(string name, string value, string domain, string path, DateTime expiry, ILogger log)
        {
            var cookie = NewCookie(name, value, domain, path, expiry, log);
            AddCookie(cookie, log);
        }

        public void AddCookie(OpenQA.Selenium.Cookie cookie, ILogger log)
        {
            var commandMessage = $"Add a cookie to the current page. Cookie name is: '{cookie.Name}'. Cookie value is: {cookie.Value}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(AddCookie), commandMessage);
            try
            {
                Driver.Manage().Cookies.AddCookie(cookie);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        
        public void AddCookie(string name, string value, ILogger log)
        {
            var cookie = NewCookie(name, value, log);
            AddCookie(cookie, log);
        }

        public void WaitForCompletelyPageLoaded(ILogger log)
        {
            var commandMessage = "Wait until page is completely loaded";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(WaitForCompletelyPageLoaded), commandMessage);

            try
            {
                WaitForPageLoaded(log);
                WaitForJQueryLoaded(log);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void WaitForPageLoaded(ILogger log, int timeoutInSec = -1)
        {
            var commandMessage = "Wait until page is loaded";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(WaitForPageLoaded), commandMessage);

            try
            {
                var tis = timeoutInSec == -1 ? Config.WaitTimeout.TotalSeconds : timeoutInSec;
                var sw = Stopwatch.StartNew();

                while (sw.Elapsed.TotalSeconds < tis)
                {
                    var r = "";

                    try
                    {
                        r = JSExecutor("return (this.document != undefined && this.document.readyState) ? this.document.readyState : 'undefined'", log).ToString().ToLower();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("TypeError: can't access dead object"))
                        {
                            this.Driver.Navigate().Refresh();
                        }
                        log?.WARN($"Error occurred during waiting for page load", ex);
                    }

                    if (r != "complete")
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    else
                    {
                        sw.Stop();
                        log?.INFO($"Page loaded in: {sw.Elapsed.TotalSeconds} seconds");
                        return;
                    }
                }

                throw new Exception($"Timeout: {tis} seconds is reached");
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void WaitForJQueryLoaded(ILogger log, int timeoutInSec = -1)
        {
            var commandMessage = "Wait until jquery is loaded";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(WaitForJQueryLoaded), commandMessage);

            try
            {
                var tis = timeoutInSec == -1 ? Config.WaitTimeout.TotalSeconds : timeoutInSec;
                var sw = Stopwatch.StartNew();

                while (sw.Elapsed.TotalSeconds < tis)
                {
                    var r = "";

                    try
                    {
                        r = JSExecutor("return jQuery.active", log).ToString();
                    }
                    catch (Exception ex)
                    {
                        log?.WARN($"Error occurred during waiting for JQuery load", ex);
                    }

                    if (r != "0")
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    else
                    {
                        sw.Stop();
                        log?.INFO($"Page loaded in: {sw.Elapsed.TotalSeconds} seconds");
                        return;
                    }
                }

                throw new Exception($"Timeout: {tis} seconds is reached");
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void WaitForFramesLoaded(ILogger log)
        {
            var commandMessage = "Wait until frames are loaded";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(WaitForFramesLoaded), commandMessage);

            try
            {
                Thread.Sleep(1000);
                Wait.Until(e => Equals(JSExecutor("return document.readyState", log).ToString().ToLower(), "complete"));
                var aFrames = Driver.FindElements(By.XPath("//iframe"));

                List<IWebElement> frames = new List<IWebElement>();
                foreach (var aFrame in aFrames)
                {
                    if (aFrame.Displayed && aFrame.Enabled)
                        frames.Add(aFrame);
                }

                if (frames.Count != 0)
                {
                    foreach (var frame in frames)
                    {
                        Driver.SwitchTo().Frame(frame);
                        Wait.Until(e => Equals(JSExecutor("return document.readyState", log).ToString().ToLower(), "complete"));
                        Driver.SwitchTo().ParentFrame();
                    }
                    foreach (var frame in frames)
                    {
                        Driver.SwitchTo().Frame(frame);
                        WaitForFramesLoaded(log);
                        Driver.SwitchTo().ParentFrame();
                    }
                }
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
    }
}

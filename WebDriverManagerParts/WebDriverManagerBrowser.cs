namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using OpenQA.Selenium;
    using System;
    using System.Linq;
    using System.Drawing;
    using System.Diagnostics;
    using WebElements;

    public partial class WebDriverManager
    {
        private string _startHandle = null;

        public void Navigate(string url, ILogger log, string user = null, string password = null)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(Navigate), url, user ?? "N/A user", password ?? "N/A password");

            try
            {
                log?.INFO($"Navigate to url: {url}");

                if (user != null && password != null)
                    url = url.Replace("://", $"://{user}:{password}@");
                Driver.Navigate().GoToUrl(url);

                if (_startHandle == null)
                {
                    if (Config.DriverType == WebDriverType.IE)
                        while (GetWindowsCount(log) > 1)
                            Close(log);
                    _startHandle = Driver.CurrentWindowHandle;
                }
                SwitchToDefaultContent(log, true);
                log?.DEBUG("URL navigating completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during URL {url} navigating", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void ClearCookies(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ClearCookies));

            try
            {
                log?.DEBUG($"Start clear cookies");

                Driver.Manage().Cookies.DeleteAllCookies();
                log?.DEBUG("Driver clearing cookies completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during driver clearing cookies", ex);
                throw;
            }
        }

        public void ForceClose(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ForceClose));

            try
            {
                log?.DEBUG($"Start force driver closing");

                Driver.Close();
                try
                {
                    Driver.SwitchTo().Alert().Accept();
                }
                catch (Exception)
                {

                }
                log?.DEBUG("Driver closing completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during driver closing", ex);
                throw;
            }
        }

        public void Close(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(Close));

            try
            {
                log?.DEBUG($"Start driver closing");
                log?.DEBUG($"Before windows count: {GetWindowsCount(log)}");
                log?.DEBUG($"Current window: {Driver.CurrentWindowHandle}");
                log?.DEBUG($"All handles: {string.Join("\n", Driver.WindowHandles)}");
                Driver.Close();

                if (GetWindowsCount(log) > 0)
                    SwitchToLastWindow(log);

                log?.DEBUG("Driver closing completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during driver closing", ex);
                throw;
            }
        }

        public void Quit(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(Quit));

            try
            {
                log?.DEBUG($"Start driver quitting");

                if (_driver == null) return;

                if (Config.DriverType == WebDriverType.Firefox)
                {
                    while (_driver.WindowHandles.Count > 1)
                    {
                        _driver.Navigate().GoToUrl("about:config");
                        _driver.Navigate().GoToUrl("about:blank");
                        _driver.Close();
                        _driver.SwitchTo().Window(_driver.WindowHandles.Last());
                    }
                    _driver.Navigate().GoToUrl("about:config");
                    _driver.Navigate().GoToUrl("about:blank");
                    _driver.Close();
                }

                try { Driver.Quit(); }
                catch { }

                var id = Config.ProcessID;
                if (id != 0)
                {
                    var arguments = new string[] { $"/pid {id} /F /T", "/IM WerFault.exe /F /T" };

                    Process process = null;
                    ProcessStartInfo startInfo = null;

                    foreach (var arg in arguments)
                    {
                        startInfo = new ProcessStartInfo
                        {
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            FileName = @"C:\Windows\System32\taskkill.exe",
                            Arguments = arg,
                            UseShellExecute = false
                        };

                        process = Process.Start(startInfo);
                        process.WaitForExit();

                        using (var stream = process.StandardOutput)
                        {
                            var str = stream.ReadToEnd();
                            log?.DEBUG(str);
                        }
                        using (var stream = process.StandardError)
                        {
                            var str = stream.ReadToEnd();
                            log?.WARN(str);
                        }
                    }
                }

                _driver = null;
                _wait = null;
                _jsExecutor = null;

                log?.DEBUG("Driver quitting completed. PID = " + id);
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during driver quitting", ex);
                throw;
            }
        }

        public void SetWindowSize(Size size,ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SetWindowSize), size);

            try
            {
                log?.DEBUG($"Resize window using size: {size}");

                Driver.Manage().Window.Size = size;
                log?.DEBUG("Window resizing completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during window resizing to size: {size}", ex);
                throw;
            }
        }

        public void SetWindowSize(int width, int height, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SetWindowSize), width, height);

            try
            {
                log?.DEBUG($"Resize window using width: {width} and height: {height}");

                Driver.Manage().Window.Size = new Size(width, height);
                log?.DEBUG("Window resizing completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during window resizing", ex);
                throw;
            }
        }

        public int GetWindowsCount(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetWindowsCount));
            try
            {
                log?.DEBUG($"Getting windows count");

                var count = Driver.WindowHandles.Count;

                log?.DEBUG($"Windows count = {count}");
                return count;
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during switching to opened window", ex);
                throw;
            }
        }

        public void SwitchToFirstOpenedWindow(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SwitchToFirstOpenedWindow));

            try
            {
                log?.DEBUG($"Switch to opened window");

                var current = Driver.CurrentWindowHandle;
                var windows = Driver.WindowHandles;
                if (windows.Count != 1)
                    Driver.SwitchTo().Window(windows.SingleOrDefault(w => w != current));

                log?.DEBUG("Switching to opened window completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during switching to opened window", ex);
                throw;
            }
        }

        public void SwitchToLastWindow(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SwitchToLastWindow));

            try
            {
                log?.DEBUG($"Switch to last window");
                string last = null;
                var handles = Driver.WindowHandles.ToList();
                log?.DEBUG("Handels.Count = " + handles.Count);
                if (handles.Count == 1)
                    last = handles[0];
                else
                    last = handles.LastOrDefault(h => h != _startHandle);
                log?.DEBUG($"Switch to '{last}'");
                log?.DEBUG($"All handles: {string.Join("\n", handles)}");
                log?.DEBUG($"Main window '{_startHandle}'");
                Driver.SwitchTo().Window(last);

                log?.DEBUG("Switching to last window completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during window switching", ex);
                throw;
            }
        }

        public void OpenNewWindow(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(OpenNewWindow));

            try
            {
                log?.DEBUG($"Open new Tab/Window");

                var cur = Driver.WindowHandles.Count;
                while (Driver.WindowHandles.Count <= cur)
                {
                    var body = Driver.FindElement(By.TagName("body"));
                    switch (Config.DriverType)
                    {
                        case WebDriverType.Firefox:
                            body.SendKeys(Keys.Control + 'n');
                            break;
                        case WebDriverType.Chrome:
                        case WebDriverType.IE:
                            body.SendKeys(Keys.Control + 't');
                            break;
                    }
                }

                SwitchToLastWindow(log);

                log?.DEBUG("Opening new tab/window completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during opening new tab/window", ex);
                throw;
            }
        }

        public void WindowMaximize(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(WindowMaximize));

            try
            {
                log?.DEBUG($"Maximize window");

                Driver.Manage().Window.Maximize();
                log?.DEBUG("Window maximizing completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during window maximizing", ex);
                throw;
            }
        }

        public void AcceptAlert(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(AcceptAlert));

            try
            {
                log?.DEBUG($"Accept alert");

                IAlert alert = Driver.SwitchTo().Alert();
                alert.Accept();
                log?.DEBUG("Alert accepting completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during alert accepting", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void DismissAlert(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(DismissAlert));

            try
            {
                log?.DEBUG($"Dismiss alert");

                IAlert alert = Driver.SwitchTo().Alert();
                alert.Dismiss();
                log?.DEBUG("Alert dismissing completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during alert dismissing", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public IAlert SwitchToAlert(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SwitchToAlert));

            try
            {
                log?.DEBUG($"Switch to alert");
                var alert = Driver.SwitchTo().Alert();
                log?.DEBUG("Switching to alert completed");
                return alert;
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during switching to alert", ex);
                throw;
            }
        }

        public void Back(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(Back));

            try
            {
                log?.TRACE($"Move back to single entry in browser's history");
                Driver.Navigate().Back();
                log?.TRACE($"Move back to single entry in browser's history completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during move back to single entry in browser's history", ex);
                throw;
            }
        }

        public void Refresh(ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(Refresh));

            try
            {
                log?.DEBUG($"Refresh page");

                Driver.Navigate().Refresh();

                SwitchToDefaultContent(log, true);
                WaitForCompletelyPageLoaded(log);

                log?.DEBUG("Refreshing page completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during page refreshing", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public string GetCurrentUrl(ILogger log = null)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetCurrentUrl));

            try
            {
                log?.DEBUG($"Start getting current url");
                var url = Driver.Url;

                log?.INFO($"Current Url = '{url}'");
                log?.DEBUG("Getting current url successfully completed");
                return url;
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred getting current url", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void ReloadPageIgnoreCache(ILogger log)
        {
            var currentUrl = GetCurrentUrl(log);

            var commandMessage = $"Reload the page '{currentUrl}', ignore cache";

            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ReloadPageIgnoreCache), commandMessage);

            try
            {
                JSExecutor("window.location.reload(true)", log);
                WaitForPageLoaded(log);

                log?.DEBUG($"Page '{currentUrl}' successfully reloaded");
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
    }
}

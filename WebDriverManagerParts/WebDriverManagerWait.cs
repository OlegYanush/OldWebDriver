namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using System;
    using OpenQA.Selenium.Interactions;
    using WebElements;
    using OpenQA.Selenium;
    using System.Collections.Generic;
    using System.Threading;
    using System.Linq;
    using OpenQA.Selenium.Support.UI;
    using System.Diagnostics;

    public partial class WebDriverManager
    {
        public IWebElement WaitForElementCondition(IWebElement parentElement, By locator, Func<IWebElement, bool> condition, int timeoutInSec = 0, int pollingInSec = 0)
        {
            var sw = Stopwatch.StartNew();
            timeoutInSec = timeoutInSec == 0 ? (int)Config.WaitTimeout.TotalSeconds : timeoutInSec;
            pollingInSec = pollingInSec == 0 ? (int)Config.PollingInterval.TotalSeconds : pollingInSec;
            var lastSpent = 0;
            while (sw.Elapsed.TotalSeconds < timeoutInSec)
            {
                var element = TryFindElementInParentWithTimeFrame(parentElement, locator, pollingInSec);
                if (element != null && condition(element))
                    return element;

                var sleepTime = pollingInSec * 1000 - (int)sw.Elapsed.TotalMilliseconds - lastSpent;
                if (sleepTime > 0)
                    Thread.Sleep(sleepTime);
            }

            throw new WebDriverTimeoutException($"Timeout {timeoutInSec} reached during waiting for condition");
        }
        public IWebElement WaitForElementAttributePresenceState(IWebElement parentElement, By locator, string attribute, bool isPresent, int timeoutInSec = 0, int pollingInSec = 0)
        {
            return WaitForElementCondition(
                parentElement,
                locator,
                WebElementConditions.HasAttributePresenceState(attribute, isPresent),
                timeoutInSec,
                pollingInSec
            );
        }
        public IWebElement WaitForElementAttributeContainsText(IWebElement parentElement, By locator, string attribute, string text, int timeoutInSec = 0, int pollingInSec = 0)
        {
            return WaitForElementCondition(
                parentElement,
                locator,
                WebElementConditions.AttributeContainsText(attribute, text),
                timeoutInSec,
                pollingInSec
            );
        }

        public IWebElement WaitForElementCondition(By locator, Func<IWebElement, bool> condition, int timeoutInSec = 0, int pollingInSec = 0)
        {
            var sw = Stopwatch.StartNew();
            timeoutInSec = timeoutInSec == 0 ? (int)Config.WaitTimeout.TotalSeconds : timeoutInSec;
            pollingInSec = pollingInSec == 0 ? (int)Config.PollingInterval.TotalSeconds : pollingInSec;
            var lastSpent = 0;
            while (sw.Elapsed.TotalSeconds < timeoutInSec)
            {
                var element = TryFindElementWithTimeFrame(locator, pollingInSec);
                if (element != null && condition(element)) return element;

                var sleepTime = pollingInSec * 1000 - (int)sw.Elapsed.TotalMilliseconds - lastSpent;
                if (sleepTime > 0)
                    Thread.Sleep(sleepTime);
            }
            throw new WebDriverTimeoutException($"Timeout {timeoutInSec} reached during waiting for condition for locator: {locator}");
        }
        public IWebElement WaitForElementAttributePresenceState(By locator, string attribute, bool isPresent, int timeoutInSec = 0, int pollingInSec = 0)
        {
            return WaitForElementCondition(
                locator,
                WebElementConditions.HasAttributePresenceState(attribute, isPresent),
                timeoutInSec,
                pollingInSec
            );
        }
        public IWebElement WaitForElementAttributeContainsText(By locator, string attribute, string text, int timeoutInSec = 0, int pollingInSec = 0)
        {
            return WaitForElementCondition(
                locator,
                WebElementConditions.AttributeContainsText(attribute, text),
                timeoutInSec,
                pollingInSec
            );
        }

        public IWebElement WaitForElementCondition(WebElement webElement, Func<IWebElement, bool> condition, ILogger log, int timeoutInSec = 0, int pollingInSec = 0)
        {
            var commandMessage = $"Wait for condition for element: {webElement.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(WaitForElementState), commandMessage);

            try
            {
                var sw = Stopwatch.StartNew();
                timeoutInSec = timeoutInSec == 0 ? (int)Config.WaitTimeout.TotalSeconds : timeoutInSec;
                pollingInSec = pollingInSec == 0 ? (int)Config.PollingInterval.TotalSeconds : pollingInSec;
                var lastSpent = 0;
                while (sw.Elapsed.TotalSeconds < timeoutInSec)
                {
                    var element = TryFindElementWithTimeFrame(webElement, pollingInSec);
                    if (element != null && condition(element)) return element;

                    var sleepTime = pollingInSec * 1000 - (int)sw.Elapsed.TotalMilliseconds - lastSpent;
                    if (sleepTime > 0)
                        Thread.Sleep(sleepTime);
                }
                throw new WebDriverTimeoutException($"Timeout {timeoutInSec} reached");
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }
        public IWebElement WaitForElementAttributePresenceState(WebElement webElement, string attribute, bool isPresent, ILogger log, int timeoutInSec = 0, int pollingInSec = 0)
        {
            var commandMessage = $"Wait for attribute {attribute} {(isPresent ? "is" : "is not")} present in element: {webElement.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(WaitForElementState), commandMessage);

            try
            {
                return WaitForElementCondition(
                    webElement,
                    WebElementConditions.HasAttributePresenceState(attribute, isPresent),
                    null,
                    timeoutInSec,
                    pollingInSec
                );
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }
        public IWebElement WaitForElementAttributeContainsText(WebElement webElement, string attribute, string text, ILogger log, int timeoutInSec = 0, int pollingInSec = 0)
        {
            var commandMessage = $"Wait for attribute {attribute} contains text '{text}' present in element: {webElement.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(WaitForElementState), commandMessage);

            try
            {
                return WaitForElementCondition(
                    webElement,
                    WebElementConditions.AttributeContainsText(attribute, text),
                    null,
                    timeoutInSec,
                    pollingInSec
                );
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }

        public IWebElement WaitForElementState(By locator, WebElementState state, int timeoutInSec = 0, int pollingInSec = 0)
        {
            switch (state)
            {
                case WebElementState.Present:
                case WebElementState.Visible:
                case WebElementState.Enabled:
                case WebElementState.NotPresent:
                case WebElementState.NotVisible:
                case WebElementState.NotVisibleOrNotPresent:
                case WebElementState.NotEnabled:
                case WebElementState.ReadyForAction:
                    break;
                case WebElementState.None:
                    throw new Exception("Couldn't wait for state: WebElementState.None");
                default:
                    throw new Exception($"Unknown state for wait: {state}");
            }

            var sw = Stopwatch.StartNew();
            timeoutInSec = timeoutInSec == 0 ? (int)Config.WaitTimeout.TotalSeconds : timeoutInSec;
            pollingInSec = pollingInSec == 0 ? (int)Config.PollingInterval.TotalSeconds : pollingInSec;
            var lastSpent = 0;
            while (sw.Elapsed.TotalSeconds < timeoutInSec)
            {
                var element = TryFindElementWithTimeFrame(locator, pollingInSec);

                if (element != null)
                {
                    switch (state)
                    {
                        case WebElementState.Present:
                            return element;
                        case WebElementState.Visible:
                            if (element.Displayed) return element;
                            break;
                        case WebElementState.Enabled:
                            if (element.Enabled) return element;
                            break;
                        case WebElementState.NotVisibleOrNotPresent:
                        case WebElementState.NotVisible:
                            if (!element.Displayed) return element;
                            break;
                        case WebElementState.NotEnabled:
                            if (!element.Enabled) return element;
                            break;
                        case WebElementState.ReadyForAction:
                            if (element.Displayed && element.Enabled) return element;
                            break;
                    }
                }
                else if (state.HasFlag(WebElementState.NotPresent))
                    return element;


                var sleepTime = pollingInSec * 1000 - (int)sw.Elapsed.TotalMilliseconds - lastSpent;
                if (sleepTime > 0)
                    Thread.Sleep(sleepTime);
            }

            throw new WebDriverTimeoutException($"Timeout {timeoutInSec} reached during waiting for state {state} for locator: {locator}");
        }
        public IWebElement WaitForElementState(IWebElement parentElement, By locator, WebElementState state, int timeoutInSec = 0, int pollingInSec = 0)
        {
            switch (state)
            {
                case WebElementState.Present:
                case WebElementState.Visible:
                case WebElementState.Enabled:
                case WebElementState.NotPresent:
                case WebElementState.NotVisible:
                case WebElementState.NotVisibleOrNotPresent:
                case WebElementState.NotEnabled:
                case WebElementState.ReadyForAction:
                    break;
                case WebElementState.None:
                    throw new Exception("Couldn't wait for state: WebElementState.None");
                default:
                    throw new Exception($"Unknown state for wait: {state}");
            }

            var sw = Stopwatch.StartNew();
            timeoutInSec = timeoutInSec == 0 ? (int)Config.WaitTimeout.TotalSeconds : timeoutInSec;
            pollingInSec = pollingInSec == 0 ? (int)Config.PollingInterval.TotalSeconds : pollingInSec;
            var lastSpent = 0;
            while (sw.Elapsed.TotalSeconds < timeoutInSec)
            {
                var element = TryFindElementInParentWithTimeFrame(parentElement, locator, pollingInSec);

                if (element != null)
                {
                    switch (state)
                    {
                        case WebElementState.Present:
                            return element;
                        case WebElementState.Visible:
                            if (element.Displayed) return element;
                            break;
                        case WebElementState.Enabled:
                            if (element.Enabled) return element;
                            break;
                        case WebElementState.NotVisibleOrNotPresent:
                        case WebElementState.NotVisible:
                            if (!element.Displayed) return element;
                            break;
                        case WebElementState.NotEnabled:
                            if (!element.Enabled) return element;
                            break;
                        case WebElementState.ReadyForAction:
                            if (element.Displayed && element.Enabled) return element;
                            break;
                    }
                }
                else if (state.HasFlag(WebElementState.NotPresent))
                    return element;


                var sleepTime = pollingInSec * 1000 - (int)sw.Elapsed.TotalMilliseconds - lastSpent;
                if (sleepTime > 0)
                    Thread.Sleep(sleepTime);
            }

            throw new WebDriverTimeoutException($"Timeout {timeoutInSec} reached during waiting for state {state} for locator: {locator}");
        }
        public IWebElement WaitForElementState(WebElement webElement, WebElementState state, ILogger log, int timeoutInSec = 0, int pollingInSec = 0)
        {
            var commandMessage = $"Wait for state: {state} for element: {webElement.Description}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(WaitForElementState), commandMessage);

            try
            {
                switch (state)
                {
                    case WebElementState.Present:
                    case WebElementState.Visible:
                    case WebElementState.Enabled:
                    case WebElementState.NotPresent:
                    case WebElementState.NotVisible:
                    case WebElementState.NotVisibleOrNotPresent:
                    case WebElementState.NotEnabled:
                    case WebElementState.ReadyForAction:
                        break;
                    case WebElementState.None:
                        throw new Exception("Couldn't wait for state: WebElementState.None");
                    default:
                        throw new Exception($"Unknown state for wait: {state}");
                }

                var sw = Stopwatch.StartNew();
                timeoutInSec = timeoutInSec == 0 ? (int)Config.WaitTimeout.TotalSeconds : timeoutInSec;
                pollingInSec = pollingInSec == 0 ? (int)Config.PollingInterval.TotalSeconds : pollingInSec;
                var lastSpent = 0;
                while (sw.Elapsed.TotalSeconds < timeoutInSec)
                {
                    var element = TryFindElementWithTimeFrame(webElement, pollingInSec);

                    if (element != null)
                    {
                        switch (state)
                        {
                            case WebElementState.Present:
                                return element;
                            case WebElementState.Visible:
                                if (element.Displayed) return element;
                                break;
                            case WebElementState.Enabled:
                                if (element.Enabled) return element;
                                break;
                            case WebElementState.NotVisibleOrNotPresent:
                            case WebElementState.NotVisible:
                                if (!element.Displayed) return element;
                                break;
                            case WebElementState.NotEnabled:
                                if (!element.Enabled) return element;
                                break;
                            case WebElementState.ReadyForAction:
                                if (element.Displayed && element.Enabled) return element;
                                break;
                        }
                    }
                    else if (state.HasFlag(WebElementState.NotPresent))
                        return element;


                    var sleepTime = pollingInSec * 1000 - (int)sw.Elapsed.TotalMilliseconds - lastSpent;
                    if (sleepTime > 0)
                        Thread.Sleep(sleepTime);
                }

                throw new WebDriverTimeoutException($"Timeout {timeoutInSec} reached");
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                TakeScreenshotOnFail(log);
                throw new Exception(commandMessage, commandException);
            }
        }

        public IWebElement TryFindElementWithTimeFrame(By locator, int searchTimeoutInSec)
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(searchTimeoutInSec);
                return Driver.FindElement(locator);
            }
            catch { }
            finally
            {
                Driver.Manage().Timeouts().ImplicitWait = Config.SearchTimeout;
            }
            return null;
        }
        public IWebElement TryFindElementInParentWithTimeFrame(IWebElement parentElement, By locator, int searchTimeoutInSec)
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(searchTimeoutInSec);

                if (parentElement != null)
                    return parentElement.FindElement(locator);
                else return Driver.FindElement(locator);
            }
            catch { }
            finally
            {
                Driver.Manage().Timeouts().ImplicitWait = Config.SearchTimeout;
            }
            return null;
        }
        public IWebElement TryFindElementWithTimeFrame(WebElement element, int searchTimeoutInSec)
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(searchTimeoutInSec);
                return FindElement(element, null);
            }
            catch { }
            finally
            {
                Driver.Manage().Timeouts().ImplicitWait = Config.SearchTimeout;
            }
            return null;
        }

        public List<IWebElement> TryFindElementsWithTimeFrame(WebElement element, int searchTimeoutInSec)
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(searchTimeoutInSec);
                return FindElements(element, null);
            }
            catch { }
            finally
            {
                Driver.Manage().Timeouts().ImplicitWait = Config.SearchTimeout;
            }
            return new List<IWebElement>();
        }

        public List<T> SpamWithFunction<T>(
            Func<WebDriverManager, T> spamFunction,
            int timeFrameInSec,
            int pollingInSec,
            bool immidiateSpamOnError = false)
        {
            var list = new List<T>();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(pollingInSec);

            var sw = Stopwatch.StartNew();

            while (sw.Elapsed.TotalSeconds < timeFrameInSec)
            {
                var current = sw.Elapsed.TotalMilliseconds;
                try
                {
                    var r = spamFunction(this);
                    list.Add(r);
                }
                catch
                {
                    if (immidiateSpamOnError) continue;
                }

                var toSleep = pollingInSec * 1000 - (sw.Elapsed.TotalMilliseconds - current);
                if (toSleep > 0) Thread.Sleep((int)toSleep);
            }

            Driver.Manage().Timeouts().ImplicitWait = Config.SearchTimeout;
            return list;
        }
    }
}

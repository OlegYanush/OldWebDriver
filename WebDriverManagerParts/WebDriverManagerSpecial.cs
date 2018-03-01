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

    public partial class WebDriverManager
    {
        public string GetStyle(WebElement element, string styleName, ILogger log)
        {
            var commandMessage = $"Get style '{styleName}' for element {element.Name}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetStyle), commandMessage);
            try
            {
                log?.DEBUG($"Element: {element}");
                var styles = GetAttribute(element, "style", log);
                return GetStyle(styles, styleName, log);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public string GetStyle(IWebElement element, string styleName, ILogger log)
        {
            var commandMessage = $"Get style '{styleName}' for element {element.TagName}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetStyle), commandMessage);
            try
            {
                log?.DEBUG($"Element: {element}");
                var styles = element.GetAttribute("style");
                return GetStyle(styles, styleName, log);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public string GetStyle(string styles, string styleName, ILogger log)
        {
            var commandMessage = $"Extract style '{styleName}' from styles '{styles}'";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetStyle), commandMessage);
            try
            {
                var _style = styles.Split(';').FirstOrDefault(s => s.Trim().StartsWith(styleName));
                string value = _style?.Substring(_style.IndexOf(':') + 1).Trim();

                log?.DEBUG($"Style value '{styleName}' = '{value}'");
                return value;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public void SelectRadio(WebElement radioButtonGroup, string value, ILogger log, bool visibilityRequired = true)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SelectRadio), ($"Select '{value}' from radio button group {radioButtonGroup.Name}"));

            try
            {
                log?.DEBUG($"Radio button group: {radioButtonGroup}");

                var child = radioButtonGroup.ChildWebElements.FirstOrDefault(el => (el as CheckboxWebElement)?.ValueForSelect == value);
                if (child == null)
                    throw new Exception($"No child with value = '{value}'");

                SetCheckboxState(child, true, log);
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during selecting value '{value}' from radio button group: {radioButtonGroup}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }


        public void Select(IWebElement dropdown, WebElement options, string text, ILogger log)
        {
            var commandMessage = $"Select text option '{text}' from dropdown {dropdown.TagName}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(Select), commandMessage);

            try
            {
                log?.DEBUG($"Options: {options}");

                dropdown.Click();
                Thread.Sleep(1000);

                var option = options.GetCopyWithXPathReplace("toReplace", "'" + text + "'");
                option.Locator.IsRelative = false;
                Click(option, log);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void Select(WebElement dropdown, WebElement options, string text, ILogger log)
        {
            var commandMessage = $"Select text option '{text}' from dropdown {dropdown.Name}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(Select), commandMessage);

            try
            {
                log?.DEBUG($"Dropdown: {dropdown}");
                log?.DEBUG($"Options: {options}");

                Select(FindElement(dropdown, log), options, text, log);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void SelectByText(WebElement dropdown, string text, ILogger log)
        {
            var commandMessage = $"Select text '{text}' from dropdown {dropdown.Name}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SelectByText), commandMessage);

            try
            {
                var drop = FindElement(dropdown, log);
                var elem = new SelectElement(drop);
                elem.SelectByText(text);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
        public void SelectByValue(WebElement dropdown, string value, ILogger log)
        {
            var commandMessage = $"Select value '{value}' from dropdown {dropdown.Name}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SelectByValue), commandMessage);

            try
            {
                var drop = FindElement(dropdown, log);
                var elem = new SelectElement(drop);
                elem.SelectByValue(value);
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public void SetCheckboxState(WebElement chekboxElement, bool isChecked, ILogger log, int timeoutInSec = 0)
        {
            var commandMessage = $"Set checkbox {chekboxElement.Name} state to {(isChecked ? "checked" : "unchecked")}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SetCheckboxState), commandMessage);


            try
            {
                IWebElement el = null;
                try
                {

                    el = WaitForElementState(chekboxElement, WebElementState.ReadyForAction, log, 3);
                }
                catch
                {
                    el = WaitForElementState(chekboxElement, WebElementState.Enabled, log, timeoutInSec);
                    var opacity = el.GetCssValue("opacity");
                    if (opacity != "0")
                    {
                        throw;
                    }

                    log?.INFO($"Checkbox is not displayed due to opacity = 0");
                }

                if (el.Selected)
                {
                    if (isChecked)
                    {
                        log?.INFO("Checkbox is already checked");
                    }
                    else
                    {
                        JSScrollTo(el, log);
                        el.Click();
                        log?.INFO("Checkbox has been checked");
                    }
                }
                else
                {
                    if (!isChecked)
                    {
                        log?.INFO("Checkbox is already unchecked");
                    }
                    else
                    {
                        JSScrollTo(el, log);
                        el.Click();
                        log?.INFO("Checkbox has been unchecked");
                    }
                }
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public bool GetCheckboxState(WebElement chekboxElement, ILogger log, int timeoutInSec = 0)
        {
            var commandMessage = $"Get checkbox {chekboxElement.Name} state";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetCheckboxState), commandMessage);


            try
            {
                IWebElement el = null;
                try
                {

                    el = WaitForElementState(chekboxElement, WebElementState.ReadyForAction, log, 3);
                }
                catch
                {
                    el = WaitForElementState(chekboxElement, WebElementState.Enabled, log, timeoutInSec);
                    var opacity = el.GetCssValue("opacity");
                    if (opacity != "0")
                    {
                        throw;
                    }

                    log?.INFO($"Checkbox is not displayed due to opacity = 0");
                }

                var isChecked = el.Selected;

                log?.INFO($"Checkbox state is {(isChecked ? "checked" : "unchecked")}");
                return isChecked;
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }
    }
}
namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using System;
    using OpenQA.Selenium.Interactions;
    using WebElements;
    using OpenQA.Selenium;

    public partial class WebDriverManager
    {
        public void ActionsMoveTo(WebElement element,ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ActionsMoveTo), element);

            try
            {
                log?.DEBUG($"Move to {element}");
                var el = WaitForElementState(element, WebElementState.ReadyForAction, log);

                new Actions(Driver).MoveToElement(el).Build().Perform();
                log?.DEBUG("Move to completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred moving to element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void ActionsClick(WebElement element,ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ActionsClick), element);

            try
            {
                log?.DEBUG($"Actions click on {element}");
                var el = WaitForElementState(element, WebElementState.ReadyForAction, log);

                new Actions(Driver).Click(el).Build().Perform();
                log?.DEBUG("Actions click completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during actions clicking on element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public enum KeyboardButton
        {
            Shift,
            Ctrl
        }
        public void ActionsClickWithHoldedButton(WebElement element, KeyboardButton keyboardButton, ILogger log)
        {
            var commandMessage = $"Click on element {element.Name} with pressed button: {keyboardButton}";
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ActionsClickWithHoldedButton), commandMessage);

            try
            {
                log?.DEBUG($"Actions click on {element}");
                var el = WaitForElementState(element, WebElementState.ReadyForAction, log);

                var key = "";
                switch (keyboardButton)
                {
                    case KeyboardButton.Shift:
                        key = Keys.Shift;
                        break;
                    case KeyboardButton.Ctrl:
                        key = Keys.Control;
                        break;
                    default:
                        throw new Exception($"Unknown KeyboardButton: {keyboardButton}");
                }

                new Actions(Driver)
                    .KeyDown(key)
                    .Click(el)
                    .KeyUp(key)
                    .Build()
                    .Perform();

                log?.DEBUG("Actions click completed");
            }
            catch (Exception commandException)
            {
                log?.ERROR(commandMessage, commandException);
                throw new Exception(commandMessage, commandException);
            }
        }

        public void ActionsRightClick(WebElement element, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ActionsRightClick), element);

            try
            {
                log?.DEBUG($"Actions right click on {element}");
                var el = WaitForElementState(element, WebElementState.ReadyForAction, log);

                new Actions(Driver).ContextClick(el).Build().Perform();
                log?.DEBUG("Actions right click completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during actions right-clicking on element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void ActionsDoubleClick(WebElement element,ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ActionsDoubleClick), element);

            try
            {
                log?.DEBUG($"Actions double click on {element}");
                var el = WaitForElementState(element, WebElementState.ReadyForAction, log);

                new Actions(Driver).DoubleClick(el).Build().Perform();
                log?.DEBUG("Actions double click completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during actions double-clicking on element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void ActionsSendKeys(WebElement element, string value,ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ActionsSendKeys), element, value);

            try
            {
                log?.DEBUG($"Actions send keys to element: {element}");
                var el = WaitForElementState(element, WebElementState.ReadyForAction, log);

                new Actions(Driver).SendKeys(el, value).Build().Perform();
                log?.DEBUG("Actions send keys completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during actions keys sending to element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void ActionsDragAndDrop(WebElement source, WebElement target, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(ActionsDragAndDrop), source, target);

            try
            {
                log?.DEBUG($"Actions drag from {source}");
                log?.DEBUG($"Drag to {target}");

                var eSource = FindElement(source, log);
                var eTarget = FindElement(target, log);

                new Actions(Driver).MoveToElement(eSource).ClickAndHold(eSource).MoveToElement(eTarget).Release(eTarget).Build().Perform();
                log?.DEBUG("Actions drag and drop completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during actions drag and drop: from {source}\n to {target}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }
    }
}

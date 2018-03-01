namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using OpenQA.Selenium;
    using System;
    using System.Threading;
    using System.Collections.Generic;
    using WebElements;
    using System.Diagnostics;

    public partial class WebDriverManager
    {
        public void JSClick(WebElement webElement, ILogger log)
        {
            log?.DEBUG("JS click on element started");
            try
            {
                string jsScript = "var evObj = document.createEvent('MouseEvents');evObj.initMouseEvent('click',true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);arguments[0].dispatchEvent(evObj);";
                JSExecutor(jsScript, webElement, log);
                log?.TRACE("JS clicking on element has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS clicking on element has been completed with exception", ex);
                throw;
            }
        }

        public void JSClick(IWebElement element, ILogger log)
        {
            log?.DEBUG("JS click on element started");
            try
            {
                string jsScript = "var evObj = document.createEvent('MouseEvents');evObj.initMouseEvent('click',true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);arguments[0].dispatchEvent(evObj);";
                JSExecutor(jsScript, new object[] { element }, log);
                log?.TRACE("JS clicking on element has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS clicking on element has been completed with exception", ex);
                throw;
            }
        }


        public void JSDoubleClick(WebElement webElement, ILogger log)
        {
            log?.DEBUG("JS double click on element started");
            try
            {
                string jsScript = "var evObj = document.createEvent('MouseEvents');evObj.initMouseEvent(\"dblclick\",true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);arguments[0].dispatchEvent(evObj);";
                JSExecutor(jsScript, webElement, log);
                log?.TRACE("JS double clicking on element has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS double clicking on element has been completed with exception", ex);
                throw;
            }
        }

        public void JSContextMenu(WebElement webElement, ILogger log)
        {
            log?.DEBUG("JS right click on element started");
            try
            {
                string jsScript = "var evObj = document.createEvent('MouseEvents');evObj.initMouseEvent(\"contextmenu\",true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);arguments[0].dispatchEvent(evObj);";
                JSExecutor(jsScript, webElement, log);
                log?.TRACE("JS right clicking on element has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS right clicking on element has been completed with exception", ex);
                throw;
            }
        }

        public void JSMouseOver(WebElement webElement, ILogger log)
        {
            log?.DEBUG("JS move to element started");
            try
            {
                string jsScript = "var evObj = document.createEvent('MouseEvents');evObj.initMouseEvent(\"mouseover\",true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);arguments[0].dispatchEvent(evObj);";
                JSExecutor(jsScript, webElement, log);
                log?.TRACE("JS moving to element has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS moving to element has been completed with exception", ex);
                throw;
            }
        }

        public void JSShow(WebElement webElement, ILogger log)
        {
            log?.DEBUG("JS show element started");
            try
            {
                JSExecutor("arguments[0].style.display = block;", webElement, log);
                log?.TRACE("JS element showing has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS element showing has been completed with exception", ex);
                throw;
            }
        }

        public void JSHide(WebElement webElement, ILogger log)
        {
            log?.DEBUG("JS hide element started");
            try
            {
                JSExecutor("arguments[0].style.display = none;", webElement, log);
                log?.TRACE("JS element hiding has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS element hiding has been completed with exception", ex);
                throw;
            }
        }

        public void JSScrollIntoView(WebElement webElement, ILogger log)
        {
            log?.DEBUG("JS scroll into view to element started");
            try
            {
                JSExecutor($"arguments[0].scrollIntoView(true);", webElement, log);
                log?.TRACE("JS scrolling into view to element has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS scrolling into view to element has been completed with exception", ex);
                throw;
            }
        }

        public void JSScrollTo(WebElement webElement, ILogger log)
        {
            log?.DEBUG("JS scroll to element started");
            try
            {
                var elem = FindElement(webElement, log);
                JSExecutor($"window.scrollTo({elem.Location.X}, {elem.Location.Y})", log);
                log?.TRACE("JS scrolling to element has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS scrolling to element has been completed with exception", ex);
                throw;
            }
        }

        public void JSScrollByVertically(int pixels, ILogger log)
        {
            log?.DEBUG($"JS scroll by vertically {pixels} pixel(s)");
            try
            {
                JSExecutor($"window.scrollBy(0, {pixels})", log);
                log?.TRACE($"JS scrolling by vertically {pixels} pixel(s) has been completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"JS scroll by vertically {pixels} pixel(s) has been completed with exception", ex);
                throw;
            }
        }

        public void JSScrollTo(IWebElement element, ILogger log)
        {
            log?.DEBUG("JS scroll to element started");
            try
            {
                JSExecutor($"window.scrollTo({element.Location.X}, {element.Location.Y})", log);
                log?.TRACE("JS scrolling to element has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS scrolling to element has been completed with exception", ex);
                throw;
            }
        }

        public void JSScrollToBottom(ILogger log)
        {
            log?.DEBUG("JS scroll to bottom of the page started");
            try
            {
                JSExecutor($"window.scrollTo(0, document.body.scrollHeight)", log);
                log?.TRACE("JS scrolling to bottom of the page has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS scrolling to bottom of the page has been completed with exception", ex);
                throw;
            }
        }

        public void JSScrollToBottom(IWebElement elem, ILogger log, int sleepTimeSec = 1)
        {
            log?.DEBUG("JS scroll to bottom of the page started");
            try
            {
                int _sleep = sleepTimeSec * 1000;
                long scrollHeightPrev = 0;
                while (true)
                {
                    long scrollHeight = (long)JSExecutor("return arguments[0].scrollHeight", elem, log);
                    if (scrollHeight > scrollHeightPrev)
                    {
                        scrollHeightPrev = scrollHeight;
                        JSExecutor($"arguments[0].scrollTop={scrollHeight}", elem, log);
                        Thread.Sleep(_sleep);
                    }
                    else break;
                }
                log?.TRACE("JS scrolling to bottom of the page has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS scrolling to bottom of the page has been completed with exception", ex);
                throw;
            }
        }

        public void HighLightElement(IWebElement element, ILogger log)
        {
            log?.DEBUG("JS highlight element started");
            try
            {
                string highlightJavascript = @"arguments[0].style.cssText = ""border-width: 2px; border-style: solid; border-color: red"";";
                JSExecutor(highlightJavascript, new object[] { element }, log);
                log?.TRACE("JS element highlighting has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS element highlighting has been completed with exception", ex);
                throw;
            }
        }

        public void UnHighLightElement(IWebElement element, ILogger log)
        {
            log?.DEBUG("JS un-highlight element started");
            try
            {
                string highlightJavascript = @"arguments[0].style.cssText = ""border-width: 0px; border-style: solid; border-color: red"";";
                JSExecutor(highlightJavascript, new object[] { element }, log);
                log?.TRACE("JS element un-highlighting has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS element un-highlighting has been completed with exception", ex);
                throw;
            }
        }

        public void JSWaitForFramesLoaded(ILogger log)
        {
            log?.DEBUG("JS Wait until frames are loaded");
            try
            {
                Thread.Sleep(1000);
                string script = @"function IframeWatcher(){this.IsAllLoaded=function(){var e=this.doMagic(document);return e},this.doMagic=function(e){var o=!0,r=null;if(e.tagName&&'iframe'==e.tagName.toLowerCase()){if(!$(e).is(':visible')||!$(e).is(':enabled'))return!0;try{r=$(e).contents()}catch(t){return console.log('error'),console.log(e),!1}}else r=$(e);if(0==r.length)return console.log('error'),console.log(e),!1;var n=r[0].readyState;if('complete'!=n)return console.log(n),console.log(e),!1;for(var l=r.find('iframe'),a=0;a<l.length;a++)o&=this.doMagic(l[a]);return o?!0:!1}}; var tt = new IframeWatcher(); return tt.IsAllLoaded();";
                Wait.Until(e => Equals(bool.Parse(JSExecutor(script, log).ToString()), true));

                log?.TRACE($"JS Waiting for frames loading has been successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR("JS Waiting for frames loading has been completed with exception", ex);
                throw;
            }
        }

        public void JSSendKeys(WebElement element, string value, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(JSSendKeys), element, value);

            try
            {
                log?.DEBUG($"Send keys '{value}' to element {element}");
                JSExecutor($"$(arguments[0]).val('{value}')", element, log);
                log?.DEBUG($"Sending keys '{value}' completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during sending keys '{value}' to element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void JSSendKeys( IWebElement element, string value, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(JSSendKeys), element, value);

            try
            {
                log?.DEBUG($"Send keys '{value}' to element {element}");
                JSExecutor($"$(arguments[0]).val('{value}')", element, log);
                log?.DEBUG($"Sending keys '{value}' completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during sending keys '{value}' to element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void SetAttribute(WebElement element, string attribute, string value, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SetAttribute), element, attribute, value);
            try
            {
                log?.DEBUG($"Set attribute: '{attribute}' value: '{value}' for element: {element}");

                var el = FindElement(element, log);
                string highlightJavascript = "arguments[0].setAttribute(arguments[1], arguments[2]);";
                JSExecutor(highlightJavascript, new object[] { el, attribute, value }, log);

                log?.DEBUG($"Setting attribute: '{attribute}' value: '{value}' for element: {element} successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during setting attribute: '{attribute}' value: '{value}' for element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void DeleteAttribute(WebElement element,string attribute, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(DeleteAttribute), element, attribute);
            try
            {
                log?.DEBUG($"Delete attribute: '{attribute}' for element: {element}");

                var el = FindElement(element, log);
                string highlightJavascript = $"arguments[0].removeAttribute('{attribute}');";
                JSExecutor(highlightJavascript, new object[] { el }, log);

                log?.DEBUG($"Deleting attribute: '{attribute}' for element: {element} successfully completed");
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during deleting attribute: '{attribute}' for element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }
        public void SetStyle(WebElement element, List<string> styles, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SetStyle), element, styles);
            try
            {
                var el = FindElement(element, log);
                SetStyle(el, styles, log);
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during setting styles for element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void SetStyle( IWebElement element, List<string> styles, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(SetStyle), element, styles);
            try
            {
                string styleJS = null;

                foreach (var style in styles)
                {
                    var ind = style.IndexOf(':');
                    var name = style.Substring(0, ind);
                    var value = style.Substring(ind + 1);
                    string ss = $"arguments[0].style.{name}='{value}';";
                    styleJS += ss + "\n";
                }
                JSExecutor(styleJS, new object[] { element }, log);
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during setting styles {string.Join("\n", styles)} for element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }

        public void DeleteStyle(IWebElement element, List<string> styles, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(DeleteStyle), element, styles);
            try
            {
                string styleJS = null;

                foreach (var style in styles)
                {
                    string ss = $"arguments[0].style.{style}='';";
                    styleJS += ss + "\n";
                }
                JSExecutor(styleJS, new object[] { element }, log);
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during deleting styles {string.Join("\n", styles)} for element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }
        public void DeleteStyle(WebElement element, List<string> styles,ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(DeleteStyle), element, styles);
            try
            {
                var elem = FindElement(element, log);
                DeleteStyle(elem, styles, log);
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during deleting styles {string.Join("\n", styles)} for element: {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }


        public string GetJsValue(WebElement element, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetJsValue), element);
            try
            {
                log?.DEBUG($"Get text for element {element}");
                var el = FindElement(element, log);
                return GetJsValue(el, log);
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during getting text for element {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }
        public string GetJsValue(IWebElement element, ILogger log)
        {
            log = log?.COMMANDSTART(nameof(WebDriverManager), nameof(GetJsValue), element);
            string text = null;
            try
            {
                log?.DEBUG($"Get text for element {element}");

                var js = @"return getTextFromNode(arguments[0], arguments[1]);function getTextFromNode(e,o){void 0==o&&(o=!0);for(var r='',d=0;d<e.childNodes.length;d++){var i=e.childNodes[d];if(3===i.nodeType){var t=i.nodeValue.trim();o&&(t=' '+t),r+=t}}return r.trim()}";
                text = (string)JSExecutor(js, new object[] { element, true }, log);
                text = text.Trim();

                log?.DEBUG($"Text = '{text}'");
                return text;
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during getting text for element {element}", ex);
                TakeScreenshotOnFail(log);
                throw;
            }
        }


        public object JSExecutor(string jsScript, ILogger log)
        {
            return JSExecutor(jsScript, new object[] { }, log);
        }
        public object JSExecutor(string jsScript, WebElement element, ILogger log)
        {
            var el = FindElement(element, log);
            return JSExecutor(jsScript, el, log);
        }
        public object JSExecutor(string jsScript, IWebElement element, ILogger log)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < Config.WaitTimeout.TotalSeconds)
            {
                if (!element.Enabled)
                    Thread.Sleep(1000);
                else
                    break;
            }
            if (!element.Enabled)
                throw new Exception($"Timeout reached during waiting for element being enabled. Timeout: {Config.WaitTimeout.TotalSeconds} seconds");
            return JSExecutor(jsScript, new object[] { element }, log);
        }
        public object JSExecutor(string jsScript, object[] args, ILogger log)
        {
            log?.DEBUG($"Execute javascript: {jsScript}");
            try
            {
                var result = JavaScriptExecutor.ExecuteScript(jsScript, args);
                log?.DEBUG($"Javascript executing completed. Result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                log?.ERROR($"Error occurred during javascript execution:\n{jsScript}\nWith arguments: {args.ToString()}", ex);
                throw;
            }
        }
    }
}

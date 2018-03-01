namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using OpenQA.Selenium;
    using WebElements;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;

    public static class WebElementExtension
    {
        public static By Get(this WebLocator locator)
        {
            switch (locator.LocatorType)
            {
                case WebLocatorType.Class:
                    return By.ClassName(locator.LocatorValue);
                case WebLocatorType.Css:
                    return By.CssSelector(locator.LocatorValue);
                case WebLocatorType.Id:
                    return By.Id(locator.LocatorValue);
                case WebLocatorType.Link:
                    return By.LinkText(locator.LocatorValue);
                case WebLocatorType.Name:
                    return By.Name(locator.LocatorValue);
                case WebLocatorType.PartialLink:
                    return By.PartialLinkText(locator.LocatorValue);
                case WebLocatorType.Tag:
                    return By.TagName(locator.LocatorValue);
                case WebLocatorType.XPath:
                    return By.XPath(locator.LocatorValue ?? locator.XPath);
                default:
                    throw new NotImplementedException($"Unknown locator type: {locator.LocatorType}");
            }
        }

        public static WebElement GetCopyWithXPathReplace(this WebElement webElement, params string[] whatWithSequence)
        {
            var copy = webElement.ShallowCopyElement();
            copy.ParentElement = webElement.ParentElement;
            for (int i = 0; i < whatWithSequence.Length; i += 2)
            {
                if (i + 1 >= whatWithSequence.Length) break;
                var what = whatWithSequence[i];
                var with = whatWithSequence[i + 1];
                var regex = new Regex($"('{what}')");
                copy.Locator.XPath = regex.Replace(copy.Locator.XPath, with);
            }
            return copy;
        }
    }
}

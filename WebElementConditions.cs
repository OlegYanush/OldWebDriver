namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using OpenQA.Selenium;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public static class WebElementConditions
    {
        public static Func<IWebElement, bool> AttributeContainsText(string attribute, string text) => 
            el =>
            {
                var attr = el.GetAttribute(attribute);
                return attr?.Contains(text) ?? false;
            };
        public static Func<IWebElement, bool> HasAttributePresenceState(string attribute, bool isPresent) =>
            el =>
            {
                var attr = el.GetAttribute(attribute);
                if (isPresent && attr != null) return true;
                if (!isPresent && attr == null) return true;
                return false;
            };
    }
}

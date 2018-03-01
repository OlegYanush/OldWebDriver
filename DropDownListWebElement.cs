namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using MetaMagic;

    [MetaType("DropDownListWebElement")]
    public class DropDownListWebElement : InputWebElement
    {
        public override void MetaInit()
        {
            var option = new WebElement();
            option.Name = "Option";
            option.Description = "Option for " + Description.ToLower();
            option.Locator = new WebLocator { XPath = ".//option[toReplace]" };

            ChildWebElements.Add(option);

            base.MetaInit();
        }

        //public override string GetValue(WebDriverManager wdm, bool onlyOwnText = false)
        //{
        //    var element = wdm.FindElement(this, null);
        //    if (element.TagName == "select")
        //    {
        //        var value = wdm.JSExecutor("return $(arguments[0]).find('option:selected').text()", element, null).ToString();
        //        return value;
        //    }
        //    else return base.GetValue(wdm, onlyOwnText);
        //}
    }
}

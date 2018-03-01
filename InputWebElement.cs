namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using MetaMagic;

    [MetaType("InputWebElement")]
    public class InputWebElement : WebElement
    {
        public override string GetValue(WebDriverManager wdm, bool onlyOwnText = false)
        {
            var value = wdm.GetAttribute(this, "value", null);
            return value;
        }
    }
}

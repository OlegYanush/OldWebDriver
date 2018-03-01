namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using MetaMagic;

    [MetaType("LinkRefElement")]
    public class LinkRefWebElement : WebElement
    {
        public override string GetValue(WebDriverManager wdm, bool onlyOwnText = false)
        {
            var value = wdm.GetAttribute(this, "href", null);
            return value;
        }
    }
}

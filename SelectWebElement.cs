namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using MetaMagic;

    [MetaType("SelectWebElement")]
    public class SelectWebElement : InputWebElement
    {
        public override string GetValue(WebDriverManager wdm, bool onlyOwnText = false)
        {
            var value = wdm.JSExecutor("return $(arguments[0]).find('option:selected').text()", this, null).ToString();
            return value;
        }
    }
}

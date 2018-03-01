namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using MetaMagic;

    [MetaType("CheckboxElement")]
    public class CheckboxWebElement : InputWebElement
    {
        [MetaTypeValue("Value for select", IsRequired = false)]
        public string ValueForSelect { get; set; } = null;
        public override string GetValue(WebDriverManager wdm, bool onlyOwnText = false)
        {
            var value = wdm.GetAttribute(this, "checked", null) ?? "false";
            return bool.Parse(value).ToString();
        }
    }
}

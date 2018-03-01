namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using CommandsMagic;
    using MetaMagic;

    [MetaType("RadioButtonWebElement")]
    public class RadioButtonGroupWebElement : InputWebElement
    {
        public override string GetValue(WebDriverManager wdm, bool onlyOwnText = false)
        {
            int checkedCount = 0;
            CheckboxWebElement lastChecked = null;
            foreach (var child in this.ChildWebElements)
            {
                var radioButton = child as CheckboxWebElement;
                if (radioButton == null) continue;

                bool _checked = bool.Parse(radioButton.GetValue(wdm));
                if (_checked)
                {
                    checkedCount++;
                    lastChecked = radioButton;
                }
            }
            if (checkedCount == 0)
                return null;
            if (checkedCount == 1)
                return lastChecked.ValueForSelect;

            throw new FunctionalException("Radio button group have more than 1 selected value", null, this.ToString());
        }
    }
}

namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using MetaMagic;
    using System.Text;
    [MetaType("Frame WebElement config")]
    [MetaLocation("frame", "frameElement")]
    public class FrameWebElement : WebElement
    {
        [MetaTypeValue("Value for frame locator")]
        [MetaLocation("frameLocatorValue")]
        [MetaConstraint("FrameType", FrameLocatorType.Id, FrameLocatorType.Index)]
        public string FrameValue { get; set; } = null;

        [MetaTypeObject("Locator for FrameWebElement")]
        [MetaConstraint("FrameType", FrameLocatorType.Locator)]
        public override WebLocator Locator { get; set; } = null;

        [MetaTypeValue("Frame locator type. Id, Index or Locator. Default: Locator", IsRequired = false)]
        [MetaLocation("frameLocatorType")]
        public FrameLocatorType FrameType { get; set; } = FrameLocatorType.Locator;

        private string _toString = null;
        public override string ToString()
        {
            if (_toString != null)
                return _toString;
            var sb = new StringBuilder();
            sb.AppendLine($"FrameWebElement with Name: {Name}\nDescription: {Description}");
            switch (FrameType)
            {
                case FrameLocatorType.Id:
                case FrameLocatorType.Index:
                    sb.AppendLine($"Frame location type: {FrameType} location value: '{FrameValue}'");
                    break;
                case FrameLocatorType.Locator:
                    sb.AppendLine($"Locator: '{Locator}'");
                    break;
            }
            _toString = sb.ToString();
            sb.Clear();
            return _toString;
        }
    }
}

namespace QA.AutomatedMagic.Managers.WebDriverManager
{
    using CommandsMagic;
    using MetaMagic;
    using System.Collections.Generic;
    using System.Linq;

    [MetaType("WebElement config", keyName: nameof(Name))]
    [MetaLocation("webElement")]
    public class WebElement : BaseMetaObject
    {
      
        public WebElement ParentElement { get; set; }

        [MetaTypeCollection("List of child WebElements", IsAssignableTypesAllowed = true, IsRequired = false)]
        [MetaLocation("children")]
        public List<WebElement> ChildWebElements { get; set; } = new List<WebElement>();

        [MetaTypeObject("Locator for web element")]
        public virtual WebLocator Locator { get; set; }

        [MetaTypeValue("Name of WebElement")]
        public string Name { get; set; }

        [MetaTypeValue("Description of WebElement")]
        public string Description { get; set; }

        private string _info = null;
        public override string ToString()
        {
            if (_info != null)
                return _info;

            _info = $"Name: {Name}\nDescription: {Description}\n{Locator}";

            return _info;
        }

        public void Init()
        {
            foreach (var child in ChildWebElements)
            {
                child.ParentElement = this;
                child.Init();
            }
        }

        public WebElement this[string name]
        {
            get
            {
                var nameParts = name.Split('.');
                return this[nameParts];
            }
        }

        public WebElement this[string[] nameParts]
        {
            get
            {
                var cwe = ChildWebElements.FirstOrDefault(c => c.Name == nameParts[0]);
                if (cwe == null)
                {
                    throw new DevelopmentException($"Couldn't find child element with name: {nameParts[0]} in parent element with name: {Name}");
                }
                if (nameParts.Length == 1)
                    return cwe;
                return cwe[nameParts.Skip(1).ToArray()];
            }
        }

        public enum FrameLocatorType
        {
            Id,
            Index,
            Locator
        }

        public virtual string GetValue(WebDriverManager wdm, bool onlyOwnText = false)
        {
            return wdm.GetValue(this, onlyOwnText, null);
        }

        public WebElement ShallowCopyElement => new WebElement
        {
            Name = this.Name,
            Description = this.Description,
            Locator = new WebLocator
            {
                IsRelative = this.Locator.IsRelative,
                LocatorType = this.Locator.LocatorType,
                LocatorValue = this.Locator.LocatorValue,
                XPath = this.Locator.XPath
            }
        };
    }
}

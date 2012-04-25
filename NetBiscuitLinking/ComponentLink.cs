using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using Tridion.ContentDelivery.Web.Linking;
using Tridion.ContentDelivery.Web.Utilities;
using tridion = Tridion.ContentDelivery.Web.UI;
using tridionLinking = Tridion.ContentDelivery.Web.Linking;

namespace NetBiscuitLinking
{
    /// <summary>
    /// Extends Tridion.ContentDelivery.Web.UI.ComponentLink,
    /// handles dynamic linking for BML-markup
    /// </summary>
    [ParseChildren(true, "OutputMarkup")]
    public class ComponentLink : tridion.ComponentLink
    {
        [DefaultValue("None")]
        [Bindable(true)]
        [Category("Appearance")]
        public string NetBiscuitLinkType { get; set; }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public string OutputMarkup { get; set; }

        private BMLLinkType biscuitLinkType;

        private const string TCM_URI_PLACE_HOLDER = @"{0}";
        private const string BML_URL_MARKUP = @"[url=""{0}""]{1}[/url]";
        private const string BML_URLNOFOLLOW_MARKUP = @"[urlnofollow=""{0}""]{1}[/urlnofollow]";
        private const string BML_LINK_MARKUP = @"<link href=""{0}"">{1}</link>";
        private const string BML_CELL_MARKUP = @"<cell href=""{0}"">{1}</cell>";

        protected override void OnInit(EventArgs e)
        {
            // for .Net 4.0 
            if(!Enum.TryParse(NetBiscuitLinkType, true, out biscuitLinkType))
            {
                biscuitLinkType = BMLLinkType.None;
            
            } 

            // for .NET 3.5
            //biscuitLinkType = (BMLLinkType)Enum.Parse(typeof(BMLLinkType), NetBiscuitLinkType, true);

            base.OnInit(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string markup = GetMarkup();

            if (biscuitLinkType != BMLLinkType.None && !string.IsNullOrEmpty(markup))
            {
                writer.Write(markup);
            }
            else
            {
                base.Render(writer);
            }
        }

        private string GetMarkup()
        {
            string url = GetHref();

            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(OutputMarkup))
            {
                return GetOutputMarkup(url);
            }

            return GetBMLLink(url);
        }

        private string GetOutputMarkup(string url)
        {
            return OutputMarkup.Replace(TCM_URI_PLACE_HOLDER, url);
        }

        private string GetBMLLink(string url)
        {
            return String.Format(GetBMLLinkMarkUp(), url, LinkText);
        }

        private string GetHref()
        {
            string url = string.Empty;
            if ((HttpContext.Current != null))
            {
                using (var componentLink = new tridionLinking.ComponentLink(new TcmUri(ComponentUri).PublicationId))
                {
                    url = ResolveComponentLink(componentLink);
                }
            }
            return url;
        }

        private string GetBMLLinkMarkUp()
        {
            switch (biscuitLinkType)
            {
                case BMLLinkType.Url:
                    return BML_URL_MARKUP;
                case BMLLinkType.UrlNoFollow:
                    return BML_URLNOFOLLOW_MARKUP;
                case BMLLinkType.Link:
                    return BML_LINK_MARKUP;
                case BMLLinkType.Cell:
                    return BML_CELL_MARKUP;
                default:
                    throw new ArgumentException("Unknown NetBiscuitLinkType: " + biscuitLinkType, "NetBiscuitLinkType");
            }
        }

        private string ResolveComponentLink(tridionLinking.ComponentLink componentLink)
        {
            Link link = componentLink.GetLink(ComponentUri);
            if (link.IsResolved)
            {
                return link.Url;
            }
            return string.Empty;
        }
    }

    public enum BMLLinkType
    {
        None,
        Url,
        UrlNoFollow,
        Link,
        Cell
    }
}

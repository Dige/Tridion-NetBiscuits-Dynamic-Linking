using System.Text.RegularExpressions;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;

namespace NetBiscuitTemplating
{
    /// <summary>
    /// Converts NetBiscuits markup to a custom ComponentLink control that resolves tcm uris into urls.     
    /// This only applies to the following NetBiscuits markup: BML link, BBCode url and BBcode urlnofollow.
    /// In the front-end the BBCode and BML markup is put back with the links at request time.
    /// </summary>
    [TcmTemplateTitle("Convert BBCode Links to Tridion Links")]
    public class ConvertBBCodeAndBMLLinksToTridionLinks : TemplateBase
    {
        private bool linksFound;

        private const string TCM_URI_PLACE_HOLDER = @"{0}";
        private const string NET_BISCUIT_CELL_PATTERN = @"(<cell(\s*text-align=""\w+"")?(\s*align=""\w+"")?(\s*width=""\d{1,3}[%]"")?\s*href=""(?<uri>tcm:\d{1,6}-\d{1,6}(-\d{1,6})?)"">(\s+)?(\r+)?.*(\s+)?(\r+)?<\s*?/\s*?cell\s*?>)";
        private const string NET_BISCUIT_LINK_PATTERN = @"<link\s*href=""(?<uri>tcm:\d{1,6}-\d{1,6}(-\d{1,6})?)"">(?<linktext>.*?)<\s*?/\s*?link\s*?>";
        private const string BBCODE_URL_NO_FOLLOW_PATTERN = @"\[(urlnofollow)(=""(?<uri>tcm:\d{1,6}-\d{1,6}(-\d{1,6})?)"")?\](?<linktext>.*?)\[\/(urlnofollow)\]"; // e.g. [urlnofollow="tcm:1-2-3"]Link Text[/urlnofollow]                                               
        private const string BBCODE_URL_PATTERN = @"\[(url)(=""(?<uri>tcm:\d{1,6}-\d{1,6}(-\d{1,6})?)"")?\](?<linktext>.*?)\[\/(url)\]"; // e.g. [url="tcm:1-2-3"]Link Text[/url]                                               
        private const string NET_BISCUIT_COMPONENT_LINK_CONTROL = "<tridion:ComponentLink ComponentURI=\"{0}\" NetBiscuitLinkType=\"{1}\" LinkText=\"{2}\" runat=\"server\"><OutputMarkup>{3}</OutputMarkup></tridion:ComponentLink>";

        private enum NetBiscuitLinkType
        {   
            Url,
            UrlNoFollow,
            Link,
            Cell
        };

        public override void Transform(Engine engine, Package package)
        {
            Initialize(engine, package);

            Item outputItem = GetOutputItem();
            if (outputItem == null)
            {
                Logger.Error("Package doesn't contain Output-item");
                return;
            }
            
            UpdateOutputItemIfLinksFound(outputItem, ReplaceBMLMarkupWithNetBiscuitComponentLink(outputItem.GetAsString()));
        }

        private string ReplaceBMLMarkupWithNetBiscuitComponentLink(string output)
        {
            string newOutput = output;
            newOutput = ReplaceLinksOfType(NetBiscuitLinkType.Url, newOutput);
            newOutput = ReplaceLinksOfType(NetBiscuitLinkType.UrlNoFollow, newOutput);
            newOutput = ReplaceLinksOfType(NetBiscuitLinkType.Link, newOutput);
            newOutput = ReplaceLinksOfType(NetBiscuitLinkType.Cell, newOutput);
            return newOutput;
        }

        private string ReplaceLinksOfType(NetBiscuitLinkType type, string output)
        {
            return Regex.Replace(output, GetRegExForType(type), new MatchEvaluator((match) =>
            {
                linksFound = true;
                return ConvertToComponentLinkTag(match, type);
            }));
        }

        private void UpdateOutputItemIfLinksFound(Item outputItem, string content)
        {
            if (linksFound)
            {
                UpdateItem(outputItem, content);
            }
        }

        private static string ConvertToComponentLinkTag(Match match, NetBiscuitLinkType type)
        {
            string outputMarkup = (type == NetBiscuitLinkType.Cell) ? ReplaceURIWithPlaceHolder(match) : string.Empty;

            return CreateComponentLinkTag(GetTcmUri(match), type, GetLinkText(match), outputMarkup);
        }

        private static string CreateComponentLinkTag(string tcmUri, NetBiscuitLinkType type, string linkText, string outputMarkup)
        {
            return string.Format(NET_BISCUIT_COMPONENT_LINK_CONTROL, tcmUri, type, linkText, outputMarkup);
        }

        private static string GetTcmUri(Match match)
        {
            return match.Groups["uri"].Value;
        }

        private static string GetLinkText(Match match)
        {
            return match.Groups["linktext"].Value;
        }

        private static string ReplaceURIWithPlaceHolder(Match match)
        {
            return match.Value.Replace(match.Groups["uri"].Value, TCM_URI_PLACE_HOLDER);
        }

        private static string GetRegExForType(NetBiscuitLinkType type)
        {
            switch (type)
            {
                case NetBiscuitLinkType.Link:
                    return NET_BISCUIT_LINK_PATTERN;
                case NetBiscuitLinkType.Url:
                    return BBCODE_URL_PATTERN;
                case NetBiscuitLinkType.UrlNoFollow:
                    return BBCODE_URL_NO_FOLLOW_PATTERN;
                case NetBiscuitLinkType.Cell:
                    return NET_BISCUIT_CELL_PATTERN;
                default:
                    return string.Empty;
            }
        }

        private Item GetOutputItem()
        {
            return _package.GetByName(Package.OutputName);
        }

        protected void UpdateItem(Item item, string updatedContent)
        {
            _package.Remove(item);
            item.SetAsString(updatedContent);
            _package.PushItem(Package.OutputName, item);
        }
    }
}

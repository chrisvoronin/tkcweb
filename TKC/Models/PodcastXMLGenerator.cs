using System.Xml;

namespace TKC.Models
{
    public class PodcastXMLGenerator
    {
        public XmlDocument GenerateXml(List<Sermon> sermons, List<ShortTake> exhortations)
        {
            var doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(xmlDeclaration);

            XmlElement rss = doc.CreateElement("rss");
            rss.SetAttribute("version", "2.0");
            rss.SetAttribute("xmlns:itunes", "http://www.itunes.com/dtds/podcast-1.0.dtd");
            rss.SetAttribute("xmlns:content", "http://purl.org/rss/1.0/modules/content/");
            rss.SetAttribute("xml:lang", "en-US");
            doc.AppendChild(rss);

            XmlElement channel = doc.CreateElement("channel");
            rss.AppendChild(channel);

            // Add channel elements
            AddElement(doc, channel, "title", "The Kings Congregation");
            AddElement(doc, channel, "link", "https://thekingscongregation.com");
            AddElement(doc, channel, "language", "en-us");
            AddElement(doc, channel, "copyright", $"Copyright © {DateTime.Now.Year}");
            AddElementWithPrefix(doc, channel, "author", "itunes", "The King's Congregation");
            AddElement(doc, channel, "description", "Sermons, exhortations and teaching from The King's Congregation, Meridian, Idaho.");
            AddElementWithPrefix(doc, channel, "image", "itunes", "", new Dictionary<string, string> { { "href", "https://thekingscongregation.com/images/itunes-cover-art-sermons.png" } });

            // add category to channel
            XmlElement category = doc.CreateElement("itunes", "category", "http://www.itunes.com/dtds/podcast-1.0.dtd");
            category.SetAttribute("text", "Religion & Spirituality");
            XmlElement subCategory = doc.CreateElement("itunes", "category", "http://www.itunes.com/dtds/podcast-1.0.dtd");
            subCategory.SetAttribute("text", "Christianity");
            category.AppendChild(subCategory);
            channel.AppendChild(category);

            AddElementWithPrefix(doc, channel, "explicit", "itunes", "false");
            AddElement(doc, channel, "link", "https://thekingscongregation.com/podcasts/sermons");

            XmlElement owner = doc.CreateElement("itunes", "owner", "http://www.itunes.com/dtds/podcast-1.0.dtd");
            AddElementWithPrefix(doc, owner, "name", "itunes", "The King's Congregation");
            AddElementWithPrefix(doc, owner, "email", "itunes", "office@thekingscongregation.com");
            channel.AppendChild(owner);

            // Add sermons
            foreach (var sermon in sermons)
            {
                if (sermon.AudioDuration == null ||  string.IsNullOrWhiteSpace(sermon.AudioUrl) || sermon.AudioFileSize == null)
                    continue;

                XmlElement item = doc.CreateElement("item");
                channel.AppendChild(item);
                AddElement(doc, item, "title", sermon.Title ?? "");
                AddElement(doc, item, "enclosure", "", new Dictionary<string, string> {
                    { "url", ProperAudioUrl(sermon.AudioUrl!) }
                    , { "length", sermon.AudioFileSize!.ToString() }
                    , { "type", "audio/mpeg" } });
                //AddElement(doc, item, "guid", sermon.Id.ToString()); //TODO: either GUID, or dont send at all
                AddElement(doc, item, "pubDate", sermon.DateCreated.ToUniversalTime().ToString("R"));
                AddElement(doc, item, "description", $"Sermon text: {sermon.SubTitle}; Speaker: {sermon.Author!}. Visit thekingscongregation.com for more sermons, exhortations, music, and events.");
                AddElementWithPrefix(doc, item, "duration", "itunes", sermon.AudioDuration!.ToString());
                string itemUri = "https://thekingscongregation.com/sermon/" + sermon.Id;
                AddElement(doc, item, "link", itemUri);
                AddElementWithPrefix(doc, channel, "explicit", "itunes", "false");
                AddElementWithPrefix(doc, item, "author", "itunes", sermon.Author ?? "");
                AddElementWithPrefix(doc, item, "keywords", "itunes", "sermon, exhortation, Bible teaching, Christian, Christian worship");
            }

            // Add exhortations
            foreach (var exh in exhortations)
            {
                if (exh.AudioDuration == null || string.IsNullOrWhiteSpace(exh.AudioUrl) || exh.AudioFileSize == null)
                    continue;

                XmlElement item = doc.CreateElement("item");
                channel.AppendChild(item);
                AddElement(doc, item, "title", $"ShortTake: {exh.Title}");
                AddElementWithPrefix(doc, item, "author", "itunes", exh.Author!);
                AddElement(doc, item, "description", "Visit thekingscongregation.com for more sermons, exhortations, music, and events.");
                AddElement(doc, item, "enclosure", "", new Dictionary<string, string> {
                    { "url", ProperAudioUrl(exh.AudioUrl!) }
                    , { "length", exh.AudioFileSize!.ToString() }
                    , { "type", "audio/mpeg" } });

                //AddElement(doc, item, "guid", exh.Id.ToString()); //TODO
                string itemUri = "https://thekingscongregation.com/shorttake/" + exh.Id;
                AddElement(doc, item, "link", itemUri);
                AddElement(doc, item, "pubDate", exh.DateCreated.ToUniversalTime().ToString("R"));
                AddElementWithPrefix(doc, item, "duration", "itunes", exh.AudioDuration!.ToString());
                AddElementWithPrefix(doc, item, "keywords", "itunes", "sermon, exhortation, Bible teaching, Christian, Christian worship");
            }

            return doc;
        }

        private string ProperAudioUrl(string AudioUrl)
        {
            if (AudioUrl.StartsWith("http"))
            {
                return AudioUrl;
            }
            return "https://thekingscongregation.com/File/" + AudioUrl;
        }

        private void AddElementWithPrefix(XmlDocument doc, XmlElement parent, string elementName, string prefix, string innerText = "", Dictionary<string, string> attributes = null)
        {
            string namespaceUri = doc.DocumentElement.GetNamespaceOfPrefix(prefix);

            var element = doc.CreateElement(prefix, elementName, namespaceUri);

            if (!string.IsNullOrEmpty(innerText))
                element.InnerText = innerText;

            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    element.SetAttribute(attribute.Key, attribute.Value);
                }
            }

            parent.AppendChild(element);
        }

        private void AddElement(XmlDocument doc, XmlElement parent, string elementName, string innerText = "", Dictionary<string, string> attributes = null)
        {
            var element = doc.CreateElement(elementName);

            if (!string.IsNullOrEmpty(innerText))
                element.InnerText = innerText;

            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    element.SetAttribute(attribute.Key, attribute.Value);
                }
            }

            parent.AppendChild(element);
        }
    }

}


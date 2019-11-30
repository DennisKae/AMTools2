using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Data.Files.Repositories
{
    public class SubscriberFileRepository : FileImportRepositoryBase
    {
        private readonly string _subscriberFilePath;

        public SubscriberFileRepository(string subscriberFilePath)
        {
            _subscriberFilePath = subscriberFilePath;
        }

        public List<Subscriber> GetAllSubscribers()
        {
            var result = new List<Subscriber>();

            if (!FileExistsAndIsNotEmpty(_subscriberFilePath))
            {
                return result;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(_subscriberFilePath);
            XmlNode rootNode = xmlDocument.DocumentElement;

            XmlNodeList subscriberNodes = rootNode.SelectNodes("subscriber");
            if (subscriberNodes.Count == 0)
            {
                subscriberNodes = rootNode.SelectNodes("Subscriber");
            }

            foreach (XmlNode subscriberNode in subscriberNodes)
            {
                string nodeValue = subscriberNode?.InnerText;
                if (string.IsNullOrWhiteSpace(nodeValue) || !nodeValue.Contains('|'))
                {
                    continue;
                }
                string[] firstSplit = nodeValue.Split('|');

                string[] secondSplit = firstSplit[0].Split('=');

                var newSubscriber = new Subscriber();
                newSubscriber.Issi = secondSplit[0].Trim();
                if (secondSplit.Length > 1)
                {
                    newSubscriber.Name = secondSplit[1].Trim();
                }

                if (firstSplit.Length > 1)
                {
                    var thirdSplit = firstSplit[1].Split('=');
                    if (thirdSplit.Length > 1)
                    {
                        newSubscriber.Qualification = thirdSplit[1].Trim();
                    }
                }

                result.Add(newSubscriber);
            }

            return result;
        }
    }
}

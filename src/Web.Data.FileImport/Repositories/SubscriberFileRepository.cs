using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Data.Files.Repositories.Interfaces;

namespace AMTools.Web.Data.Files.Repositories
{
    public class SubscriberFileRepository : FileImportRepositoryBase, ISubscriberFileRepository
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public SubscriberFileRepository(
            ILogService logService,
            IConfigurationFileRepository configurationFileRepository
            ) : base(logService)
        {
            _configurationFileRepository = configurationFileRepository;
        }

        public List<Subscriber> GetAll()
        {
            var result = new List<Subscriber>();
            DateiKonfiguration dateiKonfig = _configurationFileRepository?.GetConfigFromJsonFile<DateiKonfiguration>();

            if (!FileExistsAndIsNotEmpty(nameof(dateiKonfig.SubscriberDatei), dateiKonfig?.SubscriberDatei))
            {
                return result;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(dateiKonfig.SubscriberDatei);
            XmlNode rootNode = xmlDocument.DocumentElement;

            XmlNodeList subscriberNodes = rootNode.SelectNodes("subscriber");
            if (subscriberNodes.Count == 0)
            {
                subscriberNodes = rootNode.SelectNodes("Subscriber");
            }

            int counter = 0;
            foreach (XmlNode subscriberNode in subscriberNodes)
            {
                counter++;
                string nodeValue = subscriberNode?.InnerText;
                if (string.IsNullOrWhiteSpace(nodeValue) || !nodeValue.Contains('|'))
                {
                    continue;
                }
                string[] firstSplit = nodeValue.Split('|');

                string[] secondSplit = firstSplit[0].Split('=');

                var newSubscriber = new Subscriber();
                newSubscriber.Reihenfolge = counter;
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

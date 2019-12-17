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
using AMTools.Web.Data.Files.Models.Callout;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;

namespace AMTools.Web.Data.Files.Repositories
{
    public class AvailabilityFileRepository : FileImportRepositoryBase, IAvailabilityFileRepository
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IMapper _mapper;

        public AvailabilityFileRepository(
            ILogService logService,
            IConfigurationFileRepository configurationFileRepository,
            IMapper mapper) : base(logService)
        {
            _configurationFileRepository = configurationFileRepository;
            _mapper = mapper;
        }

        private AvailabilityImportModel GetAvailabilityFromXmlNode(XmlNode availabilityNode)
        {
            if (availabilityNode == null)
            {
                return null;
            }

            var result = new AvailabilityImportModel();
            result.Issi = availabilityNode.SelectSingleNode("ISSI//text()")?.Value;
            result.Value = int.TryParse(availabilityNode.SelectSingleNode("Availability//text()")?.Value, out int parsedNumber) ? parsedNumber : default;
            result.Timestamp = DateTime.TryParse(availabilityNode.SelectSingleNode("Timestamp//text()")?.Value, out DateTime parsedTimestamp) ? parsedTimestamp : default;

            return result;
        }

        public AvailabilityStatus GetAvailabilityByIssi(string issi)
        {
            DateiKonfiguration dateiKonfig = _configurationFileRepository?.GetConfigFromJsonFile<DateiKonfiguration>();
            if (string.IsNullOrWhiteSpace(issi) || !FileExistsAndIsNotEmpty(nameof(dateiKonfig.AvailabilityDatei), dateiKonfig?.AvailabilityDatei))
            {
                return null;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(dateiKonfig.AvailabilityDatei);
            XmlNode rootNode = xmlDocument.DocumentElement;

            XmlNode availabilityNode = rootNode.SelectSingleNode($"/Subscribers/Subscriber[ISSI='{issi}']");
            return _mapper.Map<AvailabilityStatus>(GetAvailabilityFromXmlNode(availabilityNode));
        }

        public List<AvailabilityStatus> GetAllAvailabilities()
        {
            DateiKonfiguration dateiKonfig = _configurationFileRepository?.GetConfigFromJsonFile<DateiKonfiguration>();
            var result = new List<AvailabilityImportModel>();
            if (!FileExistsAndIsNotEmpty(nameof(dateiKonfig.AvailabilityDatei), dateiKonfig?.AvailabilityDatei))
            {
                return _mapper.Map<List<AvailabilityStatus>>(result);
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(dateiKonfig.AvailabilityDatei);
            XmlNode rootNode = xmlDocument.DocumentElement;

            XmlNodeList availabilityNodes = rootNode.SelectNodes("Subscriber");

            foreach (XmlNode availabilityNode in availabilityNodes)
            {
                AvailabilityImportModel newAvailability = GetAvailabilityFromXmlNode(availabilityNode);
                if (newAvailability != null)
                {
                    result.Add(newAvailability);
                }
            }

            return _mapper.Map<List<AvailabilityStatus>>(result);
        }
    }
}

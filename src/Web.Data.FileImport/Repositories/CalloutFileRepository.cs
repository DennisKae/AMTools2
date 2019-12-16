﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Files.Models.Callout;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;

namespace AMTools.Web.Data.Files.Repositories
{
    public class CalloutFileRepository : FileImportRepositoryBase, ICalloutFileRepository
    {
        private readonly string _calloutFilePath;
        private readonly IMapper _mapper;

        public CalloutFileRepository(
            string calloutFilePath,
            IMapper mapper
            )
        {
            _calloutFilePath = calloutFilePath;
            _mapper = mapper;
        }

        public List<AlertIdentification> GetAllAlertIds()
        {
            var result = new List<AlertIdentification>();
            if (!FileExistsAndIsNotEmpty(_calloutFilePath))
            {
                return result;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(_calloutFilePath);
            XmlNode rootNode = xmlDocument.DocumentElement;

            XmlNodeList alertNodes = rootNode.SelectNodes("Alert");
            foreach (XmlNode alertNode in alertNodes)
            {
                var newAlert = new AlertIdentification
                {
                    Number = int.TryParse(alertNode.SelectSingleNode("Number//text()")?.Value, out int parsedNumber) ? parsedNumber : default,
                    Timestamp = DateTime.TryParse(alertNode.SelectSingleNode("Timestamp//text()")?.Value, out DateTime parsedTimestamp) ? parsedTimestamp : default
                };
                result.Add(newAlert);
            }

            return result;
        }

        private XmlNode GetAlertNodeById(AlertIdentification alertIdentification)
        {
            if (alertIdentification == null || !FileExistsAndIsNotEmpty(_calloutFilePath))
            {
                return null;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(_calloutFilePath);
            XmlNode rootNode = xmlDocument.DocumentElement;
            string alertTimestmap = GetTimestampFromDateTime(alertIdentification.Timestamp);
            XmlNode alertNode = rootNode.SelectSingleNode($"/Alerts/Alert[Number='{alertIdentification.Number}' and Timestamp='{alertTimestmap}']");

            return alertNode;
        }

        public Alert GetAlert(AlertIdentification alertIdentification)
        {
            XmlNode alertNode = GetAlertNodeById(alertIdentification);

            if (alertNode == null)
            {
                return null;
            }

            var result = new AlertImportModel
            {
                Number = int.TryParse(alertNode.SelectSingleNode("Number//text()")?.Value, out int parsedNumber) ? parsedNumber : default,
                Timestamp = DateTime.TryParse(alertNode.SelectSingleNode("Timestamp//text()")?.Value, out DateTime parsedTimestamp) ? parsedTimestamp : default,
                Text = alertNode.SelectSingleNode("Text/text()")?.Value,
                AlertedSubscribers = alertNode.SelectSingleNode("AlertedSubscribers/text()")?.Value,
                Xml = alertNode.OuterXml
            };
            result.AlertTimestamp = GetDateTimeFromAlertTimestamp(alertNode.SelectSingleNode("AlertTimestamp//text()")?.Value, result.Timestamp);

            return _mapper.Map<Alert>(result);
        }

        public List<UserResponse> GetUserResponses(AlertIdentification alertIdentification)
        {
            var result = new List<AlertUserResponseImportModel>();
            if (alertIdentification == null)
            {
                return _mapper.Map<List<UserResponse>>(result);
            }

            XmlNode alertNode = GetAlertNodeById(alertIdentification);

            if (alertNode == null)
            {
                return _mapper.Map<List<UserResponse>>(result);
            }

            XmlNodeList userResponseNodes = alertNode.SelectNodes("UserResponses//UserResponse");
            foreach (XmlNode userResponseNode in userResponseNodes)
            {
                var newResponse = new AlertUserResponseImportModel
                {
                    Issi = userResponseNode.SelectSingleNode("ISSI//text()")?.Value,
                    Accept = bool.TryParse(userResponseNode.SelectSingleNode("Accept//text()")?.Value, out bool parsedAccept) ? parsedAccept : default,
                    Color = userResponseNode.SelectSingleNode("Color//text()")?.Value,
                    Response = userResponseNode.SelectSingleNode("Response//text()")?.Value
                };

                if (!string.IsNullOrWhiteSpace(newResponse.Response) && newResponse.Response.Contains('-'))
                {
                    newResponse.Timestamp = GetDateTimeFromAlertTimestamp(newResponse.Response.Split('-')[0], alertIdentification.Timestamp);
                }

                result.Add(newResponse);
            }


            return _mapper.Map<List<UserResponse>>(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;

namespace AMTools.Web.Data.Files.Repositories
{
    public class SettingsFileRepository : FileImportRepositoryBase, ISettingsFileRepository
    {
        private readonly string _settingsFilePath;
        private readonly IMapper _mapper;

        public SettingsFileRepository(
            string settingsFilePath,
            IMapper mapper
            )
        {
            _settingsFilePath = settingsFilePath;
            _mapper = mapper;
        }

        private Setting GetImportModelFromValue(string value, string settingName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            var result = new Setting { Name = settingName };
            bool valueHasId = value.Contains('=');
            if (valueHasId)
            {
                string[] splittedString = value.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedString.Length > 0)
                {
                    result.Key = splittedString[0];
                }

                if (splittedString.Length > 1)
                {
                    result.Value = splittedString[1];
                }
            }

            bool valueHasValueAndColor = value.Contains('|');
            if (valueHasValueAndColor)
            {
                value = value.Replace(result.Key + "=", string.Empty);

                string[] splittedString = value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedString.Length > 0)
                {
                    result.Value = splittedString[0];
                }

                if (splittedString.Length > 1)
                {
                    result.Color = splittedString[1];
                }
            }

            if (!valueHasId && !valueHasValueAndColor)
            {
                result.Value = value;
            }

            return result;
        }

        public List<Setting> GetSetting(string settingName)
        {
            var result = new List<Setting>();
            if (string.IsNullOrWhiteSpace(settingName) || !FileExistsAndIsNotEmpty(_settingsFilePath))
            {
                return result;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(_settingsFilePath);
            XmlNode rootNode = xmlDocument.DocumentElement;

            XmlNode settingNode = rootNode.SelectSingleNode($"/settings/setting[@name='{settingName}']");
            if (settingNode == null)
            {
                return result;
            }

            XmlNodeList valueNodes = settingNode.SelectNodes("value");
            if (valueNodes == null || valueNodes.Count == 0)
            {
                return result;
            }

            foreach (XmlNode valueNode in valueNodes)
            {
                Setting importModel = GetImportModelFromValue(valueNode.InnerText, settingName);
                if (importModel != null)
                {
                    result.Add(importModel);
                }
            }

            return result;
        }

        public List<Setting> GetAllSettings()
        {
            var result = new List<Setting>();

            List<string> settingNames = typeof(SettingNames).GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => x.GetValue(null) as string).ToList();

            if (settingNames == null || settingNames.Count == 0)
            {
                return result;
            }

            foreach (string settingName in settingNames)
            {
                List<Setting> newSettings = GetSetting(settingName);
                if (newSettings.Count > 0)
                {
                    result.AddRange(newSettings);
                }
            }

            return result;
        }
    }
}

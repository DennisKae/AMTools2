using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Data.JsonStore.Models;
using Newtonsoft.Json;
using RestSharp;

namespace AMTools.Web.Data.JsonStore.Repositories
{
    public class AvailabilityStatusJsonStoreRepository : IAvailabilityJsonStoreRepository
    {
        private readonly ILogService _logService;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly string _baseUrl = "https://www.jsonstore.io/";


        public AvailabilityStatusJsonStoreRepository(ILogService logService, IConfigurationFileRepository configurationFileRepository)
        {
            _logService = logService;
            _configurationFileRepository = configurationFileRepository;
        }

        public void CreateOrUpdate(AvailabilityStorageItem availabilityStorageItem)
        {
            if (availabilityStorageItem == null)
            {
                return;
            }

            JsonStoreKonfiguration config = _configurationFileRepository.GetConfigFromJsonFile<JsonStoreKonfiguration>();
            string url = _baseUrl + config.Key;

            var client = new RestClient(url);
            client.UseSerializer<JsonNetSerializer>();

            var request = new RestRequest($"Availabilities/{availabilityStorageItem.SubscriberId}/", DataFormat.Json);
            request.AddJsonBody(availabilityStorageItem);
            IRestResponse httpResponse = client.Post(request);
            LogResponse(httpResponse);
        }

        public void Delete(int subscriberId)
        {
            JsonStoreKonfiguration config = _configurationFileRepository.GetConfigFromJsonFile<JsonStoreKonfiguration>();
            string url = _baseUrl + config.Key;

            var client = new RestClient(url);
            client.UseSerializer<JsonNetSerializer>();

            var request = new RestRequest($"Availabilities/{subscriberId}");
            IRestResponse httpResponse = client.Delete(request);
            LogResponse(httpResponse);
        }

        public void DeleteAll()
        {
            JsonStoreKonfiguration config = _configurationFileRepository.GetConfigFromJsonFile<JsonStoreKonfiguration>();
            string url = _baseUrl + config.Key;

            var client = new RestClient(url);
            client.UseSerializer<JsonNetSerializer>();

            var request = new RestRequest("Availabilities");
            IRestResponse httpResponse = client.Delete(request);
            LogResponse(httpResponse);
        }

        public List<AvailabilityStorageItem> GetAll()
        {
            JsonStoreKonfiguration config = _configurationFileRepository.GetConfigFromJsonFile<JsonStoreKonfiguration>();
            string url = _baseUrl + config.Key;

            var client = new RestClient(url);
            client.UseSerializer<JsonNetSerializer>();

            var request = new RestRequest("Availabilities", Method.GET);
            IRestResponse httpResponse = client.Get(request);
            LogResponse(httpResponse);
            string responseContent = httpResponse.Content;

            JsonStoreListResponse<AvailabilityStorageItem> formattedResponse = JsonConvert.DeserializeObject<JsonStoreListResponse<AvailabilityStorageItem>>(responseContent);

            var emptyResult = new List<AvailabilityStorageItem>();
            if (formattedResponse?.Result == null || formattedResponse.Result.Count == 0)
            {
                return emptyResult;
            }
            return formattedResponse?.Result.Where(x => x != null).ToList() ?? emptyResult;
        }

        public AvailabilityStorageItem GetBySubscriberId(int subscriberId)
        {
            JsonStoreKonfiguration config = _configurationFileRepository.GetConfigFromJsonFile<JsonStoreKonfiguration>();
            string url = $"{_baseUrl}/{config.Key}/Availabilities/{subscriberId}";

            var client = new RestClient(url);
            client.UseSerializer<JsonNetSerializer>();

            var request = new RestRequest(Method.GET);
            IRestResponse httpResponse = client.Get(request);
            LogResponse(httpResponse);
            string responseContent = httpResponse.Content;

            JsonStoreResponse<AvailabilityStorageItem> formattedResponse = JsonConvert.DeserializeObject<JsonStoreResponse<AvailabilityStorageItem>>(responseContent);
            return formattedResponse?.Result;
        }

        private void LogResponse(IRestResponse restResponse)
        {
            if (restResponse == null || restResponse.IsSuccessful)
            {
                return;
            }

            string methodText = restResponse.Request.Method.ToString().ToUpper();
            string requestedUrl = restResponse.ResponseUri.ToString();
            _logService.Error(GetType().Name + $" - {methodText} {requestedUrl}: HTTP {(int)restResponse.StatusCode} - {restResponse.StatusDescription} response. Response Body: " + Environment.NewLine + restResponse.Content ?? "- Kein Body vorhanden -");
        }
    }
}

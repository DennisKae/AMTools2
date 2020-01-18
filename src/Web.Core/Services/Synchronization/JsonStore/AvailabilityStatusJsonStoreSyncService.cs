using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Synchronization.JsonStore.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.JsonStore.Models;
using AMTools.Web.Data.JsonStore.Repositories;
using AMTools.Web.Data.JsonStore.Services.Interfaces;

namespace AMTools.Web.Core.Services.Synchronization.JsonStore
{
    public class AvailabilityStatusJsonStoreSyncService : IAvailabilityStatusJsonStoreSyncService
    {
        private readonly IAvailabilityJsonStoreRepository _availabilityJsonStoreRepository;
        private readonly ISubscriberService _subscriberService;
        private readonly IJsonStoreService _jsonStoreService;
        private readonly ILogService _logService;

        public AvailabilityStatusJsonStoreSyncService(
            IAvailabilityJsonStoreRepository availabilityJsonStoreRepository,
            ISubscriberService subscriberService,
            IJsonStoreService jsonStoreService,
            ILogService logService)
        {
            _availabilityJsonStoreRepository = availabilityJsonStoreRepository;
            _subscriberService = subscriberService;
            _jsonStoreService = jsonStoreService;
            _logService = logService;
        }

        public void Sync()
        {
            if (!_jsonStoreService.ConfigIsValid())
            {
                return;
            }
            WriteLogInfo("Sync gestartet...");

            List<SubscriberViewModel> allSubscribers = _subscriberService.GetAll();
            List<AvailabilityStorageItem> allJsonStoreItems = _availabilityJsonStoreRepository.GetAll();

            if (allSubscribers == null || allSubscribers.Count == 0)
            {
                if (allJsonStoreItems?.Count > 0)
                {
                    _availabilityJsonStoreRepository.DeleteAll();
                }
                WriteLogInfo("Sync beendet...");
                return;
            }

            if (allJsonStoreItems == null || allJsonStoreItems.Count == 0)
            {
                WriteLogInfo("Der JsonStore ist leer und wird nun komplett gefüllt...");
                allSubscribers.ForEach(x => _availabilityJsonStoreRepository.CreateOrUpdate(GetStorageItemFromViewModel(x)));
                WriteLogInfo("Sync beendet...");
                return;
            }

            // Einzelne Elemente müssen ggf. in den JsonStore eingefügt/aktualisiert werden.
            foreach (SubscriberViewModel subscriberViewModel in allSubscribers)
            {
                AvailabilityStorageItem existingJsonStoreItem = allJsonStoreItems?.FirstOrDefault(x => x.SubscriberId == subscriberViewModel.Id);
                // Existierte noch nicht oder muss aktualisiert werden
                if (existingJsonStoreItem == null || !ItemsAreEqual(subscriberViewModel, existingJsonStoreItem))
                {
                    _availabilityJsonStoreRepository.CreateOrUpdate(GetStorageItemFromViewModel(subscriberViewModel));
                }
            }


            // Obsolete JsonStore-Elemente entfernen
            foreach (var jsonStoreItem in allJsonStoreItems)
            {
                if (!allSubscribers.Any(x => x.Id == jsonStoreItem.SubscriberId))
                {
                    WriteLogInfo($"Das {nameof(AvailabilityStorageItem)} mit der Id {jsonStoreItem.SubscriberId} ist obsolet und wird gelöscht.");
                    _availabilityJsonStoreRepository.Delete(jsonStoreItem.SubscriberId);
                }
            }
            WriteLogInfo("Sync beendet...");
        }

        private void WriteLogInfo(string message) => _logService.Info(GetType().Name + $": {message}");

        private bool ItemsAreEqual(SubscriberViewModel source, AvailabilityStorageItem target)
        {
            if (source == null && target != null || source != null & target == null)
            {
                return false;
            }

            return source?.Id == target?.SubscriberId &&
                source?.AvailabilityStatus?.Setting?.Nummer == target?.AvailabilityKey &&
                source?.AvailabilityStatus?.Timestamp == target?.Timestamp;
        }

        private AvailabilityStorageItem GetStorageItemFromViewModel(SubscriberViewModel subscriberViewModel)
        {
            return new AvailabilityStorageItem
            {
                SubscriberId = subscriberViewModel.Id,
                AvailabilityKey = subscriberViewModel.AvailabilityStatus?.Setting?.Nummer,
                Timestamp = subscriberViewModel.AvailabilityStatus?.Timestamp
            };
        }
    }
}

using System;
using System.Collections.Generic;
using AMTools.Web.Data.JsonStore.Models;

namespace AMTools.Web.Data.JsonStore.Repositories
{
    public interface IAvailabilityJsonStoreRepository
    {
        void CreateOrUpdate(AvailabilityStorageItem availabilityStorageItem);
        void Delete(int subscriberId);
        void DeleteAll();
        List<AvailabilityStorageItem> GetAll();
        AvailabilityStorageItem GetBySubscriberId(int subscriberId);
    }
}
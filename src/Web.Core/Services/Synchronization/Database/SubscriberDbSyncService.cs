using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Synchronization.Database.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using AMTools.Web.Data.Database.Repositories;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;

namespace AMTools.Web.Core.Services.Synchronization.Database
{
    public class SubscriberDbSyncService : ISubscriberDbSyncService
    {
        private readonly ISubscriberFileRepository _subscriberFileRepository;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IMapper _mapper;

        public SubscriberDbSyncService(
            ISubscriberFileRepository subscriberFileRepository,
            IConfigurationFileRepository configurationFileRepository,
            IMapper mapper)
        {
            _subscriberFileRepository = subscriberFileRepository;
            _configurationFileRepository = configurationFileRepository;
            _mapper = mapper;
        }

        public void Sync()
        {
            List<Subscriber> fileSubscribers = _subscriberFileRepository.GetAll();
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var dbRepo = unit.GetRepository<SubscriberDbRepository>();

                // Keine FileSubscribers => DB leeren
                if (fileSubscribers == null || fileSubscribers.Count == 0)
                {
                    dbRepo.DeleteAll();
                    unit.SaveChanges();
                    return;
                }

                List<DbSubscriber> existingDbSubscribers = dbRepo.GetAll();

                // Keine DB-Subscriber => Alle FileSubscriber inserten
                if (existingDbSubscribers == null || existingDbSubscribers.Count == 0)
                {
                    List<DbSubscriber> mappedSubscribers = _mapper.Map<List<DbSubscriber>>(fileSubscribers);
                    dbRepo.Insert(mappedSubscribers);
                    unit.SaveChanges();
                    return;
                }

                var hasChanges = false;

                // Nicht mehr existierende Subscriber aus der DB löschen
                foreach (DbSubscriber existingDbSubscriber in existingDbSubscribers)
                {
                    if (!fileSubscribers.Any(x => x.Issi == existingDbSubscriber.Issi))
                    {
                        hasChanges = true;
                        dbRepo.Delete(existingDbSubscriber.Id);
                    }
                }

                // File-Subscriber auf neue Datensätze und auf Updates überprüfen
                foreach (Subscriber fileSubscriber in fileSubscribers)
                {
                    DbSubscriber existingDbSubscriber = existingDbSubscribers.FirstOrDefault(x => x.Issi == fileSubscriber.Issi);

                    // Subscriber existierte noch nicht => Insert
                    if (existingDbSubscriber == null)
                    {
                        DbSubscriber mappedFileSubscriber = _mapper.Map<DbSubscriber>(fileSubscriber);
                        dbRepo.Insert(mappedFileSubscriber);
                        hasChanges = true;
                        continue;
                    }

                    Subscriber mappedDbSubscriber = _mapper.Map<Subscriber>(existingDbSubscriber);

                    // Subscriber hat sich geändert => Update
                    if (!SubscribersAreEqual(fileSubscriber, mappedDbSubscriber))
                    {
                        DbSubscriber mergedSubscriber = _mapper.Map(fileSubscriber, existingDbSubscriber);
                        mergedSubscriber.SysStampUp = DateTime.Now;
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    unit.SaveChanges();
                }
            }
        }

        private bool SubscribersAreEqual(Subscriber source, Subscriber target)
        {
            if (source == null && target != null || source != null & target == null)
            {
                return false;
            }

            return
                source.Issi == target.Issi &&
                source.Reihenfolge == target.Reihenfolge &&
                source.Name == target.Name &&
                source.Qualification == target.Qualification;
        }
    }
}

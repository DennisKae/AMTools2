using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using AMTools.Web.Data.Database.Repositories;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;

namespace AMTools.Web.Core.Services.DataSynchronization
{
    public class AvailabilityStatusSyncService
    {
        private readonly IAvailabilityFileRepository _availabilityFileRepository;
        private readonly IMapper _mapper;

        public AvailabilityStatusSyncService(
            IAvailabilityFileRepository availabilityFileRepository,
            IMapper mapper)
        {
            _availabilityFileRepository = availabilityFileRepository;
            _mapper = mapper;
        }

        public void Sync()
        {
            List<AvailabilityStatus> fileStatusses = _availabilityFileRepository.GetAllAvailabilities();
            using (var unit = new UnitOfWork())
            {
                var dbRepo = unit.GetRepository<AvailabilityStatusDbRepository>();

                // Keine FileStati => DB leeren
                if (fileStatusses == null || fileStatusses.Count == 0)
                {
                    dbRepo.DeleteAll();
                    unit.SaveChanges();
                    return;
                }

                var existingDbStatusses = dbRepo.GetAll();

                // Keine DB-Stati => Alle FileStati inserten
                if (existingDbStatusses == null || existingDbStatusses.Count == 0)
                {
                    List<DbAvailabilityStatus> mappedStatusses = _mapper.Map<List<DbAvailabilityStatus>>(fileStatusses);
                    dbRepo.Insert(mappedStatusses);
                    unit.SaveChanges();
                    return;
                }

                var hasChanges = false;

                // Nicht mehr existierende Stati aus der DB löschen
                foreach (DbAvailabilityStatus existingDbStatus in existingDbStatusses)
                {
                    if (!fileStatusses.Any(x => x.Issi == existingDbStatus.Issi))
                    {
                        hasChanges = true;
                        dbRepo.Delete(existingDbStatus.Issi);
                    }
                }

                // File-Stati auf neue Datensätze und auf Updates überprüfen
                foreach (AvailabilityStatus fileStatus in fileStatusses)
                {
                    DbAvailabilityStatus existingDbStatus = existingDbStatusses.FirstOrDefault(x => x.Issi == fileStatus.Issi);

                    // Status existierte noch nicht => Insert
                    if (existingDbStatus == null)
                    {
                        DbAvailabilityStatus mappedFileStatus = _mapper.Map<DbAvailabilityStatus>(fileStatus);
                        dbRepo.Insert(mappedFileStatus);
                        hasChanges = true;
                        continue;
                    }

                    AvailabilityStatus mappedDbStatus = _mapper.Map<AvailabilityStatus>(existingDbStatus);

                    // Status hat sich geändert => Update
                    if (!StatussesAreEqual(fileStatus, mappedDbStatus))
                    {
                        DbAvailabilityStatus mergedStatus = _mapper.Map(fileStatus, existingDbStatus);
                        mergedStatus.SysStampUp = DateTime.Now;
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    unit.SaveChanges();
                }
            }
        }

        private bool StatussesAreEqual(AvailabilityStatus source, AvailabilityStatus target)
        {
            if (source == null && target != null || source != null & target == null)
            {
                return false;
            }

            return
                source?.Issi == target?.Issi &&
                source?.Timestamp == target?.Timestamp &&
                source?.Value == target?.Value;
        }
    }
}

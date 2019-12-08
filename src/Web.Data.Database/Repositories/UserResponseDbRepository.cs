﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Data.Database.Repositories
{
    public class UserResponseDbRepository
    {
        private readonly DatabaseContext _databaseContext;

        public UserResponseDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<DbUserResponse> GetByAlertId(int alertId) => _databaseContext.UserResponse.Where(x => x.AlertId == alertId).ToList();

        public void Insert(DbUserResponse userResponse) => Insert(new List<DbUserResponse> { userResponse });

        public void Insert(List<DbUserResponse> userResponses)
        {
            _databaseContext.UserResponse.AddRange(userResponses);
            _databaseContext.SaveChanges();
        }

        public void DeleteByAlertId(int alertId)
        {
            var targets = _databaseContext.UserResponse.Where(x => x.AlertId == alertId)?.ToList();
            if (targets?.Count > 0)
            {
                targets.ForEach(x => x.SysDeleted = true);
                _databaseContext.SaveChanges();
            }
        }
    }
}

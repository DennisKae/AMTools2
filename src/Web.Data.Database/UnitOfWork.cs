﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using AMTools.Web.Data.Database.Repositories;

namespace AMTools.Web.Data.Database
{
    public class UnitOfWork : IDisposable
    {
        private readonly DatabaseContext _databaseContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public UnitOfWork()
        {
            _databaseContext = new DatabaseContext();
        }

        /// <summary>Liefert eine Instanz des Repositories mit dem angegebenen Typ (Caching inklusive).</summary>
        /// <typeparam name="T">Zu instanziierender Typ.</typeparam>
        [DebuggerStepThrough]
        public T GetRepository<T>()
        {
            if (typeof(T).BaseType != typeof(BaseRepository))
            {
                throw new ArgumentException("Dieses Repository existiert nicht oder wurde falsch implementiert.");
            }

            if (!_repositories.ContainsKey(typeof(T)))
            {
                _repositories.Add(typeof(T), Activator.CreateInstance(typeof(T), _databaseContext));
            }

            return (T)_repositories[typeof(T)];
        }

        /// <summary>Speichert alle bisher aufgelaufenen Änderungen in die Datenbank.</summary>
        public void SaveChanges()
        {
            _databaseContext.ChangeTracker.DetectChanges();

            _databaseContext.SaveChanges();
        }

        public void Dispose()
        {
            _databaseContext.Dispose();
        }
    }
}

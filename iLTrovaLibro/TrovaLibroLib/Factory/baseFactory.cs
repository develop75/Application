using AutoMapper;
using Helper.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrovaLibro.DataContext;

namespace TrovaLibro.Factory
{
    public class baseFactory
    {
        internal HttpClient? _httpClient = null;
        internal readonly DbTrovaLibroContext _ctx;
        internal readonly IMapper _mapper;
        internal readonly ILog _log;
        internal readonly long _maxRecordsLoaded;

        #region Constructor
        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="ctx">contesto del database</param>
        /// <param name="log">logger</param>
        /// <param name="maxRecordsLoaded">massimo record da caricare, default 1000</param>
        public baseFactory(DbTrovaLibroContext ctx, ILog log, long maxRecordsLoaded = 1000)
        {
            _ctx = ctx;
            _mapper = getMapper();
            _log = log;
            _maxRecordsLoaded = maxRecordsLoaded;
        }
        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="ctx">contesto del database</param>
        /// <param name="log">logger</param>
        /// <param name="maxRecordsLoaded">massimo record da caricare, default 1000</param>
        /// <param name="httpClient">istanza httpclient</param>
        public baseFactory(DbTrovaLibroContext ctx, ILog log, long maxRecordsLoaded = 1000, HttpClient? httpClient = null)
        {
            _httpClient = httpClient;
            _ctx = ctx;
            _mapper = getMapper();
            _log = log;
            _maxRecordsLoaded = maxRecordsLoaded;
        }
        #endregion Constructor

        /// <summary>
        /// Controlla che il numero di record non superi il massimo consentito
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <exception cref="Exception"></exception>
        internal virtual void CheckMaxRecords<T>(IQueryable<T> query)
        {
            if (query.Count() > _maxRecordsLoaded)
                throw new Exception("Too many records. Use filters");
        }
        /// <summary>
        /// Restituisce l'istanza del mapper
        /// </summary>
        /// <returns></returns>
        internal virtual IMapper getMapper()
        {
            //throw new NotImplementedException();

            return null;
        }
    }

}

using AutoMapper;
using Helper.Log;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using TrovaLibroLib.Dto;
using TrovaLibro.Context.DataModels;
using TrovaLibro.Factory;

namespace TrovaLibroLib.Factory
{
    /**
     * @apiDefine ApiRestGroup Gruppo ApiRest
     * Gestione delle configurazioni per le chiamate API esterne (OCR, Google Books, ecc.)
     */
    public class ApiRestFactory : baseFactory, IRepository
    {
        public ApiRestFactory(DbTrovaLibroContext ctx, ILog log) : base(ctx, log, 1000)
        {
        }

        /// <summary>
        /// Configura il motore di mappatura tra entità database (TApiRest) e DTO (ApiRestDto).
        /// Gestisce anche la mappatura nidificata degli Header.
        /// </summary>
        internal override IMapper getMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                // Mapping per la configurazione API principale
                cfg.CreateMap<TApiRest, ApiRestDto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CaName))
                    .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.CaUrl))
                    .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => src.TApiRestHeaders)).ReverseMap();

                // Mapping per i singoli Header dell'API
                cfg.CreateMap<TApiRestHeader, ApiRestHeaderDto>()
                    .ForMember(dest => dest.ApiRestId, opt => opt.MapFrom(src => src.IdApiRest))
                    .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.CaKey))
                    .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.CaValue)).ReverseMap();
            }, NullLoggerFactory.Instance);

            return configuration.CreateMapper();
        }

        /**
         * @api {get} /api/ApiRest Recupera tutte le configurazioni
         * @apiName GetAllApi
         * @apiGroup ApiRestGroup
         * @apiDescription Restituisce la lista completa delle API configurate inclusi i relativi Header.
         */
        public List<T> GetAll<T>() where T : class
        {
            var mapper = getMapper();
            if (typeof(T) == typeof(ApiRestDto))
            {
                // Caricamento Eager degli Header per evitare il problema delle query N+1
                var entities = _ctx.TApiRests.Include(x => x.TApiRestHeaders).ToList();
                return mapper.Map<List<ApiRestDto>>(entities) as List<T> ?? new List<T>();
            }
            return _ctx.Set<T>().ToList();
        }

        /**
         * @api {get} /api/ApiRest/:id Recupera configurazione per ID
         * @apiName GetApiById
         * @apiGroup ApiRestGroup
         * @apiParam {Number} id Identificativo univoco dell'API.
         */
        public T? GetById<T>(long id) where T : class
        {
            var mapper = getMapper();
            if (typeof(T) == typeof(ApiRestDto))
            {
                var entity = _ctx.TApiRests.Include(x => x.TApiRestHeaders).FirstOrDefault(x => x.Id == id);
                return mapper.Map<ApiRestDto>(entity) as T;
            }
            return _ctx.Set<T>().Find(id);
        }

        /**
         * @api {post} /api/ApiRest Crea nuova configurazione
         * @apiName CreateApi
         * @apiGroup ApiRestGroup
         * @apiParam {Object} item Oggetto ApiRestDto o entità TApiRest.
         */
        public T Create<T>(T item) where T : class
        {
            var mapper = getMapper();
            if (item is ApiRestDto dto)
            {
                var entity = mapper.Map<TApiRest>(dto);
                _ctx.TApiRests.Add(entity);
                _ctx.SaveChanges();
                // Restituiamo il DTO mappato dall'entità salvata per includere l'ID generato
                return mapper.Map<ApiRestDto>(entity) as T ?? item;
            }
            _ctx.Set<T>().Add(item);
            _ctx.SaveChanges();
            return item;
        }

        /**
         * @api {put} /api/ApiRest Aggiorna configurazione esistente
         * @apiName UpdateApi
         * @apiGroup ApiRestGroup
         * @apiDescription Aggiorna i dati dell'API e sincronizza la lista degli Header associati.
         */
        public T Update<T>(T item) where T : class
        {
            var mapper = getMapper();
            if (item is ApiRestDto dto)
            {
                // 1. Carichiamo l'entità esistente includendo gli Header
                var existing = _ctx.TApiRests
                    .Include(x => x.TApiRestHeaders)
                    .FirstOrDefault(x => x.Id == dto.Id);

                if (existing == null) throw new Exception("Configurazione non trovata");

                // 2. Aggiorniamo le proprietà semplici e la collezione
                // Per evitare l'errore "severed relationship", mappiamo il DTO sull'entità esistente
                mapper.Map(dto, existing);

                // 3. OPZIONALE: Se l'errore persiste, forziamo la cancellazione degli orfani
                // Questo ciclo identifica i record che non sono più presenti nel DTO e li rimuove dal DB
                var headerIdsInDto = dto.Headers.Select(h => h.Id).ToList();
                var orfani = existing.TApiRestHeaders
                    .Where(h => h.Id > 0 && !headerIdsInDto.Contains(h.Id))
                    .ToList();

                foreach (var orfano in orfani)
                {
                    _ctx.TApiRestHeaders.Remove(orfano);
                }

                _ctx.SaveChanges();
                return mapper.Map<ApiRestDto>(existing) as T ?? item;
            }

            _ctx.Set<T>().Update(item);
            _ctx.SaveChanges();
            return item;
        }

        /**
           * @api {delete} /api/ApiRest/:id Elimina configurazione
           * @apiName DeleteApi
           * @apiGroup ApiRestGroup
           * @apiParam {Number} id ID della configurazione da rimuovere.
        */
        public void Delete<T>(long id) where T : class
        {
            if (typeof(T) == typeof(ApiRestDto))
            {
                // 1. Trova l'entità includendo i figli
                var entity = _ctx.TApiRests
                    .Include(x => x.TApiRestHeaders)
                    .FirstOrDefault(x => x.Id == id);

                if (entity != null)
                {
                    // 2. Rimuovi prima tutti i figli (Header)
                    _ctx.TApiRestHeaders.RemoveRange(entity.TApiRestHeaders);

                    // 3. Ora puoi rimuovere il padre senza errori di vincolo
                    _ctx.TApiRests.Remove(entity);

                    _ctx.SaveChanges();
                    _log.Info($"Eliminata API {id} e i relativi header manualmente.");
                }
                return;
            }

            // Caso per entità generiche
            var item = _ctx.Set<T>().Find(id);
            if (item != null)
            {
                _ctx.Set<T>().Remove(item);
                _ctx.SaveChanges();
            }
        }

        /**
         * @api {get} /api/ApiRest/find Ricerca configurazioni
         * @apiName FindApi
         * @apiGroup ApiRestGroup
         * @apiDescription Filtra le configurazioni in base a un predicato logico.
         */
        public List<T> Find<T>(Func<T, bool> predicate) where T : class
        {
            var mapper = getMapper();
            if (typeof(T) == typeof(ApiRestDto))
            {
                // Per i DTO carichiamo i dati dal DB e applichiamo il filtro in memoria
                var entities = _ctx.TApiRests.Include(x => x.TApiRestHeaders).ToList();
                var dtos = mapper.Map<List<ApiRestDto>>(entities);
                return dtos.Cast<T>().Where(predicate).ToList();
            }
            // Per le entità usiamo il set standard
            return _ctx.Set<T>().Where(predicate).ToList();
        }

        /// <summary>
        /// Recupera una configurazione specifica tramite il nome identificativo.
        /// Molto utile per i servizi interni (es. chiamare GetByName("GoogleBooks")).
        /// </summary>
        public ApiRestDto? GetByName(string name)
        {
            var mapper = getMapper();
            var entity = _ctx.TApiRests
                .Include(x => x.TApiRestHeaders)
                .FirstOrDefault(x => x.CaName.ToLower() == name.ToLower());

            return mapper.Map<ApiRestDto>(entity);
        }
    }
}
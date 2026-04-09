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
     * @apiDefine ApiRestHeaderGroup Gruppo ApiRest Header
     * Gestione dei singoli parametri di intestazione HTTP per le configurazioni API.
     */
    public class ApiRestHeaderFactory : baseFactory, IRepository
    {
        public ApiRestHeaderFactory(DbTrovaLibroContext ctx, ILog log) : base(ctx, log, 1000)
        {
        }

        /// <summary>
        /// Configura il mapping tra l'entità TApiRestHeader e il relativo DTO ApiRestHeaderDto.
        /// </summary>
        internal override IMapper getMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TApiRestHeader, ApiRestHeaderDto>()
                    .ForMember(dest => dest.ApiRestId, opt => opt.MapFrom(src => src.IdApiRest))
                    .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.CaKey))
                    .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.CaValue))
                    .ReverseMap();
            }, NullLoggerFactory.Instance);

            return configuration.CreateMapper();
        }

        /**
         * @api {get} /api/ApiRestHeader Recupera tutti gli header
         * @apiName GetAllHeaders
         * @apiGroup ApiRestHeaderGroup
         */
        public List<T> GetAll<T>() where T : class
        {
            var mapper = getMapper();
            if (typeof(T) == typeof(ApiRestHeaderDto))
            {
                var entities = _ctx.TApiRestHeaders.ToList();
                return mapper.Map<List<ApiRestHeaderDto>>(entities) as List<T> ?? new List<T>();
            }
            return _ctx.Set<T>().ToList();
        }

        /**
         * @api {get} /api/ApiRestHeader/:id Recupera header per ID
         * @apiName GetHeaderById
         * @apiGroup ApiRestHeaderGroup
         */
        public T? GetById<T>(long id) where T : class
        {
            var mapper = getMapper();
            if (typeof(T) == typeof(ApiRestHeaderDto))
            {
                var entity = _ctx.TApiRestHeaders.Find(id);
                return mapper.Map<ApiRestHeaderDto>(entity) as T;
            }
            return _ctx.Set<T>().Find(id);
        }

        /**
         * @api {post} /api/ApiRestHeader Crea nuovo header
         * @apiName CreateHeader
         * @apiGroup ApiRestHeaderGroup
         * @apiDescription Aggiunge un nuovo parametro chiave-valore a una specifica configurazione API.
         */
        public T Create<T>(T item) where T : class
        {
            var mapper = getMapper();
            if (item is ApiRestHeaderDto dto)
            {
                var entity = mapper.Map<TApiRestHeader>(dto);
                _ctx.TApiRestHeaders.Add(entity);
                _ctx.SaveChanges();
                return mapper.Map<ApiRestHeaderDto>(entity) as T ?? item;
            }
            _ctx.Set<T>().Add(item);
            _ctx.SaveChanges();
            return item;
        }

        /**
         * @api {put} /api/ApiRestHeader Aggiorna header
         * @apiName UpdateHeader
         * @apiGroup ApiRestHeaderGroup
         */
        public T Update<T>(T item) where T : class
        {
            var mapper = getMapper();
            if (item is ApiRestHeaderDto dto)
            {
                var existing = _ctx.TApiRestHeaders.Find(dto.Id);
                if (existing == null) throw new Exception("Header non trovato.");

                mapper.Map(dto, existing);
                _ctx.SaveChanges();
                return mapper.Map<ApiRestHeaderDto>(existing) as T ?? item;
            }
            _ctx.Set<T>().Update(item);
            _ctx.SaveChanges();
            return item;
        }

        /**
         * @api {delete} /api/ApiRestHeader/:id Elimina header
         * @apiName DeleteHeader
         * @apiGroup ApiRestHeaderGroup
         */
        public void Delete<T>(long id) where T : class
        {
            var entity = _ctx.Set<T>().Find(id);
            if (entity != null)
            {
                _ctx.Set<T>().Remove(entity);
                _ctx.SaveChanges();
            }
        }

        /**
         * @api {get} /api/ApiRestHeader/find Ricerca header
         * @apiName FindHeaders
         * @apiGroup ApiRestHeaderGroup
         */
        public List<T> Find<T>(Func<T, bool> predicate) where T : class
        {
            var mapper = getMapper();
            if (typeof(T) == typeof(ApiRestHeaderDto))
            {
                var entities = _ctx.TApiRestHeaders.ToList();
                var dtos = mapper.Map<List<ApiRestHeaderDto>>(entities);
                return dtos.Cast<T>().Where(predicate).ToList();
            }
            return _ctx.Set<T>().Where(predicate).ToList();
        }

        /// <summary>
        /// Recupera tutti gli header associati a una specifica API.
        /// Utile per la configurazione rapida di un HttpClient.
        /// </summary>
        public List<ApiRestHeaderDto> GetByApiId(long apiId)
        {
            var mapper = getMapper();
            var entities = _ctx.TApiRestHeaders.Where(x => x.IdApiRest == apiId).ToList();
            return mapper.Map<List<ApiRestHeaderDto>>(entities);
        }
    }
}
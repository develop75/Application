using AutoMapper;
using Helper.Log;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TrovaLibro.Context.DataModels;
using TrovaLibro.Factory;
using TrovaLibroLib.Dto;

namespace TrovaLibroLib.Factory
{
    public class ProvinceDetailFactory : baseFactory, IRepository
    {
        public ProvinceDetailFactory(DbTrovaLibroContext ctx, ILog log) : base(ctx, log, 1000)
        {
        }

        /// <summary>
        /// Configurazione Mapper specifica per la gestione geografica
        /// </summary>
        internal override IMapper getMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                // Mapping City
                cfg.CreateMap<TSysCity, CityDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.CaCityName))
                    .ForMember(dest => dest.CityCode, opt => opt.MapFrom(src => src.CaCityCode));

                // Mapping Province -> ProvinceDetailDto (Include le Cities)
                cfg.CreateMap<TSysProvince, ProvinceDetailDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.CaProvinceName))
                    .ForMember(dest => dest.ProvinceCode, opt => opt.MapFrom(src => src.CaProvinceCode))
                    .ForMember(dest => dest.Cities, opt => opt.MapFrom(src => src.TSysCities));

                // Mapping inverso per eventuali Update (Entity <- DTO)
                cfg.CreateMap<ProvinceDetailDto, TSysProvince>()
                    .ForMember(dest => dest.CaProvinceName, opt => opt.MapFrom(src => src.ProvinceName))
                    .ForMember(dest => dest.CaProvinceCode, opt => opt.MapFrom(src => src.ProvinceCode));

            }, NullLoggerFactory.Instance);

            configuration.CompileMappings();
            return configuration.CreateMapper();
        }

        /// <summary>
        /// Recupera tutte le province includendo le città (Eager Loading)
        /// </summary>
        public List<T> GetAll<T>() where T : class
        {
            if (typeof(T) == typeof(ProvinceDetailDto))
            {
                // Usiamo Include per caricare la relazione con le città ed evitare il problema N+1
                var entities = _ctx.TSysProvinces
                                   .Include(p => p.TSysCities)
                                   .AsNoTracking()
                                   .ToList();

                return getMapper().Map<List<T>>(entities);
            }

            return _ctx.Set<T>().ToList();
        }

        /// <summary>
        /// Recupera una provincia specifica con le sue città
        /// </summary>
        public T? GetById<T>(long id) where T : class
        {
            if (typeof(T) == typeof(ProvinceDetailDto))
            {
                var entity = _ctx.TSysProvinces
                                 .Include(p => p.TSysCities)
                                 .FirstOrDefault(x => x.Id == id);

                return entity == null ? null : getMapper().Map<T>(entity);
            }

            return _ctx.Set<T>().Find(id);
        }

        /// <summary>
        /// Crea o aggiunge una provincia
        /// </summary>
        public T Create<T>(T item) where T : class
        {
            var mapper = getMapper();

            if (typeof(T) == typeof(ProvinceDetailDto))
            {
                var entity = mapper.Map<TSysProvince>(item);
                _ctx.TSysProvinces.Add(entity);
                _ctx.SaveChanges();
                return mapper.Map<T>(entity);
            }

            _ctx.Set<T>().Add(item);
            _ctx.SaveChanges();
            return item;
        }

        /// <summary>
        /// Aggiorna i dati di una provincia
        /// </summary>
        public T Update<T>(T item) where T : class
        {
            var mapper = getMapper();

            if (typeof(T) == typeof(ProvinceDetailDto))
            {
                var dto = item as ProvinceDetailDto;
                var entity = _ctx.TSysProvinces.Find(dto.Id);

                if (entity == null)
                    throw new Exception($"Provincia con ID {dto.Id} non trovata.");

                mapper.Map(dto, entity);
                _ctx.SaveChanges();
                return item;
            }

            _ctx.Set<T>().Update(item);
            _ctx.SaveChanges();
            return item;
        }

        /// <summary>
        /// Elimina una provincia
        /// </summary>
        public void Delete<T>(long id) where T : class
        {
            if (typeof(T) == typeof(ProvinceDetailDto))
            {
                var entity = _ctx.TSysProvinces.Find(id);
                if (entity != null) _ctx.TSysProvinces.Remove(entity);
            }
            else
            {
                var entity = _ctx.Set<T>().Find(id);
                if (entity != null) _ctx.Set<T>().Remove(entity);
            }

            _ctx.SaveChanges();
        }

        /// <summary>
        /// Ricerca filtrata sui DTO
        /// </summary>
        public List<T> Find<T>(Func<T, bool> predicate) where T : class
        {
            var allItems = GetAll<T>();
            return allItems.Where(predicate).ToList();
        }
    }
}
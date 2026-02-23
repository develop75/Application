using AutoMapper;
using Helper.Log;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrovaLibro.DataContext;
using TrovaLibro.DataModels;
using TrovaLibro.Factory;
using TrovaLibroLib.Dto;

namespace TrovaLibroLib.Factory
{
    public class CategoryFactory : baseFactory, IRepository
    {
        public CategoryFactory(DbTrovaLibroContext ctx, ILog log) : base(ctx, log, 1000)
        {
            //
        }

        /// <summary>
        /// Mapper tra entity e dto
        /// </summary>
        /// <returns></returns>
        internal override IMapper getMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TCategory, CategoryDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CaName));

                cfg.CreateMap<CategoryDto, TCategory>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.CaName, opt => opt.MapFrom(src => src.Name));
            }, NullLoggerFactory.Instance);

            configuration.CompileMappings(); // Ensure mappings are compiled before creating the mapper

            return configuration.CreateMapper();
        }

        /// <summary>
        /// Crea una nuova categoria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Create<T>(T item) where T : class
        {
            var mapper = getMapper();

            // 1. Gestione per CategoryDto
            if (typeof(T) == typeof(CategoryDto))
            {
                // Mappiamo il DTO verso l'entità di database (TCategory)
                var entity = mapper.Map<TCategory>(item);

                // Aggiungiamo l'entità al contesto
                _ctx.TCategories.Add(entity);
                _ctx.SaveChanges();

                // Dopo SaveChanges, l'entity ha l'ID generato dal DB.
                // Lo rimappiamo nel DTO per restituire l'oggetto completo di ID.
                return mapper.Map<T>(entity);
            }

            // 3. Caso fallback: se T è già un'entità di database
            _ctx.Set<T>().Add(item);
            _ctx.SaveChanges();
            return item;
        }
        /// <summary>
        /// Elimina una categoria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public void Delete<T>(long id) where T : class
        {
            // 1. Identifichiamo l'entità di database corretta
            if (typeof(T) == typeof(CategoryDto))
            {
                var entity = _ctx.TCategories.Find(id);
                if (entity != null)
                {
                    _ctx.TCategories.Remove(entity);
                }
            }
            else
            {
                // Caso fallback: se T è già un'entità di database
                var entity = _ctx.Set<T>().Find(id);
                if (entity != null)
                {
                    _ctx.Set<T>().Remove(entity);
                }
            }

            // 2. Persistenza delle modifiche
            _ctx.SaveChanges();
        }
        /// <summary>
        /// Trova le categorie in base al predicato
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<T> Find<T>(Func<T, bool> predicate) where T : class
        {
            // 1. Recuperiamo tutti gli elementi mappati (usando il GetAll che abbiamo scritto prima)
            var allItems = GetAll<T>();

            // 2. Applichiamo il filtro LINQ sulla lista di DTO risultante
            return allItems.Where(predicate).ToList();
        }

        //public List<T> Find<T>(Func<T, bool> predicate) where T : class
        //{
        //    var mapper = getMapper();

        //    if (typeof(T) == typeof(CategoryDto))
        //    {
        //        // Recuperiamo le entità, le mappiamo e poi eseguiamo il predicato sul DTO
        //        return _ctx.TCategories
        //            .ToList() // Esegue la query SQL
        //            .Select(e => mapper.Map<T>(e)) // Mappa in CategoryDto
        //            .Where(predicate) // Filtra i DTO
        //            .ToList();
        //    }

        //    // Caso per entità dirette (non DTO)
        //    return _ctx.Set<T>().Where(predicate).ToList();
        //}
        /// <summary>
        /// Restituisce tutte le categorie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetAll<T>() where T : class
        {
            // Identifichiamo l'entità di database corrispondente al DTO richiesto (T)
            // Se T è CategoryDto, dobbiamo leggere da TCategory
            if (typeof(T) == typeof(CategoryDto))
            {
                var entities = _ctx.TCategories.ToList();

                // Usiamo il mapper per convertire da List<TCategory> a List<CategoryDto>
                return getMapper().Map<List<T>>(entities);
            }

            // Caso generico se non vuoi mappare ma restituire l'entità nuda (opzionale)
            return _ctx.Set<T>().ToList();
        }
        /// <summary>
        /// Restituisce una categoria per ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T? GetById<T>(long id) where T : class
        {
            // 1. Gestione specifica per CategoryDto
            if (typeof(T) == typeof(CategoryDto))
            {
                // Cerchiamo l'entità TCategory nel database tramite l'ID
                var entity = _ctx.TCategories.FirstOrDefault(x => x.Id == id);

                if (entity == null) return null;

                // Mappiamo l'entità trovata nel DTO richiesto
                return getMapper().Map<T>(entity);
            }

            // 3. Caso fallback: se T è un'entità di database e non un DTO
            return _ctx.Set<T>().Find(id);
        }
        /// <summary>
        /// Aggiorna una categoria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T Update<T>(T item) where T : class
        {
            var mapper = getMapper();

            // 1. Gestione per CategoryDto
            if (typeof(T) == typeof(CategoryDto))
            {
                var dto = item as CategoryDto;

                // Recuperiamo l'entità originale dal database per non perdere riferimenti
                var entity = _ctx.TCategories.Find(dto.Id);

                if (entity == null)
                    throw new Exception($"Entità con ID {dto.Id} non trovata.");

                // Aggiorniamo i valori dell'entità usando il mapping (ReverseMap)
                mapper.Map(dto, entity);

                _ctx.SaveChanges();

                // Restituiamo il DTO aggiornato (opzionale: potresti voler rimappare da entity)
                return item;
            }

            // 2. Caso generico (se passi direttamente un'entità di database)
            _ctx.Set<T>().Update(item);
            _ctx.SaveChanges();

            return item;
        }
    }
}

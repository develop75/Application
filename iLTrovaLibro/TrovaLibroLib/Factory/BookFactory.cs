using AutoMapper;
using Helper.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using TrovaLibro.DataContext;
using TrovaLibro.DataModels;
using TrovaLibro.Factory;
using TrovaLibroLib.Dto;

namespace TrovaLibroLib.Factory
{
    /// <summary>
    /// Factory per la gestione delle operazioni CRUD e logica di business per l'entità Libri (TBook).
    /// </summary>
    public class BookFactory : baseFactory, IRepository
    {
        /// <summary>
        /// Costruttore della factory dei libri.
        /// </summary>
        /// <param name="ctx">Contesto del database EF Core.</param>
        /// <param name="log">Servizio di logging.</param>
        public BookFactory(DbTrovaLibroContext ctx, ILog log) : base(ctx, log, 1000)
        {
        }

        /// <summary>
        /// Configura e restituisce il mapper specifico per la conversione tra TBook e BookDto.
        /// Gestisce la mappatura dei prefissi DB (Ca, Cd, Fl, Dt) verso le proprietà del DTO.
        /// </summary>
        /// <returns>Istanza di IMapper configurata.</returns>
        internal override IMapper getMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                // Mapping da Entity (DB) a DTO (Frontend)
                cfg.CreateMap<TBook, BookDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.IdUser))
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.CaNameUser))
                    .ForMember(dest => dest.UserCity, opt => opt.MapFrom(src => src.CaCityUser))
                    .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.IdCategory))
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.IdCategoryNavigation.CaName))
                    .ForMember(dest => dest.CategorySubId, opt => opt.MapFrom(src => src.IdCategorySub))
                    .ForMember(dest => dest.CategorySubName, opt => opt.MapFrom(src => src.IdCategorySubNavigation.CaName))
                    .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.IdTarget))
                    .ForMember(dest => dest.TargetName, opt => opt.MapFrom(src => src.IdTargetNavigation.CaName))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CaTitle))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.CaDescription))
                    .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.CaAuthor))
                    .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.CaPublisher))
                    .ForMember(dest => dest.Isbn, opt => opt.MapFrom(src => src.CaIsbn))
                    .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.FlNew ?? false))
                    .ForMember(dest => dest.Cover, opt => opt.MapFrom(src => src.CaCover))
                    .ForMember(dest => dest.CoverPrice, opt => opt.MapFrom(src => src.CdCoverPrice))
                    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.CdPrice))
                    .ForMember(dest => dest.ShippingPrice, opt => opt.MapFrom(src => src.CdShippingPrice))
                    .ForMember(dest => dest.IsShippingAvailable, opt => opt.MapFrom(src => src.FlShipping))
                    .ForMember(dest => dest.IsSelling, opt => opt.MapFrom(src => src.FlSelling))
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.FlActive ?? true))
                    .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.DtCreation))
                    .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.DtUpdate));

                // Mapping da DTO (Frontend) a Entity (DB) - Usato per inserimento e modifica
                cfg.CreateMap<BookDto, TBook>()
                    .ForMember(dest => dest.CaTitle, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.CaDescription, opt => opt.MapFrom(src => src.Description))
                    .ForMember(dest => dest.CaAuthor, opt => opt.MapFrom(src => src.Author))
                    .ForMember(dest => dest.CaPublisher, opt => opt.MapFrom(src => src.Publisher))
                    .ForMember(dest => dest.CaIsbn, opt => opt.MapFrom(src => src.Isbn))
                    .ForMember(dest => dest.FlNew, opt => opt.MapFrom(src => src.IsNew))
                    .ForMember(dest => dest.CaCover, opt => opt.MapFrom(src => src.Cover))
                    .ForMember(dest => dest.CdCoverPrice, opt => opt.MapFrom(src => src.CoverPrice))
                    .ForMember(dest => dest.CdPrice, opt => opt.MapFrom(src => src.Price))
                    .ForMember(dest => dest.CdShippingPrice, opt => opt.MapFrom(src => src.ShippingPrice))
                    .ForMember(dest => dest.FlShipping, opt => opt.MapFrom(src => src.IsShippingAvailable))
                    .ForMember(dest => dest.FlSelling, opt => opt.MapFrom(src => src.IsSelling))
                    .ForMember(dest => dest.FlActive, opt => opt.MapFrom(src => src.IsActive))
                    .ForMember(dest => dest.IdUser, opt => opt.MapFrom(src => src.UserId))
                    .ForMember(dest => dest.CaNameUser, opt => opt.MapFrom(src => src.UserName))
                    .ForMember(dest => dest.CaCityUser, opt => opt.MapFrom(src => src.UserCity))
                    .ForMember(dest => dest.IdCategory, opt => opt.MapFrom(src => src.CategoryId))
                    .ForMember(dest => dest.IdCategorySub, opt => opt.MapFrom(src => src.CategorySubId))
                    .ForMember(dest => dest.IdTarget, opt => opt.MapFrom(src => src.TargetId));

            }, NullLoggerFactory.Instance);

            configuration.CompileMappings();
            return configuration.CreateMapper();
        }

        /// <summary>
        /// Crea un nuovo record libro nel database.
        /// Gestisce automaticamente le date di creazione e aggiornamento.
        /// </summary>
        /// <typeparam name="T">Il tipo dell'oggetto (BookDto).</typeparam>
        /// <param name="item">L'oggetto da creare.</param>
        /// <returns>L'oggetto creato comprensivo di ID generato dal database.</returns>
        public T Create<T>(T item) where T : class
        {
            var mapper = getMapper();

            if (typeof(T) == typeof(BookDto))
            {
                var dto = item as BookDto;

                // 1. Controllo unicità: stesso ISBN e stesso IdUser
                // Nota: Usiamo string.Compare o Equals per gestire eventuali spazi bianchi o formattazione ISBN
                bool alreadyExists = _ctx.TBooks.Any(x =>
                    x.CaIsbn == dto.Isbn &&
                    x.IdUser == dto.UserId);

                if (alreadyExists)
                {
                    _log.Warning($"Tentativo di inserimento duplicato: ISBN {dto.Isbn} per Utente {dto.UserId}");
                    throw new InvalidOperationException($"Questo libro {dto.Isbn} è già presente nella tua libreria.");
                }


                var entity = mapper.Map<TBook>(item);
                entity.DtCreation = DateTime.Now;
                entity.DtUpdate = DateTime.Now;

                _ctx.TBooks.Add(entity);
                _ctx.SaveChanges();

                return mapper.Map<T>(entity);
            }

            _ctx.Set<T>().Add(item);
            _ctx.SaveChanges();
            return item;
        }

        /// <summary>
        /// Elimina un libro dal database in base all'ID fornito.
        /// </summary>
        /// <typeparam name="T">Il tipo di riferimento.</typeparam>
        /// <param name="id">ID univoco del libro.</param>
        public void Delete<T>(long id) where T : class
        {
            if (typeof(T) == typeof(BookDto))
            {
                var entity = _ctx.TBooks.Find(id);
                if (entity != null) _ctx.TBooks.Remove(entity);
            }
            else
            {
                var entity = _ctx.Set<T>().Find(id);
                if (entity != null) _ctx.Set<T>().Remove(entity);
            }
            _ctx.SaveChanges();
        }

        /// <summary>
        /// Recupera tutti i libri presenti nel database.
        /// Se richiesto come BookDto, esegue il caricamento (Include) delle tabelle correlate.
        /// </summary>
        /// <typeparam name="T">Il tipo di ritorno desiderato.</typeparam>
        /// <returns>Lista di oggetti mappati.</returns>
        public List<T> GetAll<T>() where T : class
        {
            if (typeof(T) == typeof(BookDto))
            {
                var entities = _ctx.TBooks
                    .Include(x => x.IdCategoryNavigation)
                    .Include(x => x.IdCategorySubNavigation)
                    .Include(x => x.IdTargetNavigation)
                    .AsNoTracking()
                    .ToList();

                return getMapper().Map<List<T>>(entities);
            }

            return _ctx.Set<T>().ToList();
        }

        /// <summary>
        /// Recupera un singolo libro tramite il suo ID univoco.
        /// </summary>
        /// <typeparam name="T">Il tipo di ritorno desiderato.</typeparam>
        /// <param name="id">ID del libro.</param>
        /// <returns>L'oggetto trovato o null.</returns>
        public T? GetById<T>(long id) where T : class
        {
            if (typeof(T) == typeof(BookDto))
            {
                var entity = _ctx.TBooks
                    .Include(x => x.IdCategoryNavigation)
                    .Include(x => x.IdCategorySubNavigation)
                    .Include(x => x.IdTargetNavigation)
                    .FirstOrDefault(x => x.Id == id);

                return entity == null ? null : getMapper().Map<T>(entity);
            }

            return _ctx.Set<T>().Find(id);
        }

        /// <summary>
        /// Esegue una ricerca filtrata sulla lista completa degli oggetti.
        /// </summary>
        /// <typeparam name="T">Il tipo di oggetto.</typeparam>
        /// <param name="predicate">Funzione di filtro (LINQ).</param>
        /// <returns>Lista di oggetti che soddisfano il predicato.</returns>
        public List<T> Find<T>(Func<T, bool> predicate) where T : class
        {
            var allItems = GetAll<T>();
            return allItems.Where(predicate).ToList();
        }

        /// <summary>
        /// Aggiorna un record esistente nel database.
        /// Gestisce la mappatura parziale e l'aggiornamento della data di modifica.
        /// </summary>
        /// <typeparam name="T">Il tipo dell'oggetto.</typeparam>
        /// <param name="item">L'oggetto con i dati aggiornati.</param>
        /// <returns>L'oggetto aggiornato.</returns>
        /// <exception cref="Exception">Lanciata se il libro con l'ID fornito non esiste.</exception>
        public T Update<T>(T item) where T : class
        {
            var mapper = getMapper();

            if (typeof(T) == typeof(BookDto))
            {
                var dto = item as BookDto;
                var entity = _ctx.TBooks.Find(dto.Id);

                if (entity == null) throw new Exception($"Libro con ID {dto.Id} non trovato.");

                mapper.Map(dto, entity);
                entity.DtUpdate = DateTime.Now;

                _ctx.SaveChanges();
                return item;
            }

            _ctx.Set<T>().Update(item);
            _ctx.SaveChanges();
            return item;
        }
    }
}
using AutoMapper;
using Helper.Log;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrovaLibro.Context.DataModels;
using TrovaLibro.Factory;
using TrovaLibroLib.Dto;

namespace TrovaLibroLib.Factory
{
    public class UserFactory : baseFactory, IRepository
    {
        public UserFactory(DbTrovaLibroContext ctx, ILog log) : base(ctx, log, 1000)
        {
            //
        }

        /// <summary>
        /// Mapper tra entity (TSysUser) e dto (UserDto)
        /// </summary>
        /// <returns></returns>
        internal override IMapper getMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSysUser, UserDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.IdCity))
                    .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.IdProvince))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.CaEmail))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CaName))
                    .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.CaSurname))
                    .ForMember(dest => dest.Cell, opt => opt.MapFrom(src => src.CaCell))
                    .ForMember(dest => dest.FiscalCode, opt => opt.MapFrom(src => src.CaFiscalCode))
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.CaAddress))
                    .ForMember(dest => dest.Cap, opt => opt.MapFrom(src => src.CaCap))
                    .ForMember(dest => dest.Iban, opt => opt.MapFrom(src => src.CaIban))
                    .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.CdRating))
                    .ForMember(dest => dest.ShippingConsent, opt => opt.MapFrom(src => src.FlShipping))
                    .ForMember(dest => dest.MarketingConsent, opt => opt.MapFrom(src => src.FlMarketing))
                    .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.DaCreation))
                    .ForMember(dest => dest.LastActivity, opt => opt.MapFrom(src => src.DaLastActivity))
                    .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.CaIpaddress))
                    .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.FlOnline))
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.FlActive));

                cfg.CreateMap<UserDto, TSysUser>()
                    .ForMember(dest => dest.IdCity, opt => opt.MapFrom(src => src.CityId))
                    .ForMember(dest => dest.IdProvince, opt => opt.MapFrom(src => src.ProvinceId))
                    .ForMember(dest => dest.CaEmail, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.CaName, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.CaSurname, opt => opt.MapFrom(src => src.Surname))
                    .ForMember(dest => dest.CaCell, opt => opt.MapFrom(src => src.Cell))
                    .ForMember(dest => dest.CaFiscalCode, opt => opt.MapFrom(src => src.FiscalCode))
                    .ForMember(dest => dest.CaAddress, opt => opt.MapFrom(src => src.Address))
                    .ForMember(dest => dest.CaCap, opt => opt.MapFrom(src => src.Cap))
                    .ForMember(dest => dest.CaIban, opt => opt.MapFrom(src => src.Iban))
                    .ForMember(dest => dest.CdRating, opt => opt.MapFrom(src => src.Rating))
                    .ForMember(dest => dest.FlShipping, opt => opt.MapFrom(src => src.ShippingConsent))
                    .ForMember(dest => dest.FlMarketing, opt => opt.MapFrom(src => src.MarketingConsent))
                    .ForMember(dest => dest.DaCreation, opt => opt.MapFrom(src => src.CreationDate))
                    .ForMember(dest => dest.DaLastActivity, opt => opt.MapFrom(src => src.LastActivity))
                    .ForMember(dest => dest.CaIpaddress, opt => opt.MapFrom(src => src.IpAddress))
                    .ForMember(dest => dest.FlOnline, opt => opt.MapFrom(src => src.IsOnline))
                    .ForMember(dest => dest.FlActive, opt => opt.MapFrom(src => src.IsActive))
                    .ForMember(dest => dest.CaPassword, opt => opt.Ignore()); // La password viene gestita separatamente per sicurezza

                // Mapping specifico per la Registrazione
                cfg.CreateMap<UserRegistrationDto, TSysUser>()
                    .ForMember(dest => dest.CaEmail, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.CaName, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.CaSurname, opt => opt.MapFrom(src => src.Surname))
                    .ForMember(dest => dest.CaPassword, opt => opt.MapFrom(src => src.Password)) // Mappiamo la password
                    .ForMember(dest => dest.CaFiscalCode, opt => opt.MapFrom(src => src.FiscalCode))
                    .ForMember(dest => dest.CaCell, opt => opt.MapFrom(src => src.Cell))
                    .ForMember(dest => dest.IdCity, opt => opt.MapFrom(src => src.CityId))
                    .ForMember(dest => dest.IdProvince, opt => opt.MapFrom(src => src.ProvinceId))
                    .ForMember(dest => dest.CaAddress, opt => opt.MapFrom(src => src.Address))
                    .ForMember(dest => dest.CaCap, opt => opt.MapFrom(src => src.Cap))
                    .ForMember(dest => dest.FlShipping, opt => opt.MapFrom(src => src.ShippingConsent))
                    .ForMember(dest => dest.FlMarketing, opt => opt.MapFrom(src => src.MarketingConsent))
                    .ForMember(dest => dest.Id, opt => opt.Ignore()); // L'ID lo decide il DB

            }, NullLoggerFactory.Instance);

            configuration.CompileMappings();
            return configuration.CreateMapper();
        }

        /// <summary>
        /// Crea un nuovo utente verificando che non esista già (Email)
        /// </summary>
        public UserDto Create(UserRegistrationDto item, string sKeyCripto) 
        {
            var mapper = getMapper();

            // Gestione esclusiva per la registrazione
     
            var regDto = item as UserRegistrationDto;

            // 1. Validazione unicità (Email e Codice Fiscale)
            bool exists = _ctx.TSysUsers.Any(x =>
                x.CaEmail.ToLower() == regDto.Email.ToLower()
            );

            if (exists)
            {
                _log.Warning($"Registrazione fallita: utente già esistente ({regDto.Email})");
                throw new Exception("Email già presente nei nostri sistemi.");
            }

            // 2. Mapping da RegistrationDto a Entity (TSysUser)
            var entity = mapper.Map<TSysUser>(regDto);

            // 3. Campi di sistema (Audit e Default)
            entity.DaCreation = DateTime.Now;
            entity.FlActive = true;
            entity.FlOnline = false;
            entity.CdRating = 1;

            // NOTA: Qui dovresti gestire l'hashing della password se non arriva già hashata
            entity.CaPassword = Helper.Security.CryptoString.EncryptString(regDto.Password, sKeyCripto);

            _ctx.TSysUsers.Add(entity);
            _ctx.SaveChanges();

            // 4. Restituiamo il DTO "pubblico" (UserDto) e NON quello di registrazione
            // Questo garantisce che la password non torni mai indietro nella risposta API
            return mapper.Map<UserDto>(entity);
            
        }
        /// <summary>
        /// Cambia la password di un utente esistente previa verifica della vecchia password
        /// </summary>
        /// <param name="userId">ID dell'utente</param>
        /// <param name="oldPassword">Password attuale per verifica</param>
        /// <param name="newPassword">Nuova password da impostare</param>
        /// <returns>True se l'operazione è riuscita, altrimenti solleva un'eccezione</returns>
        public bool ChangePassword(long userId, string oldPassword, string newPassword, string sKeyCripto)
        {
            // 1. Recuperiamo l'entità dal database
            var userEntity = _ctx.TSysUsers.Find(userId);

            if (userEntity == null)
            {
                _log.Error($"Cambio password fallito: Utente {userId} non trovato.");
                throw new Exception("Utente non trovato.");
            }

            // 2. Verifica della vecchia password
            // Se usi BCrypt: if (!BCrypt.Net.BCrypt.Verify(oldPassword, userEntity.CaPassword))
            string passwordInDb = Helper.Security.CryptoString.DecryptString(userEntity.CaPassword, sKeyCripto);
            if (passwordInDb != oldPassword)
            {
                _log.Warning($"Cambio password fallito per utente {userId}: Vecchia password errata.");
                throw new Exception("La password attuale inserita non è corretta.");
            }

            // 3. Validazione minima nuova password (opzionale ma consigliata)
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                throw new Exception("La nuova password deve contenere almeno 8 caratteri.");
            }

            if (oldPassword == newPassword)
            {
                throw new Exception("La nuova password non può essere uguale a quella precedente.");
            }

            // 4. Aggiornamento
            // Se usi BCrypt: userEntity.CaPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            userEntity.CaPassword = Helper.Security.CryptoString.EncryptString(newPassword, sKeyCripto);
            userEntity.DaLastActivity = DateTime.Now; // Segniamo l'attività

            _ctx.SaveChanges();

            _log.Info($"Password aggiornata con successo per l'utente {userId}.");

            return true;
        }
        /// <summary>
        /// Effettua il login verificando email e password
        /// </summary>
        /// <param name="email">Email dell'utente</param>
        /// <param name="password">Password in chiaro (verrà confrontata con l'hash nel DB)</param>
        /// <returns>UserDto se le credenziali sono corrette, null altrimenti</returns>
        public UserDto? Login(string email, string password)
        {
            var mapper = getMapper();

            // 1. Cerchiamo l'utente nel database tramite email
            // Usiamo ToLower() per rendere la ricerca case-insensitive
            var userEntity = _ctx.TSysUsers.FirstOrDefault(x =>
                x.CaEmail.ToLower() == email.ToLower() &&
                (x.FlActive == true) // Solo utenti attivi possono loggare
            );

            if (userEntity == null)
            {
                _log.Warning($"Tentativo di login fallito: email {email} non trovata o utente disattivato.");
                return null;
            }

            // 2. Verifica della Password
            // NOTA: Qui dovresti usare una libreria di hashing (es. BCrypt) 
            // IF (BCrypt.Verify(password, userEntity.CaPassword)) ...

            // Esempio con confronto stringa (da usare SOLO se non hai ancora l'hashing implementato)
            if (userEntity.CaPassword != password)
            {
                _log.Warning($"Password errata per l'utente: {email}");
                return null;
            }

            // 3. Aggiornamento dati di sessione (Opzionale)
            userEntity.DaLastActivity = DateTime.Now;
            userEntity.FlOnline = true;
            _ctx.SaveChanges();

            _log.Info($"Login effettuato con successo: {email}");

            // 4. Mappatura verso UserDto (che non contiene la password)
            return mapper.Map<UserDto>(userEntity);
        }
        /// <summary>
        /// Elimina un utente
        /// </summary>
        public void Delete<T>(long id) where T : class
        {
            if (typeof(T) == typeof(UserDto))
            {
                var entity = _ctx.TSysUsers.Find(id);
                if (entity != null) _ctx.TSysUsers.Remove(entity);
            }
            else
            {
                var entity = _ctx.Set<T>().Find(id);
                if (entity != null) _ctx.Set<T>().Remove(entity);
            }

            _ctx.SaveChanges();
        }

        /// <summary>
        /// Filtra gli utenti
        /// </summary>
        public List<T> Find<T>(Func<T, bool> predicate) where T : class
        {
            var allItems = GetAll<T>();
            return allItems.Where(predicate).ToList();
        }

        /// <summary>
        /// Restituisce tutti gli utenti
        /// </summary>
        public List<T> GetAll<T>() where T : class
        {
            if (typeof(T) == typeof(UserDto))
            {
                var entities = _ctx.TSysUsers.ToList();
                return getMapper().Map<List<T>>(entities);
            }

            return _ctx.Set<T>().ToList();
        }

        /// <summary>
        /// Restituisce un utente per ID
        /// </summary>
        public T? GetById<T>(long id) where T : class
        {
            if (typeof(T) == typeof(UserDto))
            {
                var entity = _ctx.TSysUsers.FirstOrDefault(x => x.Id == id);
                if (entity == null) return null;
                return getMapper().Map<T>(entity);
            }

            return _ctx.Set<T>().Find(id);
        }

        /// <summary>
        /// Aggiorna un utente
        /// </summary>
        public T Update<T>(T item) where T : class
        {
            var mapper = getMapper();

            if (typeof(T) == typeof(UserDto))
            {
                var dto = item as UserDto;
                var entity = _ctx.TSysUsers.Find(dto.Id);

                if (entity == null)
                    throw new Exception($"Utente con ID {dto.Id} non trovato.");

                mapper.Map(dto, entity);
                _ctx.SaveChanges();
                return item;
            }

            _ctx.Set<T>().Update(item);
            _ctx.SaveChanges();
            return item;
        }

        public T Create<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
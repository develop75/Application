using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrovaLibroLib.Dto
{
    public class UserRegistrationDto
    {
        // Dati Anagrafici obbligatori per la registrazione
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string FiscalCode { get; set; }
        public string Cell { get; set; }

        // Localizzazione
        public long ProvinceId { get; set; }
        public long CityId { get; set; }
        public string Address { get; set; }
        public string Cap { get; set; }

        // Credenziali (Solo in ingresso)
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // Consensi (Privacy/Marketing)
        public bool ShippingConsent { get; set; }
        public bool MarketingConsent { get; set; }

        // L'Iban potrebbe essere opzionale in fase di registrazione
        public string? Iban { get; set; }
    }
}

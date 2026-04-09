using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrovaLibroLib.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public long CityId { get; set; }
        public long ProvinceId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Cell { get; set; }
        public string FiscalCode { get; set; }
        public string Address { get; set; }
        public string Cap { get; set; }
        public string Iban { get; set; }
        public int Rating { get; set; }
        public bool ShippingConsent { get; set; }
        public bool MarketingConsent { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastActivity { get; set; }
        public string IpAddress { get; set; }
        public bool IsOnline { get; set; }
        public bool IsActive { get; set; }
    }
}

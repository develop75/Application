using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrovaLibroLib.Dto
{
    public class CountryDetailDto
    {
        public long Id { get; set; }
        public string CountryName { get; set; }
        public List<ProvinceDetailDto> Provinces { get; set; } = new();
    }
}

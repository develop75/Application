using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrovaLibroLib.Dto
{
    public class ProvinceDetailDto
    {
        public long Id { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceCode { get; set; }
        public List<CityDto> Cities { get; set; } = new();
    }
}

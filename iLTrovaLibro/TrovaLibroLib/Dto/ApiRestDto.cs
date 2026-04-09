using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrovaLibroLib.Dto
{
    public class ApiRestDto
    {
        [JsonPropertyName("Id")]
        public long Id { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Url")]
        public string Url { get; set; } = string.Empty;

        // Lista degli header associati
        [JsonPropertyName("Headers")]
        public List<ApiRestHeaderDto> Headers { get; set; } = new List<ApiRestHeaderDto>();
    }
}

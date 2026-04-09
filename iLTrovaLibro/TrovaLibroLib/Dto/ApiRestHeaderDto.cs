using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrovaLibroLib.Dto
{
    public class ApiRestHeaderDto
    {
        [JsonPropertyName("Id")]
        public long Id { get; set; }

        [JsonPropertyName("ApiRestId")]
        public long ApiRestId { get; set; }

        [JsonPropertyName("Key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("Value")]
        public string Value { get; set; } = string.Empty;
    }
}

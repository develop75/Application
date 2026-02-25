using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrovaLibroLib.Dto
{
    public class BookDto
    {
        [JsonPropertyName("Id")]
        public long Id { get; set; }

        [JsonPropertyName("UserId")]
        public long UserId { get; set; }

        [JsonPropertyName("UserMail")]
        public string UserMail { get; set; }

        [JsonPropertyName("UserCity")]
        public string UserCity { get; set; }

        [JsonPropertyName("CategoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("CategoryName")] // Da mappare da IdCategoryNavigation
        public string? CategoryName { get; set; }

        [JsonPropertyName("CategorySubId")]
        public long CategorySubId { get; set; }

        [JsonPropertyName("CategorySubName")] // Da mappare da IdCategorySubNavigation
        public string? CategorySubName { get; set; }

        [JsonPropertyName("TargetId")]
        public long TargetId { get; set; }

        [JsonPropertyName("TargetName")] // Da mappare da IdTargetNavigation
        public string? TargetName { get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("Author")]
        public string Author { get; set; }

        [JsonPropertyName("Publisher")]
        public string Publisher { get; set; }

        [JsonPropertyName("Isbn")]
        public string Isbn { get; set; }

        [JsonPropertyName("IsNew")]
        public bool IsNew { get; set; }

        [JsonPropertyName("Cover")]
        public string Cover { get; set; }

        [JsonPropertyName("CoverPrice")]
        public decimal? CoverPrice { get; set; }

        [JsonPropertyName("Price")]
        public decimal Price { get; set; }

        [JsonPropertyName("PriceOld")]
        public decimal PriceOld { get; set; }

        [JsonPropertyName("ShippingPrice")]
        public decimal ShippingPrice { get; set; }

        [JsonPropertyName("IsShippingAvailable")]
        public bool IsShippingAvailable { get; set; }

        [JsonPropertyName("IsSelling")]
        public bool IsSelling { get; set; }

        [JsonPropertyName("IsActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("CreationDate")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("UpdateDate")]
        public DateTime UpdateDate { get; set; }
    }
}

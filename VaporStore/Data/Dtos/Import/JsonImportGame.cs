using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Dtos.Import
{
    public class JsonImportGame
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "435654657564")]
        public decimal Price { get; set; }

        [Required]
        public string ReleaseDate { get; set; }

        [Required]
        [IgnoreMap]
        [JsonProperty("Developer")]
        public string DeveloperName { get; set; }

        [Required] 
        [IgnoreMap]
        [JsonProperty("Genre")]
        public string GenreName { get; set; }

        [Required]
        [IgnoreMap]
        [JsonProperty("Tags")]
        public List<string> TagNames { get; set; } = new List<string>();
    }
}

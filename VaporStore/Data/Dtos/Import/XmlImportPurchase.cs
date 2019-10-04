using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.Data.Dtos.Import
{
    [XmlType("Purchase")]
    public class XmlImportPurchase
    {
        [Required]
        [XmlAttribute("title")]
        public string Title { get; set; }

        [Required]
        [XmlElement("Type")]
        public string Type { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z\d]{4}-[A-Z\d]{4}-[A-Z\d]{4}$")]
        [XmlElement("Key")]
        public string ProductKey { get; set; }

        [Required]
        [XmlElement("Card")]
        [RegularExpression(@"^\d{4} \d{4} \d{4} \d{4}$")]
        [IgnoreMap]
        public string CreditCardNumber { get; set; }

        [Required]
        [XmlElement("Date")]
        public string Date { get; set; }


        
    }
}

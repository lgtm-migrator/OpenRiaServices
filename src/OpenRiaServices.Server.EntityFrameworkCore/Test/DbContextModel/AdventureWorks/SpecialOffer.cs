﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreModels.AdventureWorks.AdventureWorks
{
    [Table("SpecialOffer", Schema = "Sales")]
    public partial class SpecialOffer
    {
        public int SpecialOfferID { get; set; }
        [Required]
        [StringLength(255)]
        public string Description { get; set; }
        [Column(TypeName = "money")]
        public decimal DiscountPct { get; set; }
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        [Required]
        [StringLength(50)]
        public string Category { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime EndDate { get; set; }
        public int MinQty { get; set; }
        public int? MaxQty { get; set; }
        public Guid rowguid { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ModifiedDate { get; set; }
    }
}
﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreModels.AdventureWorks.AdventureWorks
{
    [Table("CountryRegion", Schema = "Person")]
    public partial class CountryRegion
    {
        [Required]
        [StringLength(3)]
        public string CountryRegionCode { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ModifiedDate { get; set; }
    }
}
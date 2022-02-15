﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreModels.AdventureWorks.AdventureWorks
{
    [Table("ProductModelProductDescriptionCulture", Schema = "Production")]
    public partial class ProductModelProductDescriptionCulture
    {
        public int ProductModelID { get; set; }
        public int ProductDescriptionID { get; set; }
        [Required]
        [StringLength(6)]
        public string CultureID { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ModifiedDate { get; set; }
    }
}
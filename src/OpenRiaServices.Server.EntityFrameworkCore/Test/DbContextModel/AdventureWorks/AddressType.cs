﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreModels.AdventureWorks.AdventureWorks
{
    [Table("AddressType", Schema = "Person")]
    public partial class AddressType
    {
        public int AddressTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public Guid rowguid { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ModifiedDate { get; set; }
    }
}
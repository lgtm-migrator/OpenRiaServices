﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreModels.AdventureWorks.AdventureWorks
{
    [Table("Vendor", Schema = "Purchasing")]
    public partial class Vendor
    {
        public int VendorID { get; set; }
        [Required]
        [StringLength(15)]
        public string AccountNumber { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public byte CreditRating { get; set; }
        public bool PreferredVendorStatus { get; set; }
        public bool ActiveFlag { get; set; }
        [StringLength(1024)]
        public string PurchasingWebServiceURL { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ModifiedDate { get; set; }
    }
}
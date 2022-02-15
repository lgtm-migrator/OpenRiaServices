﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreModels.AdventureWorks.AdventureWorks
{
    [Table("TransactionHistory", Schema = "Production")]
    public partial class TransactionHistory
    {
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public int ReferenceOrderID { get; set; }
        public int ReferenceOrderLineID { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime TransactionDate { get; set; }
        [Required]
        [StringLength(1)]
        public string TransactionType { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal ActualCost { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ModifiedDate { get; set; }
    }
}
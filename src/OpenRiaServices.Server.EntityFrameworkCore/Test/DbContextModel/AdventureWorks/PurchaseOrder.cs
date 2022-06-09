﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenRiaServices.Server;

namespace EFCoreModels.AdventureWorks
{
    [Table("PurchaseOrderHeader", Schema = "Purchasing")]
    public partial class PurchaseOrder
    {
        public int PurchaseOrderID { get; set; }
        public byte RevisionNumber { get; set; }
        public byte Status { get; set; }
        public int EmployeeID { get; set; }
        public int VendorID { get; set; }
        public int ShipMethodID { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime OrderDate { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? ShipDate { get; set; }
        [Column(TypeName = "money")]
        public decimal SubTotal { get; set; }
        [Column(TypeName = "money")]
        public decimal TaxAmt { get; set; }
        [Column(TypeName = "money")]
        public decimal Freight { get; set; }
        [Column(TypeName = "money")]
        public decimal TotalDue { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ModifiedDate { get; set; }

        [Include]
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new HashSet<PurchaseOrderDetail>();
    }
}
﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreModels.AdventureWorks.AdventureWorks
{
    [Table("VendorContact", Schema = "Purchasing")]
    public partial class VendorContact
    {
        public int VendorID { get; set; }
        public int ContactID { get; set; }
        public int ContactTypeID { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ModifiedDate { get; set; }
    }
}
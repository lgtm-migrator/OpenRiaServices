﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreModels.AdventureWorks.AdventureWorks
{
    [Table("WorkOrderRouting", Schema = "Production")]
    public partial class WorkOrderRouting
    {
        public int WorkOrderID { get; set; }
        public int ProductID { get; set; }
        public short OperationSequence { get; set; }
        public short LocationID { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ScheduledStartDate { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ScheduledEndDate { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? ActualStartDate { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? ActualEndDate { get; set; }
        [Column(TypeName = "decimal(9, 4)")]
        public decimal? ActualResourceHrs { get; set; }
        [Column(TypeName = "money")]
        public decimal PlannedCost { get; set; }
        [Column(TypeName = "money")]
        public decimal? ActualCost { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime ModifiedDate { get; set; }
    }
}
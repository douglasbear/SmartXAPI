using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_DeliveryDispatch")]
    public partial class InvDeliveryDispatch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_DispatchID")]
        public int NDispatchId { get; set; }
        [Column("X_DispatchNo")]
        [StringLength(50)]
        public string XDispatchNo { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("D_ScheduleDelDate", TypeName = "datetime")]
        public DateTime? DScheduleDelDate { get; set; }
        [Column("D_ScheduleDelTime")]
        public TimeSpan? DScheduleDelTime { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_Address")]
        [StringLength(500)]
        public string XAddress { get; set; }
        [Column("X_Location")]
        [StringLength(200)]
        public string XLocation { get; set; }
        [Column("N_TruckID")]
        public int? NTruckId { get; set; }
        [Column("X_Driver")]
        [StringLength(100)]
        public string XDriver { get; set; }
        [Column("X_AssignedPerson")]
        [StringLength(100)]
        public string XAssignedPerson { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_DispatchScheduled")]
        public bool? BDispatchScheduled { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_SOrderID")]
        public int? NSorderId { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
    }
}

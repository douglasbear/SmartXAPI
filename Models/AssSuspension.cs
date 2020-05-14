using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_Suspension")]
    public partial class AssSuspension
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_SuspendID")]
        public int NSuspendId { get; set; }
        [Column("N_ItemID")]
        [StringLength(50)]
        public string NItemId { get; set; }
        [Column("D_FromDate", TypeName = "datetime")]
        public DateTime? DFromDate { get; set; }
        [Column("D_ToDate", TypeName = "datetime")]
        public DateTime? DToDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}

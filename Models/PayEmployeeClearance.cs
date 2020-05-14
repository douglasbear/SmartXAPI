using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeeClearance")]
    public partial class PayEmployeeClearance
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Key]
        [Column("N_ClearanceID")]
        public int NClearanceId { get; set; }
        [Column("X_ClearanceCode")]
        [StringLength(20)]
        public string XClearanceCode { get; set; }
        [Column("N_PurposeID")]
        public int? NPurposeId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
        [Column("D_RequestDate", TypeName = "datetime")]
        public DateTime? DRequestDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
    }
}

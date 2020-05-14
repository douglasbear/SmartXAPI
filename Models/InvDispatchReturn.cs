using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_DispatchReturn")]
    public partial class InvDispatchReturn
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Key]
        [Column("N_DispatchReturnId")]
        public int NDispatchReturnId { get; set; }
        [Column("X_DispatchReturnNo")]
        [StringLength(50)]
        public string XDispatchReturnNo { get; set; }
        [Column("D_ReturnDate", TypeName = "smalldatetime")]
        public DateTime? DReturnDate { get; set; }
        [Column("N_ProjectId")]
        public int? NProjectId { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_DispatchID")]
        public int? NDispatchId { get; set; }
    }
}

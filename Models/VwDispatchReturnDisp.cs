using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDispatchReturnDisp
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("N_ProjectId")]
        public int? NProjectId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_DispatchReturnId")]
        public int NDispatchReturnId { get; set; }
        [Column("X_DispatchReturnNo")]
        [StringLength(50)]
        public string XDispatchReturnNo { get; set; }
        [Required]
        [Column("D_ReturnDate")]
        [StringLength(8000)]
        public string DReturnDate { get; set; }
        [Column("N_BillAmt")]
        [StringLength(50)]
        public string NBillAmt { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_DispatchID")]
        public int? NDispatchId { get; set; }
        [Column("X_DispatchNo")]
        [StringLength(50)]
        public string XDispatchNo { get; set; }
    }
}

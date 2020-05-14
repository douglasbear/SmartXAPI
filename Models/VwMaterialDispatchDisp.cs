using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMaterialDispatchDisp
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_DispatchId")]
        public int NDispatchId { get; set; }
        [Column("X_DispatchNo")]
        [StringLength(50)]
        public string XDispatchNo { get; set; }
        [Required]
        [Column("D_DispatchDate")]
        [StringLength(8000)]
        public string DDispatchDate { get; set; }
        [Column("N_ProjectId")]
        public int NProjectId { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Required]
        [Column("X_ProjectDescription")]
        [StringLength(250)]
        public string XProjectDescription { get; set; }
        [Column("N_BillAmt")]
        [StringLength(50)]
        public string NBillAmt { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Required]
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("N_DepartmentID")]
        public int NDepartmentId { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Required]
        public string Department { get; set; }
        [Column("X_Responsible")]
        [StringLength(200)]
        public string XResponsible { get; set; }
        [Column("N_ResponsiblePartyID")]
        public int NResponsiblePartyId { get; set; }
    }
}

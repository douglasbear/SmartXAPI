using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_MaterialDispatch")]
    public partial class InvMaterialDispatch
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
        [Column("N_DispatchId")]
        public int NDispatchId { get; set; }
        [Column("X_DispatchNo")]
        [StringLength(50)]
        public string XDispatchNo { get; set; }
        [Column("D_DispatchDate", TypeName = "smalldatetime")]
        public DateTime? DDispatchDate { get; set; }
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
        [Column("X_Responsible")]
        [StringLength(200)]
        public string XResponsible { get; set; }
        [Column("N_ResponsiblePartyID")]
        public int? NResponsiblePartyId { get; set; }
        [Column("N_RSID")]
        public int? NRsid { get; set; }
    }
}

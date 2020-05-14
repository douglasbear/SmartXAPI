using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseOrderNoSearch
    {
        [Required]
        [StringLength(50)]
        public string FileNo { get; set; }
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Column("Order Date")]
        [StringLength(8000)]
        public string OrderDate { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("Order No")]
        [StringLength(50)]
        public string OrderNo { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("Vendor Code")]
        [StringLength(50)]
        public string VendorCode { get; set; }
        [StringLength(100)]
        public string Vendor { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_POrderDate", TypeName = "datetime")]
        public DateTime? DPorderDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Required]
        [StringLength(5)]
        public string TransType { get; set; }
        [Column("N_Amount")]
        [StringLength(30)]
        public string NAmount { get; set; }
        [Column("X_Description")]
        public string XDescription { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Required]
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("N_SOId")]
        public int? NSoid { get; set; }
        [Column("D_UpToDate")]
        [StringLength(8)]
        public string DUpToDate { get; set; }
    }
}

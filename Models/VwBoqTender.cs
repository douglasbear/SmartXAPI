using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBoqTender
    {
        [Column("X_TenderName")]
        [StringLength(100)]
        public string XTenderName { get; set; }
        [Column("X_TenderCode")]
        [StringLength(50)]
        public string XTenderCode { get; set; }
        [Column("N_TenderID")]
        public int? NTenderId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_BOQID")]
        public int NBoqid { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_TaxCategoryID")]
        public int? NTaxCategoryId { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(50)]
        public string XPkeyCode { get; set; }
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("N_productAmt", TypeName = "money")]
        public decimal? NProductAmt { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_FormType")]
        public int? NFormType { get; set; }
        [Column("N_ActualAmt", TypeName = "money")]
        public decimal? NActualAmt { get; set; }
        [Column("N_TenderSaveDraft")]
        public int? NTenderSaveDraft { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
    }
}

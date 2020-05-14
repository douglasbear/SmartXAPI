using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjWorkOrder
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_WorkOrderId")]
        public int NWorkOrderId { get; set; }
        [Column("X_WorkOrderNo")]
        [StringLength(50)]
        public string XWorkOrderNo { get; set; }
        [Column("D_OrderDate", TypeName = "smalldatetime")]
        public DateTime? DOrderDate { get; set; }
        [Column("N_ProjectId")]
        public int? NProjectId { get; set; }
        [Column("N_VendorId")]
        public int? NVendorId { get; set; }
        [Column("X_FDTNo")]
        [StringLength(100)]
        public string XFdtno { get; set; }
        [Column("X_JobNo")]
        [StringLength(100)]
        public string XJobNo { get; set; }
        [Column("N_TypeOfWorkID")]
        public int? NTypeOfWorkId { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("X_Period")]
        [StringLength(10)]
        public string XPeriod { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_TandC")]
        [StringLength(2500)]
        public string XTandC { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_SaveDraft")]
        public int? NSaveDraft { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
        [Column("X_LocationName")]
        [StringLength(2000)]
        public string XLocationName { get; set; }
        [Column("X_Comments")]
        [StringLength(200)]
        public string XComments { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_TaxCategoryID")]
        public int NTaxCategoryId { get; set; }
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
    }
}

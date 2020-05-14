using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectWiseTxnDetail
    {
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_ProjectDescription")]
        [StringLength(250)]
        public string XProjectDescription { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_ContactPerson")]
        [StringLength(50)]
        public string XContactPerson { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("N_EstimateCost", TypeName = "money")]
        public decimal? NEstimateCost { get; set; }
        [Column("N_ContractAmt", TypeName = "money")]
        public decimal? NContractAmt { get; set; }
        [Column("X_Type")]
        [StringLength(15)]
        public string XType { get; set; }
        [Column("N_RefID")]
        public int? NRefId { get; set; }
        [Column("X_RefNo")]
        [StringLength(50)]
        public string XRefNo { get; set; }
        [Column("D_TrnDate", TypeName = "datetime")]
        public DateTime? DTrnDate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice")]
        public double NSprice { get; set; }
        [Column("N_Cost")]
        public double NCost { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_Progress")]
        public int? NProgress { get; set; }
        [Column("B_IsClose")]
        public bool? BIsClose { get; set; }
        [Required]
        [StringLength(6)]
        public string Status { get; set; }
        public double? BaseUnitQty { get; set; }
        [StringLength(500)]
        public string BaseUnit { get; set; }
        [Column("BaseUnitID")]
        public int? BaseUnitId { get; set; }
        [Column("X_FreeDescription")]
        [StringLength(50)]
        public string XFreeDescription { get; set; }
        [Column("B_IsSaveDraft")]
        public int? BIsSaveDraft { get; set; }
        [Column("N_PartyName")]
        [StringLength(100)]
        public string NPartyName { get; set; }
        [Column(TypeName = "money")]
        public decimal? InvoiceDisc { get; set; }
        [Column("X_LedgerName")]
        [StringLength(500)]
        public string XLedgerName { get; set; }
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("N_ManID")]
        public int NManId { get; set; }
        [Column("X_MainProject")]
        [StringLength(500)]
        public string XMainProject { get; set; }
        [Column("N_MainProjectID")]
        public int? NMainProjectId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column(TypeName = "money")]
        public decimal ActualBudget { get; set; }
        public double? RemainingBudget { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPrsdetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("D_PRSDate", TypeName = "datetime")]
        public DateTime? DPrsdate { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_Location")]
        [StringLength(50)]
        public string XLocation { get; set; }
        [Column("N_TotalPRSAmt", TypeName = "money")]
        public decimal? NTotalPrsamt { get; set; }
        [Column("N_PRSDetailsID")]
        public int NPrsdetailsId { get; set; }
        [Column("D_Enquirydate", TypeName = "datetime")]
        public DateTime? DEnquirydate { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("PRS_DeliveryDays")]
        public double? PrsDeliveryDays { get; set; }
        [Column("N_BinBalance")]
        public double? NBinBalance { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_DepQty")]
        public double? NDepQty { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_TransName")]
        [StringLength(50)]
        public string XTransName { get; set; }
        [Column("X_LeadTime")]
        [StringLength(50)]
        public string XLeadTime { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_ProcuredQty")]
        public double? NProcuredQty { get; set; }
        public int? DetailsProcessed { get; set; }
        public double? RemaingQty { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        [Column("N_ItemUnitqty")]
        public double? NItemUnitqty { get; set; }
        [Column("N_UnitQty")]
        public double? NUnitQty { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("X_Remarks")]
        [StringLength(50)]
        public string XRemarks { get; set; }
        [Column("N_MinQty")]
        public double? NMinQty { get; set; }
        [Column("PRSQty")]
        public double Prsqty { get; set; }
        public double StockIssueQty { get; set; }
        [Column("X_Rack")]
        [StringLength(50)]
        public string XRack { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Column("N_UnitSPrice", TypeName = "money")]
        public decimal? NUnitSprice { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
        [Column("N_PreferredVendorID")]
        public int? NPreferredVendorId { get; set; }
        [Column("N_LeadDays")]
        public int? NLeadDays { get; set; }
        [Column("N_TransitDays")]
        public int? NTransitDays { get; set; }
        [Column("N_DeliveryDays")]
        public int? NDeliveryDays { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("X_PartNo")]
        [StringLength(250)]
        public string XPartNo { get; set; }
        [Column("N_SalesOrderDetailsID")]
        public int? NSalesOrderDetailsId { get; set; }
        [Column("N_TaxCategoryID1")]
        public int? NTaxCategoryId1 { get; set; }
        [Column("N_TaxPercentage1", TypeName = "money")]
        public decimal? NTaxPercentage1 { get; set; }
        [Column("N_TaxAmt1", TypeName = "money")]
        public decimal? NTaxAmt1 { get; set; }
        [Column("N_TaxCategoryID2")]
        public int? NTaxCategoryId2 { get; set; }
        [Column("N_TaxPercentage2", TypeName = "money")]
        public decimal? NTaxPercentage2 { get; set; }
        [Column("N_TaxAmt2", TypeName = "money")]
        public decimal? NTaxAmt2 { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_TransTypeID")]
        public int NTransTypeId { get; set; }
        [Column("N_MainProjectID")]
        public int? NMainProjectId { get; set; }
    }
}

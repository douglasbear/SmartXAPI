using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesOrderDtlsForDelNote
    {
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("D_OrderDate", TypeName = "smalldatetime")]
        public DateTime? DOrderDate { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_CashReceived", TypeName = "money")]
        public decimal? NCashReceived { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_SalesOrderDetailsID")]
        public int NSalesOrderDetailsId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_UnitAddlAmount", TypeName = "money")]
        public decimal? NUnitAddlAmount { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("Item Class")]
        [StringLength(25)]
        public string ItemClass { get; set; }
        [Column("Class Item Name")]
        [StringLength(600)]
        public string ClassItemName { get; set; }
        [Column("N_MainQty")]
        public double? NMainQty { get; set; }
        [Column("N_MainSPrice", TypeName = "money")]
        public decimal? NMainSprice { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("Class Item Code")]
        [StringLength(100)]
        public string ClassItemCode { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("X_PhoneNo2")]
        [StringLength(20)]
        public string XPhoneNo2 { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("N_UnitQty")]
        public double? NUnitQty { get; set; }
        [Column("N_MinimumMargin")]
        public double? NMinimumMargin { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("X_ItemRemarks")]
        [StringLength(500)]
        public string XItemRemarks { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_SalesUnitQty")]
        public double? NSalesUnitQty { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_FreeDescription")]
        [StringLength(250)]
        public string XFreeDescription { get; set; }
        [Column("N_SPriceTypeID")]
        public int? NSpriceTypeId { get; set; }
        [Column("N_DeliveryDays")]
        public int? NDeliveryDays { get; set; }
        [Column("X_PartNo")]
        [StringLength(250)]
        public string XPartNo { get; set; }
        [Column("X_ItemManufacturer")]
        [StringLength(50)]
        public string XItemManufacturer { get; set; }
        [Column("D_ExpDeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DExpDeliveryDate { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [StringLength(20)]
        public string Expr1 { get; set; }
        [StringLength(20)]
        public string Expr2 { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("X_FaxNo")]
        [StringLength(100)]
        public string XFaxNo { get; set; }
        [StringLength(100)]
        public string Expr3 { get; set; }
        [Column("X_ContactName")]
        [StringLength(100)]
        public string XContactName { get; set; }
        [Column("X_RfqRefNo")]
        [StringLength(100)]
        public string XRfqRefNo { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("N_MinOrderQty")]
        public double? NMinOrderQty { get; set; }
        [Column(TypeName = "money")]
        public decimal? Discount { get; set; }
        [Column("week")]
        public int? Week { get; set; }
        [Column("days")]
        public int? Days { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
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
        [Column("N_TaxCategoryID")]
        public int? NTaxCategoryId { get; set; }
        [Column("X_DisplayName2")]
        [StringLength(100)]
        public string XDisplayName2 { get; set; }
        [Column("N_Amount2", TypeName = "money")]
        public decimal? NAmount2 { get; set; }
        [Column("N_PkeyID2")]
        public int? NPkeyId2 { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("N_PkeyID")]
        public int? NPkeyId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesReturnWithoutSaleDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_DebitNoteId")]
        public int NDebitNoteId { get; set; }
        [Column("X_DebitNoteNo")]
        [StringLength(50)]
        public string XDebitNoteNo { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("D_ReturnDate", TypeName = "datetime")]
        public DateTime? DReturnDate { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_TotalPaidAmount", TypeName = "money")]
        public decimal? NTotalPaidAmount { get; set; }
        [Column("N_TotalReturnAmount", TypeName = "money")]
        public decimal? NTotalReturnAmount { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_DeliveryNoteId")]
        public int? NDeliveryNoteId { get; set; }
        [Column("B_Invoice")]
        public bool? BInvoice { get; set; }
        [Column("N_SalesDetailsId")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_DebitNoteDetailsID")]
        public int NDebitNoteDetailsId { get; set; }
        [Column("N_RetQty")]
        public int? NRetQty { get; set; }
        [Column("N_RetAmount", TypeName = "money")]
        public decimal? NRetAmount { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("X_ReturnType")]
        [StringLength(25)]
        public string XReturnType { get; set; }
        [Column("X_ReturnRemarks")]
        [StringLength(150)]
        public string XReturnRemarks { get; set; }
        [Column("N_ReturnCost", TypeName = "money")]
        public decimal? NReturnCost { get; set; }
        [Column("N_UnitID")]
        public int? NUnitId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_SellingPrice", TypeName = "money")]
        public decimal? NSellingPrice { get; set; }
        [Column("B_DirPosting")]
        public bool? BDirPosting { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("X_PhoneNo2")]
        [StringLength(20)]
        public string XPhoneNo2 { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("X_DisplayName2")]
        [StringLength(100)]
        public string XDisplayName2 { get; set; }
        [Column("N_TaxID1")]
        public int? NTaxId1 { get; set; }
        [Column("N_TaxPerc1", TypeName = "money")]
        public decimal? NTaxPerc1 { get; set; }
        [Column("N_TaxAmt1", TypeName = "money")]
        public decimal? NTaxAmt1 { get; set; }
        [Column("N_TaxID2")]
        public int? NTaxId2 { get; set; }
        [Column("N_TaxPerc2", TypeName = "money")]
        public decimal? NTaxPerc2 { get; set; }
        [Column("N_TaxAmt2", TypeName = "money")]
        public decimal? NTaxAmt2 { get; set; }
        [Column("N_SPrice")]
        public int? NSprice { get; set; }
    }
}

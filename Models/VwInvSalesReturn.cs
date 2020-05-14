﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesReturn
    {
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
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
        [Column("N_SalesDetailsID")]
        public int NSalesDetailsId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
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
        [StringLength(50)]
        public string ClassItemCode { get; set; }
        [Column("X_CreditNoteNo")]
        [StringLength(50)]
        public string XCreditNoteNo { get; set; }
        [Column("N_RetQty")]
        public int? NRetQty { get; set; }
        [Column("D_ReturnDate", TypeName = "smalldatetime")]
        public DateTime? DReturnDate { get; set; }
        [Column("N_TotalPaidAmount", TypeName = "money")]
        public decimal? NTotalPaidAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal? RetSprice { get; set; }
        [Column("N_MainItemID")]
        public int? NMainItemId { get; set; }
        [Column("N_SubItemQty")]
        public double? NSubItemQty { get; set; }
        [Column("N_CreditNoteId")]
        public int? NCreditNoteId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}

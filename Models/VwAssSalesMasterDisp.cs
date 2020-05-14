﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssSalesMasterDisp
    {
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_AssetInventoryID")]
        public int NAssetInventoryId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("N_InvoiceAmt", TypeName = "money")]
        public decimal? NInvoiceAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_CashReceived", TypeName = "money")]
        public decimal? NCashReceived { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [StringLength(61)]
        public string RunDate { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
    }
}

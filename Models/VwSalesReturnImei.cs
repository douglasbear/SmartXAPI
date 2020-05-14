﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesReturnImei
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
        [Column("N_DebitNoteDetailsID")]
        public int NDebitNoteDetailsId { get; set; }
        [Column("N_SalesDetailsId")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_RetQty")]
        public int? NRetQty { get; set; }
        [Column("N_RetAmount", TypeName = "money")]
        public decimal? NRetAmount { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Expr1 { get; set; }
        public int? Expr2 { get; set; }
        public int? Expr3 { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("X_ReturnType")]
        [StringLength(25)]
        public string XReturnType { get; set; }
        [Column("X_ReturnRemarks")]
        [StringLength(150)]
        public string XReturnRemarks { get; set; }
    }
}

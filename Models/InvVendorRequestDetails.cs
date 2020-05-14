﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_VendorRequestDetails")]
    public partial class InvVendorRequestDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Key]
        [Column("N_QuotationDetailsID")]
        public int NQuotationDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Pprice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_MainItemID")]
        public int? NMainItemId { get; set; }
        [Column("N_MainQty")]
        public double? NMainQty { get; set; }
        [Column("N_MainSPrice", TypeName = "money")]
        public decimal? NMainSprice { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("B_OutwardsProcessed")]
        public bool? BOutwardsProcessed { get; set; }
    }
}

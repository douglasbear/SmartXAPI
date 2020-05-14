using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvReceivableStockSearch
    {
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_ReceivableId")]
        public int NReceivableId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("Memo No")]
        [StringLength(50)]
        public string MemoNo { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("Location Code")]
        public string LocationCode { get; set; }
        [Column("Location Name")]
        public string LocationName { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwLogTransactionRpt
    {
        [StringLength(100)]
        public string Action { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("X_EntryForm")]
        [StringLength(50)]
        public string XEntryForm { get; set; }
        [Column("Voucher No")]
        [StringLength(50)]
        public string VoucherNo { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("System Name")]
        [StringLength(100)]
        public string SystemName { get; set; }
        [Column("D_ActionDate", TypeName = "datetime")]
        public DateTime? DActionDate { get; set; }
        [StringLength(1000)]
        public string Remarks { get; set; }
    }
}

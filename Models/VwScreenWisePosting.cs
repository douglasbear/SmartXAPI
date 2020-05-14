using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwScreenWisePosting
    {
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(500)]
        public string XLedgerName { get; set; }
        [Column(TypeName = "money")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "money")]
        public decimal? Credit { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("X_Code")]
        [StringLength(100)]
        public string XCode { get; set; }
        [Column("X_CompanyName")]
        [StringLength(250)]
        public string XCompanyName { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AccTransID")]
        public int NAccTransId { get; set; }
        [Column("N_LineNo")]
        public int NLineNo { get; set; }
    }
}

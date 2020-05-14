using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class AccVoucherCcamtDefference
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_StmtAmount", TypeName = "money")]
        public decimal? NStmtAmount { get; set; }
        [Required]
        [Column("X_Remarks")]
        [StringLength(250)]
        public string XRemarks { get; set; }
        [Column("N_CCAmount", TypeName = "money")]
        public decimal? NCcamount { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("X_LedgerName_Ar")]
        [StringLength(50)]
        public string XLedgerNameAr { get; set; }
    }
}

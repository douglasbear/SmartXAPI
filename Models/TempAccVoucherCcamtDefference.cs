using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("temp_AccVoucherCCAmtDefference")]
    public partial class TempAccVoucherCcamtDefference
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
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
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(250)]
        public string XRemarks { get; set; }
        [Column("CostCenter_N_Amount", TypeName = "money")]
        public decimal? CostCenterNAmount { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(100)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("X_LedgerName_Ar")]
        [StringLength(50)]
        public string XLedgerNameAr { get; set; }
        [Column("X_CompanyName")]
        [StringLength(250)]
        public string XCompanyName { get; set; }
        [Column("X_ShortName")]
        [StringLength(100)]
        public string XShortName { get; set; }
    }
}

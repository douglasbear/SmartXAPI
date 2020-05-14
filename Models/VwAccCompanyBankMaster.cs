using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccCompanyBankMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BankID")]
        public int NBankId { get; set; }
        [Column("X_BankCode")]
        [StringLength(50)]
        public string XBankCode { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("X_BankNameLocale")]
        [StringLength(100)]
        public string XBankNameLocale { get; set; }
    }
}

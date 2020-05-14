using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCompanyBankDetail
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BankID")]
        public int NBankId { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("X_SwiftNo")]
        [StringLength(50)]
        public string XSwiftNo { get; set; }
        [Column("X_AccountNo")]
        [StringLength(50)]
        public string XAccountNo { get; set; }
        [Column("X_BankNameLocale")]
        [StringLength(100)]
        public string XBankNameLocale { get; set; }
        [Column("X_AccountHolderName")]
        [StringLength(100)]
        public string XAccountHolderName { get; set; }
        [Column("X_IBAN")]
        [StringLength(100)]
        public string XIban { get; set; }
    }
}

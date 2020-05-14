using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBeneficiaryPartyList
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Required]
        [Column("X_FormID")]
        [StringLength(3)]
        public string XFormId { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("X_PartyCode")]
        [StringLength(50)]
        public string XPartyCode { get; set; }
        [Column("X_PartyName")]
        [StringLength(100)]
        public string XPartyName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_BankID")]
        public int? NBankId { get; set; }
        [Column("X_BankAccountNo")]
        [StringLength(50)]
        public string XBankAccountNo { get; set; }
    }
}

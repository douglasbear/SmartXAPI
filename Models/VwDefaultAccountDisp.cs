using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDefaultAccountDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PaymentMethodID")]
        public int NPaymentMethodId { get; set; }
        [Column("X_PayMethod")]
        [StringLength(50)]
        public string XPayMethod { get; set; }
        [Column("X_FieldDescr")]
        [StringLength(50)]
        public string XFieldDescr { get; set; }
        [Column("Account Code")]
        [StringLength(50)]
        public string AccountCode { get; set; }
        [StringLength(100)]
        public string Account { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("B_IsDefault")]
        public bool? BIsDefault { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
    }
}

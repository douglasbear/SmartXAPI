using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_CurrencyMaster")]
    public partial class AccCurrencyMaster
    {
        public AccCurrencyMaster()
        {
            InvVendor = new HashSet<InvVendor>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_CurrencyID")]
        public int NCurrencyId { get; set; }
        [Column("X_CurrencyCode")]
        [StringLength(50)]
        public string XCurrencyCode { get; set; }
        [Column("X_CurrencyName")]
        [StringLength(50)]
        public string XCurrencyName { get; set; }
        [Column("X_ShortName")]
        [StringLength(50)]
        public string XShortName { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("B_default")]
        public bool? BDefault { get; set; }
        [Column("N_Decimal")]
        public int? NDecimal { get; set; }

        [InverseProperty("NCurrency")]
        public virtual ICollection<InvVendor> InvVendor { get; set; }
    }
}

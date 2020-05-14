using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Nationality")]
    public partial class PayNationality
    {
        public PayNationality()
        {
            SchDriverRegistration = new HashSet<SchDriverRegistration>();
        }

        [Key]
        [Column("N_NationalityID")]
        public int NNationalityId { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_NationalityLocale")]
        [StringLength(100)]
        public string XNationalityLocale { get; set; }
        [Column("X_NationalityCode")]
        [StringLength(50)]
        public string XNationalityCode { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_Country")]
        [StringLength(300)]
        public string XCountry { get; set; }
        [Column("B_Default")]
        public bool? BDefault { get; set; }
        [Column("X_Currency")]
        [StringLength(50)]
        public string XCurrency { get; set; }

        [InverseProperty("NNationality")]
        public virtual ICollection<SchDriverRegistration> SchDriverRegistration { get; set; }
    }
}

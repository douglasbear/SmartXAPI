using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_Country")]
    public partial class AccCountry
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_CountryID")]
        public int NCountryId { get; set; }
        [Column("X_CountryCode")]
        [StringLength(50)]
        public string XCountryCode { get; set; }
        [Column("X_CountryName")]
        [StringLength(50)]
        public string XCountryName { get; set; }
        [Column("B_TaxImplement")]
        public bool? BTaxImplement { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_Default")]
        public bool? BDefault { get; set; }
        [Column("X_Currency")]
        [StringLength(50)]
        public string XCurrency { get; set; }
    }
}

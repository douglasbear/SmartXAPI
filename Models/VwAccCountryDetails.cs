using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccCountryDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CountryID")]
        public int NCountryId { get; set; }
        [StringLength(50)]
        public string CountryCode { get; set; }
        [StringLength(50)]
        public string CountryName { get; set; }
        [Column("B_TaxImplement")]
        public bool? BTaxImplement { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
    }
}

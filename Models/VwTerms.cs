using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTerms
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_TermsID")]
        public int NTermsId { get; set; }
        [Column("N_ReferanceID")]
        public int? NReferanceId { get; set; }
        [Column("X_Terms")]
        [StringLength(1000)]
        public string XTerms { get; set; }
        [Column("N_Percentage")]
        [StringLength(100)]
        public string NPercentage { get; set; }
        [Column("N_Duration")]
        public int? NDuration { get; set; }
        [Column("X_Type")]
        [StringLength(1000)]
        public string XType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class ArabicWords
    {
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [StringLength(1000)]
        public string SalesAmountInWords { get; set; }
        [StringLength(1000)]
        public string TaxAmountInWords { get; set; }
    }
}

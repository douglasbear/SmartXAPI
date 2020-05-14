using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTaxDetailsRpt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        public int MainCategory { get; set; }
        public int SubCategory { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
        [StringLength(50)]
        public string Reference { get; set; }
        [Column("Party_Code")]
        [StringLength(50)]
        public string PartyCode { get; set; }
        [Column("Party_Name")]
        [StringLength(100)]
        public string PartyName { get; set; }
        public double? Qty { get; set; }
        [Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Column("N_DetailsID")]
        public int? NDetailsId { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
    }
}

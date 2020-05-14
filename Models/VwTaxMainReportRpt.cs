using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTaxMainReportRpt
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
        [Column(TypeName = "money")]
        public decimal? TaxAmt { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
        [Required]
        [StringLength(50)]
        public string PartyInvoice { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [StringLength(50)]
        public string TaxNo { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [StringLength(50)]
        public string TransType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class TaxDetailsGstRpt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_CessAmt", TypeName = "money")]
        public decimal? NCessAmt { get; set; }
        [Column("SGST")]
        [StringLength(100)]
        public string Sgst { get; set; }
        [Column("CGST")]
        [StringLength(100)]
        public string Cgst { get; set; }
        [Column("SGSTID")]
        public int Sgstid { get; set; }
        [Column("CGSTID")]
        public int Cgstid { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("CGST_0", TypeName = "money")]
        public decimal? Cgst0 { get; set; }
        [Column("SGST_0", TypeName = "money")]
        public decimal? Sgst0 { get; set; }
        [Column("CGST_5", TypeName = "money")]
        public decimal? Cgst5 { get; set; }
        [Column("SGST_5", TypeName = "money")]
        public decimal? Sgst5 { get; set; }
        [Column("CGST_12", TypeName = "money")]
        public decimal? Cgst12 { get; set; }
        [Column("SGST_12", TypeName = "money")]
        public decimal? Sgst12 { get; set; }
        [Column("CGST_18", TypeName = "money")]
        public decimal? Cgst18 { get; set; }
        [Column("SGST_18", TypeName = "money")]
        public decimal? Sgst18 { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
    }
}

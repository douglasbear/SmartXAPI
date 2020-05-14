using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaPaymentdetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_ItemCost", TypeName = "money")]
        public decimal? NItemCost { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_TaxCategoryId")]
        public int? NTaxCategoryId { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal? NTaxAmount { get; set; }
        [Column("N_TaxPerc", TypeName = "money")]
        public decimal? NTaxPerc { get; set; }
        [Column("date")]
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("N_FileID")]
        public int? NFileId { get; set; }
        [Column(TypeName = "money")]
        public decimal? PayAmount { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("paid_amount", TypeName = "money")]
        public decimal? PaidAmount { get; set; }
        [StringLength(100)]
        public string TaxCategory { get; set; }
        [Column("B_InvProcess")]
        public bool? BInvProcess { get; set; }
    }
}

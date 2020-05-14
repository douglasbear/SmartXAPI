using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_AgedPayables")]
    public partial class AccAgedPayables
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnyearID")]
        public int? NFnyearId { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(100)]
        public string XInvoiceNo { get; set; }
        [Column(TypeName = "money")]
        public decimal? Column1 { get; set; }
        [Column(TypeName = "money")]
        public decimal? Column2 { get; set; }
        [Column(TypeName = "money")]
        public decimal? Column3 { get; set; }
        [Column(TypeName = "money")]
        public decimal? Column4 { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("X_Description")]
        [StringLength(300)]
        public string XDescription { get; set; }
        [Column(TypeName = "money")]
        public decimal? Column5 { get; set; }
        [Column("N_PaymentMethod")]
        public int? NPaymentMethod { get; set; }
    }
}

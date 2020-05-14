using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_AgedReceivables")]
    public partial class AccAgedReceivables
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnyearID")]
        public int? NFnyearId { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
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
        [Column(TypeName = "money")]
        public decimal? Column5 { get; set; }
        [Column(TypeName = "money")]
        public decimal? Column6 { get; set; }
        [Column(TypeName = "money")]
        public decimal? Column7 { get; set; }
        [Column("n_ProjectID")]
        public int? NProjectId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPayReceiptSearch
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PayReceiptId")]
        public int NPayReceiptId { get; set; }
        [Required]
        [StringLength(50)]
        public string Memo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(10)]
        public string XType { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [Column("Customer Name")]
        [StringLength(100)]
        public string CustomerName { get; set; }
        [Required]
        [Column("Receipt No")]
        [StringLength(50)]
        public string ReceiptNo { get; set; }
    }
}

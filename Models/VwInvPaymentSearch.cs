using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPaymentSearch
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
        [StringLength(8000)]
        public string Date { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(10)]
        public string XType { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("Vendor Code")]
        [StringLength(50)]
        public string VendorCode { get; set; }
        [Column("Vendor Name")]
        [StringLength(100)]
        public string VendorName { get; set; }
        [Required]
        [Column("Receipt No")]
        [StringLength(50)]
        public string ReceiptNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("X_PaymentMethod")]
        [StringLength(50)]
        public string XPaymentMethod { get; set; }
        [StringLength(50)]
        public string Amount { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(5)]
        public string XStatus { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; }
    }
}

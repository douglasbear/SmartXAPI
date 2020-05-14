using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchReceiptSearch
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ReceiptID")]
        public int NReceiptId { get; set; }
        [Column("Receipt No")]
        [StringLength(50)]
        public string ReceiptNo { get; set; }
        [Required]
        [Column("Register No")]
        [StringLength(25)]
        public string RegisterNo { get; set; }
        [Column("Student Name")]
        [StringLength(200)]
        public string StudentName { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("X_Remarks")]
        [StringLength(150)]
        public string XRemarks { get; set; }
        [Column("B_IsTransfer")]
        public bool? BIsTransfer { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("X_Paymentmethod")]
        [StringLength(50)]
        public string XPaymentmethod { get; set; }
        [Column("N_Amount")]
        [StringLength(20)]
        public string NAmount { get; set; }
    }
}

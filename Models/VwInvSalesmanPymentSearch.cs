using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesmanPymentSearch
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
        [Column("Salesman Code")]
        [StringLength(50)]
        public string SalesmanCode { get; set; }
        [Column("Salesman Name")]
        [StringLength(100)]
        public string SalesmanName { get; set; }
        [Required]
        [Column("Receipt No")]
        [StringLength(50)]
        public string ReceiptNo { get; set; }
        public int Expr1 { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("Mode of Receipt")]
        [StringLength(50)]
        public string ModeOfReceipt { get; set; }
        [Column("Total Amount")]
        [StringLength(20)]
        public string TotalAmount { get; set; }
    }
}

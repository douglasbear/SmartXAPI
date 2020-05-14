using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Previousbalance")]
    public partial class InvPreviousbalance
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TotalPaid", TypeName = "money")]
        public decimal? NTotalPaid { get; set; }
        [Column("N_CashPaid", TypeName = "money")]
        public decimal? NCashPaid { get; set; }
        [Column("N_CreditPaid", TypeName = "money")]
        public decimal? NCreditPaid { get; set; }
        [Column("N_PreviousPaid", TypeName = "money")]
        public decimal? NPreviousPaid { get; set; }
        [Column("N_ReturnAmt", TypeName = "money")]
        public decimal? NReturnAmt { get; set; }
        [Column("N_NetAmount", TypeName = "money")]
        public decimal? NNetAmount { get; set; }
        [Column("D_Day")]
        [StringLength(100)]
        public string DDay { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("D_SalesDate", TypeName = "datetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_PreCashPaid", TypeName = "money")]
        public decimal? NPreCashPaid { get; set; }
        [Column("N_PreCardPaid", TypeName = "money")]
        public decimal? NPreCardPaid { get; set; }
    }
}

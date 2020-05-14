using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesReturnReceipts
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_ReferenseNo")]
        [StringLength(66)]
        public string XReferenseNo { get; set; }
        [Column("N_SalesAmount", TypeName = "money")]
        public decimal? NSalesAmount { get; set; }
        [Column("N_ReturnAmount", TypeName = "money")]
        public decimal? NReturnAmount { get; set; }
        [Column("N_TotalReturnAmt", TypeName = "money")]
        public decimal? NTotalReturnAmt { get; set; }
        [Column("N_ReturnPaidAmount", TypeName = "money")]
        public decimal? NReturnPaidAmount { get; set; }
        [Column("N_ReceivepAmount", TypeName = "money")]
        public decimal? NReceivepAmount { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
    }
}

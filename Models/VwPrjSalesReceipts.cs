using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjSalesReceipts
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_ReferenseNo")]
        [StringLength(63)]
        public string XReferenseNo { get; set; }
        [Column("N_SalesAmount", TypeName = "money")]
        public decimal? NSalesAmount { get; set; }
        [Column("N_ReturnAmount")]
        public int NReturnAmount { get; set; }
        [Column("N_ReceivedAmount", TypeName = "money")]
        public decimal? NReceivedAmount { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("ProjectID")]
        public int? ProjectId { get; set; }
        [Column("N_SalesType")]
        public int? NSalesType { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
    }
}

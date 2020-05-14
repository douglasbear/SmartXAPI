using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesEntryRpt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_ReceiptNo")]
        public int? NReceiptNo { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_GSTIN")]
        [StringLength(100)]
        public string XGstin { get; set; }
    }
}

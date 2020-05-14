using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjSalesCustomer
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("N_SalesType")]
        public int? NSalesType { get; set; }
        [Column("N_SalesRefID")]
        public int? NSalesRefId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
    }
}

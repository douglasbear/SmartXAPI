using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPriceCodeCustomer
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("X_PriceCode")]
        [StringLength(100)]
        public string XPriceCode { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Required]
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Required]
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
    }
}

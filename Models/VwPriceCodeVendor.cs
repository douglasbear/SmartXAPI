using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPriceCodeVendor
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("X_PriceCode")]
        [StringLength(100)]
        public string XPriceCode { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
    }
}

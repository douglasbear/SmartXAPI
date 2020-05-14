using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjVendorDisp
    {
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}

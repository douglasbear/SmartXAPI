using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesmanAppcount
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(137)]
        public string XSalesmanName { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
    }
}

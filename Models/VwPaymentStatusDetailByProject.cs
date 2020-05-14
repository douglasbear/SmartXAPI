using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaymentStatusDetailByProject
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("amount", TypeName = "money")]
        public decimal? Amount { get; set; }
    }
}

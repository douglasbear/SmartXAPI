using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaymentStatusDetailByCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("amount", TypeName = "money")]
        public decimal? Amount { get; set; }
        [Required]
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
    }
}

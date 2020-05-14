using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeeStatus")]
    public partial class PayEmployeeStatus
    {
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
    }
}

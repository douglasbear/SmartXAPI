using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeeRequestType")]
    public partial class PayEmployeeRequestType
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_RequestTypeID")]
        public int NRequestTypeId { get; set; }
        [Column("X_RequestTypeDesc")]
        [StringLength(50)]
        public string XRequestTypeDesc { get; set; }
    }
}

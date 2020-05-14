using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmploymentHistory")]
    public partial class PayEmploymentHistory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_JobID")]
        public int NJobId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_Position")]
        [StringLength(200)]
        public string XPosition { get; set; }
        [Column("X_Company")]
        [StringLength(800)]
        public string XCompany { get; set; }
        [Column("X_Year")]
        [StringLength(100)]
        public string XYear { get; set; }
        [Column("X_Country")]
        [StringLength(200)]
        public string XCountry { get; set; }
        [Column("X_Industry")]
        [StringLength(1000)]
        public string XIndustry { get; set; }
        [Column("D_From", TypeName = "datetime")]
        public DateTime? DFrom { get; set; }
        [Column("D_To", TypeName = "datetime")]
        public DateTime? DTo { get; set; }
    }
}

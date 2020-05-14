using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInterviewEmployeeDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Required]
        [Column("X_RecruitmentCode")]
        [StringLength(400)]
        public string XRecruitmentCode { get; set; }
        [Column("X_Name")]
        [StringLength(100)]
        public string XName { get; set; }
        [Column("X_ContactNo")]
        [StringLength(20)]
        public string XContactNo { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_IntName")]
        [StringLength(1000)]
        public string XIntName { get; set; }
        [Column("T_Time", TypeName = "time(5)")]
        public TimeSpan? TTime { get; set; }
        [Column("X_StatusName")]
        [StringLength(50)]
        public string XStatusName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjects
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("B_IsClose")]
        public bool? BIsClose { get; set; }
        [Column("D_CloseDate", TypeName = "datetime")]
        public DateTime? DCloseDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}

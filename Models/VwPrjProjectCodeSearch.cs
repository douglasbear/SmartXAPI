using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjProjectCodeSearch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("Project Code")]
        [StringLength(50)]
        public string ProjectCode { get; set; }
        [Column("Project Name")]
        [StringLength(100)]
        public string ProjectName { get; set; }
        [Column("Start Date", TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
    }
}

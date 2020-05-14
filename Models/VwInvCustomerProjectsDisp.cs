using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvCustomerProjectsDisp
    {
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("Project Code")]
        [StringLength(100)]
        public string ProjectCode { get; set; }
        [Column("Project Name")]
        [StringLength(50)]
        public string ProjectName { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Required]
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
    }
}

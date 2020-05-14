using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_ApprovalSettings_EmployeeDetails")]
    public partial class SecApprovalSettingsEmployeeDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ApprovalSettingsID")]
        public int NApprovalSettingsId { get; set; }
        [Column("N_ApprovalSettingsDetailsID")]
        public int NApprovalSettingsDetailsId { get; set; }
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
    }
}

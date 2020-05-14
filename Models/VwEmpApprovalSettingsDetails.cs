using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmpApprovalSettingsDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("N_level")]
        public int NLevel { get; set; }
        [Column("N_ActionTypeId")]
        public int? NActionTypeId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
    }
}

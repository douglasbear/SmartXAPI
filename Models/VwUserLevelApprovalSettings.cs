using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwUserLevelApprovalSettings
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ModuleID")]
        public int NModuleId { get; set; }
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
        [Column("N_level")]
        public int NLevel { get; set; }
        [Column("N_ActionTypeId")]
        public int? NActionTypeId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
    }
}

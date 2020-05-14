using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSecUserLevelPermissionDisp
    {
        [Column("X_UserCategory")]
        [StringLength(50)]
        public string XUserCategory { get; set; }
        [Column("X_UserCategoryCode")]
        [StringLength(50)]
        public string XUserCategoryCode { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("B_Active")]
        public bool BActive { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_UserCategoryID")]
        public int? NUserCategoryId { get; set; }
        [Column("N_level")]
        public int? NLevel { get; set; }
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("N_ModuleID")]
        public int NModuleId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_ActionTypeId")]
        public int? NActionTypeId { get; set; }
    }
}

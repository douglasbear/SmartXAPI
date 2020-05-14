using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwApprovalCodeDetails
    {
        [Column("N_ApprovalDetailsID")]
        public int? NApprovalDetailsId { get; set; }
        [Column("N_ActionTypeId")]
        public int? NActionTypeId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("X_UserCategory")]
        [StringLength(50)]
        public string XUserCategory { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("B_Active")]
        public bool BActive { get; set; }
        [Column("X_UserCategoryCode")]
        [StringLength(50)]
        public string XUserCategoryCode { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("N_UserCategoryID")]
        public int? NUserCategoryId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_level")]
        public int? NLevel { get; set; }
    }
}

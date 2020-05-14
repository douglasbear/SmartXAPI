using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_ApprovalCodesDetails")]
    public partial class GenApprovalCodesDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("N_ApprovalDetailsID")]
        public int NApprovalDetailsId { get; set; }
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_level")]
        public int NLevel { get; set; }
        [Column("N_ActionTypeId")]
        public int? NActionTypeId { get; set; }
    }
}

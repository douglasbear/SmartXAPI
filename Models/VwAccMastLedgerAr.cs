using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccMastLedgerAr
    {
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_GroupID")]
        public int NGroupId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("Account Code")]
        [StringLength(50)]
        public string AccountCode { get; set; }
        [StringLength(50)]
        public string Account { get; set; }
        [Column("N_Reserved")]
        public int? NReserved { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Level")]
        [StringLength(100)]
        public string XLevel { get; set; }
        [Required]
        [Column("X_CashTypeBehaviour")]
        [StringLength(50)]
        public string XCashTypeBehaviour { get; set; }
        [Column("X_Type")]
        [StringLength(1)]
        public string XType { get; set; }
        [Column("B_CostCenterEnabled")]
        public bool? BCostCenterEnabled { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_PostingBahavID")]
        public int? NPostingBahavId { get; set; }
        [Column("X_LedgerName_Ar")]
        [StringLength(50)]
        public string XLedgerNameAr { get; set; }
        [Column("X_GroupCode")]
        [StringLength(50)]
        public string XGroupCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(100)]
        public string XGroupName { get; set; }
        [Column("N_LedgerBehaviourID")]
        public int? NLedgerBehaviourId { get; set; }
        [Column("N_CashBahavID")]
        public int? NCashBahavId { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("N_BehaviourGroupID")]
        public int? NBehaviourGroupId { get; set; }
    }
}

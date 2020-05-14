using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeeClearanceDetails")]
    public partial class PayEmployeeClearanceDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ClearanceID")]
        public int? NClearanceId { get; set; }
        [Key]
        [Column("N_ClearanceDetailsID")]
        public int NClearanceDetailsId { get; set; }
        [Column("N_ClearanceSettingsID")]
        public int? NClearanceSettingsId { get; set; }
        [Column("N_ClearanceSettingsDetailsID")]
        public int? NClearanceSettingsDetailsId { get; set; }
        [Column("B_IsApproved")]
        public bool? BIsApproved { get; set; }
        [Column("N_ApprovedUserID")]
        public int? NApprovedUserId { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
        [Column("D_ApprovedDate", TypeName = "datetime")]
        public DateTime? DApprovedDate { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
    }
}

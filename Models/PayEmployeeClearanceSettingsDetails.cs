using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeeClearanceSettingsDetails")]
    public partial class PayEmployeeClearanceSettingsDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ClearanceSettingsID")]
        public int NClearanceSettingsId { get; set; }
        [Key]
        [Column("N_ClearanceSettingsDetailsID")]
        public int NClearanceSettingsDetailsId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_ClearanceItem")]
        [StringLength(200)]
        public string XClearanceItem { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_PurposeID")]
        public int? NPurposeId { get; set; }
    }
}

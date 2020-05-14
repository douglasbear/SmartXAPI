using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeeAlerts")]
    public partial class PayEmployeeAlerts
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AlertID")]
        public int? NAlertId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_Subject")]
        [StringLength(100)]
        public string XSubject { get; set; }
        [Column("D_AlertDate", TypeName = "datetime")]
        public DateTime? DAlertDate { get; set; }
        [Column("N_Days")]
        public int? NDays { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("B_DonotShow")]
        public bool? BDonotShow { get; set; }
        [Column("X_AlertType")]
        [StringLength(50)]
        public string XAlertType { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}

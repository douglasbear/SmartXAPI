using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Supervisor")]
    public partial class PaySupervisor
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_SupervisorID")]
        public int NSupervisorId { get; set; }
        [Column("X_SupervisorCode")]
        [StringLength(50)]
        public string XSupervisorCode { get; set; }
        [Column("X_Supervisor")]
        [StringLength(100)]
        public string XSupervisor { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}

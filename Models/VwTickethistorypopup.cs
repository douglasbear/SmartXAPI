using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTickethistorypopup
    {
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("D_VacSanctionDate", TypeName = "datetime")]
        public DateTime? DVacSanctionDate { get; set; }
        [Column("D_VacApprovedDate", TypeName = "datetime")]
        public DateTime? DVacApprovedDate { get; set; }
        [Column("B_Ticket")]
        public bool? BTicket { get; set; }
        [Column("N_TicketAmount", TypeName = "money")]
        public decimal? NTicketAmount { get; set; }
        [Column("X_TicketRoute")]
        [StringLength(150)]
        public string XTicketRoute { get; set; }
        [Column("N_TicketCount")]
        public int? NTicketCount { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTicketDashBoard
    {
        [Column("X_TicketType")]
        [StringLength(50)]
        public string XTicketType { get; set; }
        [Column("N_TicketAmount")]
        [StringLength(30)]
        public string NTicketAmount { get; set; }
        [Column("N_TicketCount")]
        [StringLength(30)]
        public string NTicketCount { get; set; }
        [Column("X_TicketRoute")]
        [StringLength(150)]
        public string XTicketRoute { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Required]
        [Column("D_DepartureDate")]
        [StringLength(1)]
        public string DDepartureDate { get; set; }
        [Required]
        [Column("D_ArrivalDate")]
        [StringLength(1)]
        public string DArrivalDate { get; set; }
        [Column("X_Reason")]
        [StringLength(50)]
        public string XReason { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("D_TravelDate")]
        [StringLength(30)]
        public string DTravelDate { get; set; }
        [Column("X_TravelClass")]
        [StringLength(50)]
        public string XTravelClass { get; set; }
        [Required]
        [Column("X_AgentName")]
        [StringLength(100)]
        public string XAgentName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}

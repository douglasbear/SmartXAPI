using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Tickets")]
    public partial class PayTickets
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TicketsID")]
        public int NTicketsId { get; set; }
        [Column("X_TicketCode")]
        [StringLength(50)]
        public string XTicketCode { get; set; }
        [Column("X_TicketName")]
        [StringLength(100)]
        public string XTicketName { get; set; }
        [Column("X_TicketType")]
        [StringLength(100)]
        public string XTicketType { get; set; }
        [Column("N_MaxAmt")]
        public int? NMaxAmt { get; set; }
        [Column("N_TicketCount")]
        public int? NTicketCount { get; set; }
        [Column("N_TravelClass")]
        public int? NTravelClass { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_Dayclosing")]
    public partial class AccDayclosing
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("D_ClosedDate", TypeName = "date")]
        public DateTime DClosedDate { get; set; }
        [Key]
        [Column("N_CloseID")]
        public int NCloseId { get; set; }
        [Column("N_CashOpening", TypeName = "money")]
        public decimal? NCashOpening { get; set; }
        [Column("N_CashCr", TypeName = "money")]
        public decimal? NCashCr { get; set; }
        [Column("N_CashDr", TypeName = "money")]
        public decimal? NCashDr { get; set; }
        [Column("N_CashBalance", TypeName = "money")]
        public decimal? NCashBalance { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("B_Closed")]
        public bool? BClosed { get; set; }
        [Column("B_SmsSend")]
        public bool? BSmsSend { get; set; }
        [Column("B_EmailSend")]
        public bool? BEmailSend { get; set; }
        [Column("X_SystemName")]
        [StringLength(100)]
        public string XSystemName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_MastLedger123")]
    public partial class AccMastLedger123
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_GroupID")]
        public int NGroupId { get; set; }
        [Key]
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(10)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_Reserved")]
        public int? NReserved { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Level")]
        [StringLength(100)]
        public string XLevel { get; set; }
        [Column("X_CashTypeBehaviour")]
        [StringLength(50)]
        public string XCashTypeBehaviour { get; set; }
        [Column("X_LedgerName_Ar")]
        [StringLength(50)]
        public string XLedgerNameAr { get; set; }
    }
}

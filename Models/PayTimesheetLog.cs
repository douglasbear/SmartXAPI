using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_TimesheetLog")]
    public partial class PayTimesheetLog
    {
        [Key]
        public long IndexKey { get; set; }
        [Column("UserIDIndex")]
        public long? UserIdindex { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TransactionTime { get; set; }
        [Column("UserID")]
        [StringLength(30)]
        public string UserId { get; set; }
        [Column("TerminalID")]
        public int TerminalId { get; set; }
        public int? AuthType { get; set; }
        public int? AuthResult { get; set; }
        public int? FunctionKey { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ServerRecordTime { get; set; }
        public int? Reserved { get; set; }
        public int? LogType { get; set; }
        public int? TempValue { get; set; }
        public int? MinIndex { get; set; }
        public bool? IsApproved { get; set; }
    }
}

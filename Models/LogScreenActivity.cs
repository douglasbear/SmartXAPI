using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Log_ScreenActivity")]
    public partial class LogScreenActivity
    {
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_EntryForm")]
        [StringLength(100)]
        public string XEntryForm { get; set; }
        [Column("X_DocNo")]
        [StringLength(100)]
        public string XDocNo { get; set; }
        [Column("X_ActionUser")]
        [StringLength(100)]
        public string XActionUser { get; set; }
        [Column("X_ActionType")]
        [StringLength(100)]
        public string XActionType { get; set; }
        [Column("D_ActionDate", TypeName = "datetime")]
        public DateTime? DActionDate { get; set; }
        [Column("X_SystemName")]
        [StringLength(100)]
        public string XSystemName { get; set; }
        [Column("X_IP")]
        [StringLength(100)]
        public string XIp { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
    }
}

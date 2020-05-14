using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwGenApprovalStatus
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_StatusId")]
        public int NStatusId { get; set; }
        [Column("X_StatusName")]
        [StringLength(50)]
        public string XStatusName { get; set; }
        [Column("X_StatusCaption")]
        [StringLength(50)]
        public string XStatusCaption { get; set; }
        [Column("N_YesAction")]
        public int? NYesAction { get; set; }
        [Column("N_NoAction")]
        public int? NNoAction { get; set; }
        [Column("X_MsgStatus")]
        [StringLength(500)]
        public string XMsgStatus { get; set; }
        [Column("B_SysVal")]
        public bool? BSysVal { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
        [Column("N_YesStatusID")]
        public int? NYesStatusId { get; set; }
        [Column("X_YesStatusCaption")]
        [StringLength(50)]
        public string XYesStatusCaption { get; set; }
        [Column("N_NoStatusID")]
        public int? NNoStatusId { get; set; }
        [Column("X_NoStatusCaption")]
        [StringLength(50)]
        public string XNoStatusCaption { get; set; }
    }
}

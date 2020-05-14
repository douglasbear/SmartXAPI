using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_Status")]
    public partial class GenStatus
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Key]
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
        [Column("N_MailID")]
        public int NMailId { get; set; }
        [Column("N_AttachmentCount")]
        public int? NAttachmentCount { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
    }
}

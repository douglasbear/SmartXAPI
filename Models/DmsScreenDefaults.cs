using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Dms_ScreenDefaults")]
    public partial class DmsScreenDefaults
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("X_Subject")]
        [StringLength(100)]
        public string XSubject { get; set; }
        [Column("N_AttachmentCategoryID")]
        public int? NAttachmentCategoryId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_DefaultID")]
        public int? NDefaultId { get; set; }
    }
}

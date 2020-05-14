using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("DMS_MasterFiles")]
    public partial class DmsMasterFiles
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("X_FileCode")]
        public string XFileCode { get; set; }
        [Column("N_FolderID")]
        public int? NFolderId { get; set; }
        [Column("X_Name")]
        public string XName { get; set; }
        [Column("X_Title")]
        public string XTitle { get; set; }
        [Column("X_Contents")]
        public string XContents { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_refName")]
        [StringLength(100)]
        public string XRefName { get; set; }
        [Column("N_AttachmentID")]
        public int? NAttachmentId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_ReminderID")]
        public int? NReminderId { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
    }
}

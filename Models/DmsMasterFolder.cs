using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("DMS_MasterFolder")]
    public partial class DmsMasterFolder
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FolderID")]
        public int NFolderId { get; set; }
        [Column("X_FolderCode")]
        [StringLength(500)]
        public string XFolderCode { get; set; }
        [Column("N_ParentFolderID")]
        public int? NParentFolderId { get; set; }
        [Column("X_Path")]
        public string XPath { get; set; }
        [Column("X_Name")]
        public string XName { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_AttachmentID")]
        public int? NAttachmentId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("B_Default")]
        public bool? BDefault { get; set; }
    }
}

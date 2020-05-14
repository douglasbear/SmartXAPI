using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDmsFileDetail
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("X_FileCode")]
        [StringLength(50)]
        public string XFileCode { get; set; }
        [Column("N_FolderID")]
        public int? NFolderId { get; set; }
        [Column("X_Name")]
        [StringLength(100)]
        public string XName { get; set; }
        [Column("X_refName")]
        [StringLength(100)]
        public string XRefName { get; set; }
        [Column("X_Title")]
        [StringLength(200)]
        public string XTitle { get; set; }
        [Column("X_Contents")]
        [StringLength(200)]
        public string XContents { get; set; }
        [Column("X_FolderName")]
        [StringLength(100)]
        public string XFolderName { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_Path")]
        [StringLength(100)]
        public string XPath { get; set; }
        [Column("X_PathLen")]
        public int? XPathLen { get; set; }
    }
}

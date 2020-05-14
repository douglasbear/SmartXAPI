using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDocumentFolderList
    {
        [Column("X_FolderCode")]
        [StringLength(50)]
        public string XFolderCode { get; set; }
        [Column("N_ParentFolderID")]
        public int? NParentFolderId { get; set; }
        [Column("X_Path")]
        [StringLength(100)]
        public string XPath { get; set; }
        [Column("X_Name")]
        [StringLength(100)]
        public string XName { get; set; }
        [Column("X_FileCode")]
        [StringLength(50)]
        public string XFileCode { get; set; }
        [Column("N_FileID")]
        public int? NFileId { get; set; }
        [StringLength(100)]
        public string FileName { get; set; }
        [Column("X_Title")]
        [StringLength(200)]
        public string XTitle { get; set; }
        [Column("N_FolderID")]
        public int? NFolderId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}

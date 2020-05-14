using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_AssetAddlInfo")]
    public partial class AssAssetAddlInfo
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AddlInfoID")]
        public int NAddlInfoId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_Subject")]
        [StringLength(100)]
        public string XSubject { get; set; }
        [Column("D_IssueDate", TypeName = "datetime")]
        public DateTime? DIssueDate { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Notes")]
        [StringLength(200)]
        public string XNotes { get; set; }
        [Column("N_SubjectID")]
        public int? NSubjectId { get; set; }
    }
}

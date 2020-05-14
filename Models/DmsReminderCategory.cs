using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Dms_ReminderCategory")]
    public partial class DmsReminderCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(100)]
        public string XCategoryCode { get; set; }
        [Column("N_UserCategoryID")]
        public int? NUserCategoryId { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
    }
}

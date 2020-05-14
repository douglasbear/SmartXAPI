using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMeetingTrackerCategory
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(100)]
        public string XCategoryCode { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_DepartmentID")]
        public int NDepartmentId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_PositionID")]
        [StringLength(200)]
        public string XPositionId { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_Positions")]
        [StringLength(200)]
        public string XPositions { get; set; }
        [Column("N_Frequency")]
        public int NFrequency { get; set; }
    }
}

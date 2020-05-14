using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__PSD_StudentList_temp")]
    public partial class PsdStudentListTemp
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("X_Name")]
        [StringLength(500)]
        public string XName { get; set; }
        [Column("X_LastName")]
        [StringLength(500)]
        public string XLastName { get; set; }
        [Column("X_GivenName")]
        [StringLength(300)]
        public string XGivenName { get; set; }
        [Column("X_MiddleName")]
        [StringLength(300)]
        public string XMiddleName { get; set; }
        [Column("X_Initial")]
        [StringLength(100)]
        public string XInitial { get; set; }
        [Column("X_FatherName")]
        [StringLength(300)]
        public string XFatherName { get; set; }
        [Column("X_MotherName")]
        [StringLength(300)]
        public string XMotherName { get; set; }
        [Column("X_FatherEmail")]
        [StringLength(300)]
        public string XFatherEmail { get; set; }
        [Column("X_MotherEmail")]
        [StringLength(300)]
        public string XMotherEmail { get; set; }
        [Column("X_PreviosAddress")]
        [StringLength(300)]
        public string XPreviosAddress { get; set; }
        [Column("X_LeavelPrevious")]
        [StringLength(300)]
        public string XLeavelPrevious { get; set; }
        [Column("X_Status")]
        [StringLength(300)]
        public string XStatus { get; set; }
        [Column("X_CourseType")]
        [StringLength(300)]
        public string XCourseType { get; set; }
        [Column("X_Session")]
        [StringLength(300)]
        public string XSession { get; set; }
    }
}

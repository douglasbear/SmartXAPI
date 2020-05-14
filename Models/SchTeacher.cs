using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_Teacher")]
    public partial class SchTeacher
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_TeacherID")]
        public int NTeacherId { get; set; }
        [Column("N_EmpRefID")]
        public int NEmpRefId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_TeacherCode")]
        [StringLength(50)]
        public string XTeacherCode { get; set; }
        [Column("D_JoinDate", TypeName = "datetime")]
        public DateTime? DJoinDate { get; set; }
        [Column("X_TeacherName")]
        [StringLength(100)]
        public string XTeacherName { get; set; }
        [Column("X_Sex")]
        [StringLength(50)]
        public string XSex { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        public string ImageName { get; set; }
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
        [Column("X_PassportNo")]
        [StringLength(50)]
        public string XPassportNo { get; set; }
        [Column("X_Phone1")]
        [StringLength(50)]
        public string XPhone1 { get; set; }
        [Column("X_Phone2")]
        [StringLength(50)]
        public string XPhone2 { get; set; }
        [Column("X_ResidencePhone")]
        [StringLength(50)]
        public string XResidencePhone { get; set; }
        [Column("X_Qualification")]
        [StringLength(50)]
        public string XQualification { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}

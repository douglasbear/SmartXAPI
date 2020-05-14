using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRecruitdetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_RecruitmentID")]
        public int NRecruitmentId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Required]
        [Column("X_RecruitmentCode")]
        [StringLength(400)]
        public string XRecruitmentCode { get; set; }
        [Column("X_Name")]
        [StringLength(100)]
        public string XName { get; set; }
        [Column("X_City")]
        [StringLength(100)]
        public string XCity { get; set; }
        [Column("N_ExperianceInYears")]
        [StringLength(30)]
        public string NExperianceInYears { get; set; }
        [Column("X_ContactNo")]
        [StringLength(20)]
        public string XContactNo { get; set; }
        [Column("X_Email")]
        [StringLength(100)]
        public string XEmail { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_EduBackground")]
        [StringLength(100)]
        public string XEduBackground { get; set; }
        [Column("X_EduMajor")]
        [StringLength(100)]
        public string XEduMajor { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("X_Vacancy")]
        [StringLength(100)]
        public string XVacancy { get; set; }
        [Column("CV")]
        [StringLength(500)]
        public string Cv { get; set; }
        [Column("X_Status")]
        [StringLength(50)]
        public string XStatus { get; set; }
        [Column("X_Address")]
        [StringLength(400)]
        public string XAddress { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime DDob { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_CvName")]
        [StringLength(100)]
        public string XCvName { get; set; }
        [Column("X_PassPortNo")]
        [StringLength(500)]
        public string XPassPortNo { get; set; }
        [Column("X_ResidanceNo")]
        [StringLength(500)]
        public string XResidanceNo { get; set; }
        [Column("X_IDNo")]
        [StringLength(100)]
        public string XIdno { get; set; }
    }
}

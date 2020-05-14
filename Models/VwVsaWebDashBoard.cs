using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaWebDashBoard
    {
        [Column("DOB")]
        [StringLength(8000)]
        public string Dob { get; set; }
        [Column("N_WebId")]
        public int? NWebId { get; set; }
        [Column("N_RegId")]
        public int NRegId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_RegCode")]
        [StringLength(50)]
        public string XRegCode { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_University")]
        [StringLength(60)]
        public string XUniversity { get; set; }
        [Column("X_Profession")]
        [StringLength(60)]
        public string XProfession { get; set; }
        [Column("X_Qualification")]
        [StringLength(60)]
        public string XQualification { get; set; }
        [Column("X_Email")]
        [StringLength(30)]
        public string XEmail { get; set; }
        [Column("X_mobileNo")]
        [StringLength(20)]
        public string XMobileNo { get; set; }
        [Column("N_Experience")]
        public int? NExperience { get; set; }
        [Column("N_contryid")]
        public int? NContryid { get; set; }
        [Column("N_Nationality")]
        public int? NNationality { get; set; }
        [Column("X_SpouseQualification")]
        [StringLength(20)]
        public string XSpouseQualification { get; set; }
        [Column("X_SpouseUniversity")]
        [StringLength(30)]
        public string XSpouseUniversity { get; set; }
        [Column("X_SpouseOccupation")]
        [StringLength(20)]
        public string XSpouseOccupation { get; set; }
        [Column("N_SpouseExperience")]
        public int? NSpouseExperience { get; set; }
        [Column("N_SpouseAge")]
        public int? NSpouseAge { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("Resume_Path")]
        [StringLength(100)]
        public string ResumePath { get; set; }
        [Column("B_Rejected")]
        public bool? BRejected { get; set; }
    }
}

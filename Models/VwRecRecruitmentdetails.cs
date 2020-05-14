using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRecRecruitmentdetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_RecruitmentID")]
        public int NRecruitmentId { get; set; }
        [Required]
        [Column("X_RecruitmentCode")]
        [StringLength(400)]
        public string XRecruitmentCode { get; set; }
        [Column("X_Name")]
        [StringLength(100)]
        public string XName { get; set; }
        [Column("X_ContactNo")]
        [StringLength(20)]
        public string XContactNo { get; set; }
        [Column("X_Email")]
        [StringLength(100)]
        public string XEmail { get; set; }
        [Column("D_DOB")]
        [StringLength(30)]
        public string DDob { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_Religion")]
        [StringLength(100)]
        public string XReligion { get; set; }
        [Column("X_IDNo")]
        [StringLength(100)]
        public string XIdno { get; set; }
        [Column("X_MaritalStatus")]
        [StringLength(20)]
        public string XMaritalStatus { get; set; }
        [Column("X_City")]
        [StringLength(100)]
        public string XCity { get; set; }
        [Column("X_PassPortNo")]
        [StringLength(500)]
        public string XPassPortNo { get; set; }
        [Column("D_IssueDate")]
        [StringLength(30)]
        public string DIssueDate { get; set; }
        [Column("D_ExpiryDate")]
        [StringLength(30)]
        public string DExpiryDate { get; set; }
        [Column("X_LicenceNo")]
        [StringLength(50)]
        public string XLicenceNo { get; set; }
    }
}

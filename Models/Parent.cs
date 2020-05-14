using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class Parent
    {
        [Column("Family_Id")]
        public int FamilyId { get; set; }
        [Column("FAMILY_FATHER_FIRST_NAME")]
        [StringLength(50)]
        public string FamilyFatherFirstName { get; set; }
        [Required]
        [Column("FAMILY_FATHER_Middle_NAME")]
        [StringLength(1)]
        public string FamilyFatherMiddleName { get; set; }
        [Required]
        [Column("FAMILY_FATHER_Last_NAME")]
        [StringLength(1)]
        public string FamilyFatherLastName { get; set; }
        [Column("Family_Name")]
        [StringLength(50)]
        public string FamilyName { get; set; }
        [Required]
        [Column("Family_Father_age")]
        [StringLength(1)]
        public string FamilyFatherAge { get; set; }
        [Column("Family_father_city")]
        [StringLength(50)]
        public string FamilyFatherCity { get; set; }
        [Column("Family_Father_Address")]
        [StringLength(150)]
        public string FamilyFatherAddress { get; set; }
        [Column("FAMILY_FATHER_EMAIL")]
        [StringLength(100)]
        public string FamilyFatherEmail { get; set; }
        [Required]
        [Column("FAMILY_FATHER_BLOOD_GROUP")]
        [StringLength(1)]
        public string FamilyFatherBloodGroup { get; set; }
        [Column("FAMILY_FATHER_PHOTO", TypeName = "image")]
        public byte[] FamilyFatherPhoto { get; set; }
        [Column("FAMILY_FATHER_NATIONALITY")]
        [StringLength(50)]
        public string FamilyFatherNationality { get; set; }
        [Column("FAMILY_FATHER_NATIONAL_ID")]
        [StringLength(50)]
        public string FamilyFatherNationalId { get; set; }
        [Column("FAMILY_FATHER_PASSPORT")]
        [StringLength(50)]
        public string FamilyFatherPassport { get; set; }
        [Required]
        [Column("FAMILY_FATHER_PROFFESSION")]
        [StringLength(1)]
        public string FamilyFatherProffession { get; set; }
        [Required]
        [Column("FAMILY_FATHER_ANNUAL_INCOME")]
        [StringLength(1)]
        public string FamilyFatherAnnualIncome { get; set; }
        [Column("FAMILY_FATHER_PRIMARY_MOBILE_PHONE")]
        [StringLength(50)]
        public string FamilyFatherPrimaryMobilePhone { get; set; }
        [Required]
        [Column("FAMILY_FATHER_SECONDARY_MOBILE_PHONE")]
        [StringLength(1)]
        public string FamilyFatherSecondaryMobilePhone { get; set; }
        [Column("FAMILY_FATHER_PRIMARY_RESIDENCE_PHONE")]
        [StringLength(100)]
        public string FamilyFatherPrimaryResidencePhone { get; set; }
        [Column("FAMILY_FATHER_SECONDARY_RESIDENCE_PHONE")]
        [StringLength(50)]
        public string FamilyFatherSecondaryResidencePhone { get; set; }
        [Column("FAMILY_MOTHER_FIRST_NAME")]
        [StringLength(50)]
        public string FamilyMotherFirstName { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_MIDDILE_NAME")]
        [StringLength(1)]
        public string FamilyMotherMiddileName { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_LAST_NAME")]
        [StringLength(1)]
        public string FamilyMotherLastName { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_AGE")]
        [StringLength(1)]
        public string FamilyMotherAge { get; set; }
        [Column("FAMILY_MOTHER_ADDRESS")]
        [StringLength(150)]
        public string FamilyMotherAddress { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_EMAIL")]
        [StringLength(1)]
        public string FamilyMotherEmail { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_BLOOD_GROUP")]
        [StringLength(1)]
        public string FamilyMotherBloodGroup { get; set; }
        [Column("FAMILY_MOTHER_PHOTO", TypeName = "image")]
        public byte[] FamilyMotherPhoto { get; set; }
        [Column("FAMILY_MOTHER_NATIONALITY")]
        [StringLength(50)]
        public string FamilyMotherNationality { get; set; }
        [Column("FAMILIY_MOTHER_PASSPORT")]
        [StringLength(50)]
        public string FamiliyMotherPassport { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_PROFFESSION")]
        [StringLength(1)]
        public string FamilyMotherProffession { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_ANNUAL_INCOME")]
        [StringLength(1)]
        public string FamilyMotherAnnualIncome { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_PRIMARY_MOBILE_PHONE")]
        [StringLength(1)]
        public string FamilyMotherPrimaryMobilePhone { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_SECONDARY_MOBILE_PHONE")]
        [StringLength(1)]
        public string FamilyMotherSecondaryMobilePhone { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_PRIMARY_RESIDENCE_PHONE")]
        [StringLength(1)]
        public string FamilyMotherPrimaryResidencePhone { get; set; }
        [Required]
        [Column("FAMILY_MOTHER_SECONDARY_RESIDENCE_PHONE")]
        [StringLength(1)]
        public string FamilyMotherSecondaryResidencePhone { get; set; }
        [Required]
        [Column("FAMILY_NO_OF_BROTHERS")]
        [StringLength(1)]
        public string FamilyNoOfBrothers { get; set; }
        [Required]
        [Column("FAMILY_NO_OF_SISTERS")]
        [StringLength(1)]
        public string FamilyNoOfSisters { get; set; }
        [Required]
        [Column("FAMILY_TOTAL_MEMBERS")]
        [StringLength(1)]
        public string FamilyTotalMembers { get; set; }
        [Required]
        [Column("FAMILY_USER_ID")]
        [StringLength(1)]
        public string FamilyUserId { get; set; }
        [Required]
        [Column("FAMILY_USERNAME")]
        [StringLength(1)]
        public string FamilyUsername { get; set; }
        [Required]
        [Column("FAMILY_PASSWORD")]
        [StringLength(1)]
        public string FamilyPassword { get; set; }
        [Required]
        [Column("FAMILY_REMARKS")]
        [StringLength(1)]
        public string FamilyRemarks { get; set; }
        [Column("FAMILY_CREATED_DATE", TypeName = "datetime")]
        public DateTime FamilyCreatedDate { get; set; }
        [Column("FAMILY_MODIFIED_DATE", TypeName = "datetime")]
        public DateTime FamilyModifiedDate { get; set; }
    }
}

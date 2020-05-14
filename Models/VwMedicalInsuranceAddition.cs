using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMedicalInsuranceAddition
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(50)]
        public string XIqamaNo { get; set; }
        [Column("X_Phone1")]
        [StringLength(50)]
        public string XPhone1 { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_Sex")]
        [StringLength(50)]
        public string XSex { get; set; }
        [Column("X_MaritalStatus")]
        [StringLength(50)]
        public string XMaritalStatus { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("D_HireDate", TypeName = "datetime")]
        public DateTime? DHireDate { get; set; }
        [Column("X_EmployeeSponsorNo")]
        [StringLength(40)]
        public string XEmployeeSponsorNo { get; set; }
        [Column("N_InsuranceID")]
        public int? NInsuranceId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal NPrice { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal NCost { get; set; }
        [Column("D_AdditionDate", TypeName = "datetime")]
        public DateTime? DAdditionDate { get; set; }
        [Required]
        [Column("D_EndDate")]
        [StringLength(23)]
        public string DEndDate { get; set; }
        [Column("N_AdditionId")]
        public int NAdditionId { get; set; }
        [Column("N_DependenceID")]
        public int? NDependenceId { get; set; }
        [Column("N_RelationID")]
        public int? NRelationId { get; set; }
        [Required]
        [Column("X_Relation")]
        [StringLength(50)]
        public string XRelation { get; set; }
        [Column("X_DName")]
        [StringLength(100)]
        public string XDname { get; set; }
        [Column("D_DDOB", TypeName = "datetime")]
        public DateTime? DDdob { get; set; }
        [Column("X_DIqamaNo")]
        [StringLength(50)]
        public string XDiqamaNo { get; set; }
        [Column("X_InsuranceCode")]
        [StringLength(50)]
        public string XInsuranceCode { get; set; }
        [Column("X_InsuranceName")]
        [StringLength(100)]
        public string XInsuranceName { get; set; }
        [StringLength(50)]
        public string InsuranceClassDep { get; set; }
        [Column("X_InsuranceClassEmp")]
        [StringLength(50)]
        public string XInsuranceClassEmp { get; set; }
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
        [Required]
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_CEndDate")]
        public int? NCendDate { get; set; }
    }
}

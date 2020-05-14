using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchStudentReservationDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_RegID")]
        public int NRegId { get; set; }
        [Required]
        [Column("X_RegNo")]
        [StringLength(25)]
        public string XRegNo { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_Name_Ar")]
        [StringLength(250)]
        public string XNameAr { get; set; }
        [Column("X_MiddleName")]
        [StringLength(200)]
        public string XMiddleName { get; set; }
        [Column("X_LastName")]
        [StringLength(200)]
        public string XLastName { get; set; }
        [Column("X_Initial")]
        [StringLength(200)]
        public string XInitial { get; set; }
        [Column("X_GivenName")]
        [StringLength(200)]
        public string XGivenName { get; set; }
        [Column("X_Address")]
        [StringLength(100)]
        public string XAddress { get; set; }
        [Required]
        [Column("X_Phone")]
        [StringLength(25)]
        public string XPhone { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("D_ReserveDate")]
        public string DReserveDate { get; set; }
        [Required]
        [Column("D_TransDate")]
        public string DTransDate { get; set; }
        [Required]
        [Column("N_Amount")]
        [StringLength(30)]
        public string NAmount { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(9)]
        public string XStatus { get; set; }
    }
}

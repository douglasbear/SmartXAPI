using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchDriverRegistration
    {
        [Column("N_NationalityID")]
        public int NNationalityId { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_DriverName")]
        [StringLength(50)]
        public string XDriverName { get; set; }
        [Column("X_IqamaNumber")]
        [StringLength(50)]
        public string XIqamaNumber { get; set; }
        [Column("X_DrivingLicenseID")]
        [StringLength(50)]
        public string XDrivingLicenseId { get; set; }
        public int? Expr1 { get; set; }
        [Column("N_DriverID")]
        public int NDriverId { get; set; }
    }
}

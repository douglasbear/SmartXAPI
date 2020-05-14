using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBusRegistrationRpt
    {
        [Required]
        [Column("Trans_Type")]
        [StringLength(4)]
        public string TransType { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("D_RegisterDate", TypeName = "datetime")]
        public DateTime? DRegisterDate { get; set; }
        [Column("date_month")]
        [StringLength(30)]
        public string DateMonth { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_RouteNo")]
        [StringLength(50)]
        public string XRouteNo { get; set; }
        [Column("X_RouteName")]
        [StringLength(200)]
        public string XRouteName { get; set; }
        [Column("N_VehicleID")]
        public int? NVehicleId { get; set; }
        [Column("X_VehicleCode")]
        [StringLength(50)]
        public string XVehicleCode { get; set; }
        [Column("X_VehicleName")]
        [StringLength(50)]
        public string XVehicleName { get; set; }
    }
}

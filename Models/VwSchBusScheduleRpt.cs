using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchBusScheduleRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("D_AdmissionDate", TypeName = "datetime")]
        public DateTime? DAdmissionDate { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("X_PAddress")]
        [StringLength(150)]
        public string XPaddress { get; set; }
        [Column("X_BuildingNo")]
        [StringLength(100)]
        public string XBuildingNo { get; set; }
        [Column("X_StreetNo")]
        [StringLength(100)]
        public string XStreetNo { get; set; }
        [Column("X_ZoneNo")]
        [StringLength(100)]
        public string XZoneNo { get; set; }
        [Column("X_BuildingName")]
        [StringLength(500)]
        public string XBuildingName { get; set; }
        [Column("X_StreetName")]
        [StringLength(500)]
        public string XStreetName { get; set; }
        [Column("X_ZOneName")]
        [StringLength(500)]
        public string XZoneName { get; set; }
        [Column("X_LandMark")]
        [StringLength(500)]
        public string XLandMark { get; set; }
        [Column("N_Fees", TypeName = "money")]
        public decimal? NFees { get; set; }
        [Column("D_Time")]
        [StringLength(100)]
        public string DTime { get; set; }
        [Column("X_VehicleCode")]
        [StringLength(50)]
        public string XVehicleCode { get; set; }
        [Column("X_VehicleName")]
        [StringLength(50)]
        public string XVehicleName { get; set; }
        [Column("D_Time1")]
        [StringLength(100)]
        public string DTime1 { get; set; }
        [Column("X_VehicleCode1")]
        [StringLength(50)]
        public string XVehicleCode1 { get; set; }
        [Column("X_VehicleName1")]
        [StringLength(50)]
        public string XVehicleName1 { get; set; }
        [Column("X_GaurdianName")]
        [StringLength(250)]
        public string XGaurdianName { get; set; }
        [Column("X_PPhoneNo")]
        [StringLength(50)]
        public string XPphoneNo { get; set; }
        [Column("X_PMobileNo")]
        [StringLength(50)]
        public string XPmobileNo { get; set; }
        [Column("N_RouteID")]
        public int? NRouteId { get; set; }
        [Column("X_RouteNo")]
        [StringLength(50)]
        public string XRouteNo { get; set; }
        [Column("N_RouteID1")]
        public int? NRouteId1 { get; set; }
        [Column("X_RouteNo1")]
        [StringLength(50)]
        public string XRouteNo1 { get; set; }
        [Column("N_VehicleID")]
        public int? NVehicleId { get; set; }
        [Column("N_VehicleID1")]
        public int? NVehicleId1 { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("X_StopCode")]
        [StringLength(20)]
        public string XStopCode { get; set; }
        [Column("X_StopName")]
        [StringLength(20)]
        public string XStopName { get; set; }
        [Column("X_RouteName")]
        [StringLength(200)]
        public string XRouteName { get; set; }
        [Column("X_StopCode1")]
        [StringLength(20)]
        public string XStopCode1 { get; set; }
        [Column("X_StopName1")]
        [StringLength(20)]
        public string XStopName1 { get; set; }
        [Column("X_RouteName1")]
        [StringLength(200)]
        public string XRouteName1 { get; set; }
        [Column("N_AdviserID")]
        public int? NAdviserId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_DriverName")]
        [StringLength(50)]
        public string XDriverName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchStudentBusRegList
    {
        [Column("N_PickRoute")]
        public int? NPickRoute { get; set; }
        [Column("N_PickPoint")]
        public int? NPickPoint { get; set; }
        [Column("N_DropRoute")]
        public int? NDropRoute { get; set; }
        [Column("N_DropPoint")]
        public int? NDropPoint { get; set; }
        [Column("N_FeeTypeID")]
        public int? NFeeTypeId { get; set; }
        [Column("N_Fees", TypeName = "money")]
        public decimal? NFees { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_RouteName")]
        [StringLength(200)]
        public string XRouteName { get; set; }
        [Column("X_RouteNo")]
        [StringLength(50)]
        public string XRouteNo { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("D_Time", TypeName = "datetime")]
        public DateTime DTime { get; set; }
        [Column("N_BusID")]
        public int NBusId { get; set; }
        [Column("X_VehicleCode")]
        [StringLength(50)]
        public string XVehicleCode { get; set; }
        [Column("X_VehicleName")]
        [StringLength(50)]
        public string XVehicleName { get; set; }
        [Column("X_StopCode")]
        [StringLength(20)]
        public string XStopCode { get; set; }
        [Column("X_StopName")]
        [StringLength(20)]
        public string XStopName { get; set; }
        [StringLength(50)]
        public string RouteNo1 { get; set; }
        [StringLength(200)]
        public string RouteName1 { get; set; }
        [Column("TypeID1")]
        public int TypeId1 { get; set; }
        [Column("X_VehicleCode1")]
        [StringLength(50)]
        public string XVehicleCode1 { get; set; }
        [Column("X_VehicleName1")]
        [StringLength(50)]
        public string XVehicleName1 { get; set; }
        [Column("D_Time1", TypeName = "datetime")]
        public DateTime DTime1 { get; set; }
        [Column("N_BusID1")]
        public int NBusId1 { get; set; }
        [Column("X_StopCode1")]
        [StringLength(20)]
        public string XStopCode1 { get; set; }
        [Column("X_StopName1")]
        [StringLength(20)]
        public string XStopName1 { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
    }
}

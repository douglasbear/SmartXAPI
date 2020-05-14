using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchRegDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_RegistrationID")]
        public int NRegistrationId { get; set; }
        [Required]
        [Column("X_RegistrationCode")]
        [StringLength(25)]
        public string XRegistrationCode { get; set; }
        [Column("D_RegisterDate", TypeName = "datetime")]
        public DateTime? DRegisterDate { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Column("X_BuildingNo")]
        [StringLength(100)]
        public string XBuildingNo { get; set; }
        [Column("X_BuildingName")]
        [StringLength(500)]
        public string XBuildingName { get; set; }
        [Column("X_StreetNo")]
        [StringLength(100)]
        public string XStreetNo { get; set; }
        [Column("X_StreetName")]
        [StringLength(500)]
        public string XStreetName { get; set; }
        [Column("X_ZoneNo")]
        [StringLength(100)]
        public string XZoneNo { get; set; }
        [Column("X_ZOneName")]
        [StringLength(500)]
        public string XZoneName { get; set; }
        [Column("X_LandMark")]
        [StringLength(500)]
        public string XLandMark { get; set; }
        [Column("N_AdviserID")]
        public int? NAdviserId { get; set; }
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
        [StringLength(500)]
        public string Remarks { get; set; }
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
        public int? NTypeId { get; set; }
        [Column("X_StopName")]
        [StringLength(20)]
        public string XStopName { get; set; }
        [Column("N_StopID")]
        public int? NStopId { get; set; }
        [Column("X_StopCode")]
        [StringLength(20)]
        public string XStopCode { get; set; }
        [StringLength(50)]
        public string DropRouteNo { get; set; }
        [StringLength(200)]
        public string DropRoute { get; set; }
        public int? DropRouteType { get; set; }
        [StringLength(20)]
        public string DropPointCode { get; set; }
        [StringLength(20)]
        public string DropPoint { get; set; }
        [Column("DropPointID")]
        public int? DropPointId { get; set; }
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_SeesionID")]
        public int? NSeesionId { get; set; }
        [Column("D_PickTime")]
        [StringLength(100)]
        public string DPickTime { get; set; }
        [Column("D_DropTime")]
        [StringLength(100)]
        public string DDropTime { get; set; }
        [Column("I_Photo", TypeName = "image")]
        public byte[] IPhoto { get; set; }
        [Column("D_PickStartDate", TypeName = "datetime")]
        public DateTime? DPickStartDate { get; set; }
        [Column("D_PickEndDate", TypeName = "datetime")]
        public DateTime? DPickEndDate { get; set; }
        [Column("D_DropStartDate", TypeName = "datetime")]
        public DateTime? DDropStartDate { get; set; }
        [Column("D_DropEndDate", TypeName = "datetime")]
        public DateTime? DDropEndDate { get; set; }
    }
}

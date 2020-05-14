using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchParentDetailsDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ParentID")]
        public int NParentId { get; set; }
        [Column("X_PFatherName")]
        [StringLength(200)]
        public string XPfatherName { get; set; }
        [Column("X_PMotherName")]
        [StringLength(200)]
        public string XPmotherName { get; set; }
        [Column("X_PAddress")]
        [StringLength(150)]
        public string XPaddress { get; set; }
        [Column("X_PCity")]
        [StringLength(50)]
        public string XPcity { get; set; }
        [Column("X_PNationalIDF")]
        [StringLength(50)]
        public string XPnationalIdf { get; set; }
        [Column("X_PNationalityF")]
        [StringLength(50)]
        public string XPnationalityF { get; set; }
        [Column("X_PPhoneNo")]
        [StringLength(50)]
        public string XPphoneNo { get; set; }
        [Column("X_PMobileNo")]
        [StringLength(50)]
        public string XPmobileNo { get; set; }
        [Column("X_PFamilyName")]
        [StringLength(50)]
        public string XPfamilyName { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("X_GaurdianName")]
        [StringLength(250)]
        public string XGaurdianName { get; set; }
        [Column("X_GaurdianName_Ar")]
        [StringLength(250)]
        public string XGaurdianNameAr { get; set; }
        [Column("N_RelationId")]
        public int? NRelationId { get; set; }
        [Column("N_EQualificationIdF")]
        public int? NEqualificationIdF { get; set; }
        [Column("X_CompanyF")]
        [StringLength(100)]
        public string XCompanyF { get; set; }
        [Column("X_JobF")]
        [StringLength(100)]
        public string XJobF { get; set; }
        [Column("X_Relation")]
        [StringLength(50)]
        public string XRelation { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("D_PNationalExpiryF", TypeName = "datetime")]
        public DateTime? DPnationalExpiryF { get; set; }
        [Column("X_PNationalIDM")]
        [StringLength(50)]
        public string XPnationalIdm { get; set; }
        [Column("X_PNationalityM")]
        [StringLength(50)]
        public string XPnationalityM { get; set; }
        [Column("X_CompanyM")]
        [StringLength(100)]
        public string XCompanyM { get; set; }
        [Column("X_JobM")]
        [StringLength(100)]
        public string XJobM { get; set; }
        [Column("D_PNationalExpiryM", TypeName = "date")]
        public DateTime? DPnationalExpiryM { get; set; }
        [Column("D_PPExpiryF", TypeName = "date")]
        public DateTime? DPpexpiryF { get; set; }
        [Column("D_PPExpiryM", TypeName = "date")]
        public DateTime? DPpexpiryM { get; set; }
        [Column("X_TelephoneF")]
        [StringLength(100)]
        public string XTelephoneF { get; set; }
        [Column("X_TelephoneM")]
        [StringLength(100)]
        public string XTelephoneM { get; set; }
        [Column("X_OfficeNoF")]
        [StringLength(100)]
        public string XOfficeNoF { get; set; }
        [Column("X_OfficeNoM")]
        [StringLength(100)]
        public string XOfficeNoM { get; set; }
        [Column("X_EmailF")]
        [StringLength(100)]
        public string XEmailF { get; set; }
        [Column("X_EmailM")]
        [StringLength(100)]
        public string XEmailM { get; set; }
        [Column("X_PPassportNoF")]
        [StringLength(50)]
        public string XPpassportNoF { get; set; }
        [Column("X_PPassportNoM")]
        [StringLength(50)]
        public string XPpassportNoM { get; set; }
        [Column("N_EQualificationIdM")]
        public int? NEqualificationIdM { get; set; }
        [Column("X_EQualificationM")]
        [StringLength(50)]
        public string XEqualificationM { get; set; }
        [Column("X_ParentCode")]
        [StringLength(100)]
        public string XParentCode { get; set; }
        [Column("X_BuildingName")]
        [StringLength(100)]
        public string XBuildingName { get; set; }
        [Column("X_StreetName")]
        [StringLength(100)]
        public string XStreetName { get; set; }
        [Column("X_ZoneName")]
        [StringLength(100)]
        public string XZoneName { get; set; }
        [Column("X_LandMark")]
        [StringLength(150)]
        public string XLandMark { get; set; }
        [Column("X_BuildingNo")]
        [StringLength(50)]
        public string XBuildingNo { get; set; }
        [Column("X_StreetNo")]
        [StringLength(50)]
        public string XStreetNo { get; set; }
        [Column("X_ZoneNo")]
        [StringLength(50)]
        public string XZoneNo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchAdmission
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_AdmittedClassID")]
        public int NAdmittedClassId { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("D_AdmissionDate", TypeName = "datetime")]
        public DateTime? DAdmissionDate { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("X_Gender")]
        [StringLength(10)]
        public string XGender { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("I_Photo", TypeName = "image")]
        public byte[] IPhoto { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("X_ContactPersonName1")]
        [StringLength(50)]
        public string XContactPersonName1 { get; set; }
        [Column("X_ContactPersonNo1")]
        [StringLength(25)]
        public string XContactPersonNo1 { get; set; }
        [Column("X_ContactPersonName2")]
        [StringLength(50)]
        public string XContactPersonName2 { get; set; }
        [Column("X_ContactPersonNo2")]
        [StringLength(25)]
        public string XContactPersonNo2 { get; set; }
        [Column("X_BloodGroup")]
        [StringLength(25)]
        public string XBloodGroup { get; set; }
        [Column("X_Alergies")]
        [StringLength(250)]
        public string XAlergies { get; set; }
        [Column("X_DisPhysicalAbility")]
        [StringLength(250)]
        public string XDisPhysicalAbility { get; set; }
        [Column("X_SpecialCareRequirement")]
        [StringLength(250)]
        public string XSpecialCareRequirement { get; set; }
        [Column("X_MedicationDetails")]
        [StringLength(250)]
        public string XMedicationDetails { get; set; }
        [Column("X_TransportationBusNo")]
        [StringLength(25)]
        public string XTransportationBusNo { get; set; }
        [Column("X_TransportationType")]
        [StringLength(25)]
        public string XTransportationType { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("X_AdmittedClass")]
        [StringLength(50)]
        public string XAdmittedClass { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_AdmittedDivisionID")]
        public int? NAdmittedDivisionId { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_CF1")]
        public int? NCf1 { get; set; }
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
        [Column("X_PFatherName")]
        [StringLength(200)]
        public string XPfatherName { get; set; }
        [Column("X_PMotherName")]
        [StringLength(200)]
        public string XPmotherName { get; set; }
        [Column("X_PFamilyName")]
        [StringLength(50)]
        public string XPfamilyName { get; set; }
        [Column("X_LedgerName_Ar")]
        [StringLength(100)]
        public string XLedgerNameAr { get; set; }
        [Required]
        [Column("X_AcYear")]
        [StringLength(50)]
        public string XAcYear { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
        [Column("X_NationalID")]
        [StringLength(50)]
        public string XNationalId { get; set; }
        [Column("N_ParentID")]
        public int? NParentId { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("X_PUserID")]
        [StringLength(50)]
        public string XPuserId { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
        [Column("N_VehicleID")]
        public int? NVehicleId { get; set; }
        [Column("N_Capacity")]
        public int? NCapacity { get; set; }
        [Column("X_VehicleName")]
        [StringLength(50)]
        public string XVehicleName { get; set; }
        [Column("X_Root")]
        [StringLength(50)]
        public string XRoot { get; set; }
        [Column("N_TransportationID")]
        public int? NTransportationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_Name_Ar")]
        [StringLength(250)]
        public string XNameAr { get; set; }
        [Column("X_BookCode")]
        [StringLength(20)]
        public string XBookCode { get; set; }
        [Column("X_GaurdianName")]
        [StringLength(250)]
        public string XGaurdianName { get; set; }
        [Column("X_GaurdianName_Ar")]
        [StringLength(250)]
        public string XGaurdianNameAr { get; set; }
        [Column("N_BookId")]
        public int? NBookId { get; set; }
        [Column("N_CountryOBId")]
        public int? NCountryObid { get; set; }
        [Column("X_COB")]
        [StringLength(100)]
        public string XCob { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Required]
        [Column("X_HouseName")]
        [StringLength(50)]
        public string XHouseName { get; set; }
        [Column("N_HouseID")]
        public int? NHouseId { get; set; }
        [Column("X_PlaceofBirth")]
        [StringLength(500)]
        public string XPlaceofBirth { get; set; }
        [Column("X_StudentMobile")]
        [StringLength(50)]
        public string XStudentMobile { get; set; }
        [Column("X_BridgePgm")]
        [StringLength(100)]
        public string XBridgePgm { get; set; }
        [Column("X_PassPortNo")]
        [StringLength(50)]
        public string XPassPortNo { get; set; }
        [Column("D_PPExpiryDate", TypeName = "datetime")]
        public DateTime? DPpexpiryDate { get; set; }
        [Column("X_LastSchoolAttnd")]
        [StringLength(500)]
        public string XLastSchoolAttnd { get; set; }
        [Column("X_LastSchoolGrade")]
        [StringLength(20)]
        public string XLastSchoolGrade { get; set; }
        [Column("X_LastSchoolyear")]
        [StringLength(20)]
        public string XLastSchoolyear { get; set; }
        [Column("N_Session")]
        public int? NSession { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_ParentCode")]
        [StringLength(100)]
        public string XParentCode { get; set; }
        [Column("N_TestScheduleID")]
        public int? NTestScheduleId { get; set; }
        [Column("N_ClubID")]
        public int? NClubId { get; set; }
        [Column("N_EmployeeID")]
        public int? NEmployeeId { get; set; }
        [Column("X_TransferReason")]
        [StringLength(500)]
        public string XTransferReason { get; set; }
        [Column("D_TransferDate", TypeName = "datetime")]
        public DateTime? DTransferDate { get; set; }
        [Column("X_AssesmentRemarks")]
        [StringLength(500)]
        public string XAssesmentRemarks { get; set; }
        [Column("D_TestScheduleDate", TypeName = "datetime")]
        public DateTime? DTestScheduleDate { get; set; }
        [Column("X_LastSchoolyear1")]
        [StringLength(20)]
        public string XLastSchoolyear1 { get; set; }
        [Column("X_LastSchoolGrade1")]
        [StringLength(20)]
        public string XLastSchoolGrade1 { get; set; }
        [Column("X_LastSchoolAttnd1")]
        [StringLength(500)]
        public string XLastSchoolAttnd1 { get; set; }
        [Column("X_BridgeProgramYear")]
        [StringLength(20)]
        public string XBridgeProgramYear { get; set; }
        [Column("X_RoomNo")]
        [StringLength(50)]
        public string XRoomNo { get; set; }
        [Column("X_LastName")]
        [StringLength(50)]
        public string XLastName { get; set; }
        [Column("X_MiddleName")]
        [StringLength(50)]
        public string XMiddleName { get; set; }
        [Column("X_Club")]
        [StringLength(50)]
        public string XClub { get; set; }
        [Column("X_TestResult")]
        [StringLength(50)]
        public string XTestResult { get; set; }
        [Column("X_Initial")]
        [StringLength(50)]
        public string XInitial { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("B_BridgeProgram")]
        public bool? BBridgeProgram { get; set; }
        [Column("X_GivenName")]
        [StringLength(200)]
        public string XGivenName { get; set; }
        [Column("D_LeftDate", TypeName = "datetime")]
        public DateTime? DLeftDate { get; set; }
        [Column("D_ReservaionDate", TypeName = "datetime")]
        public DateTime? DReservaionDate { get; set; }
        [Column("Room_No")]
        [StringLength(50)]
        public string RoomNo { get; set; }
        [Column("N_RegID")]
        public int? NRegId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_AdvisorID")]
        public int? NAdvisorId { get; set; }
        [Column("X_LRN")]
        [StringLength(50)]
        public string XLrn { get; set; }
    }
}

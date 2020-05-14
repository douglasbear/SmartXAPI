using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_ClientFile")]
    public partial class VsaClientFile
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Date", TypeName = "smalldatetime")]
        public DateTime DDate { get; set; }
        [Key]
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("N_FileStatusID")]
        public int? NFileStatusId { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("N_RegId")]
        public int? NRegId { get; set; }
        [Column("N_OccupationID")]
        public int? NOccupationId { get; set; }
        [Column("N_AssessmentID")]
        public int? NAssessmentId { get; set; }
        [Column("N_VisaSubClassID")]
        public int? NVisaSubClassId { get; set; }
        [Column("N_StatesID")]
        public int? NStatesId { get; set; }
        [Column("X_StatusNote")]
        [StringLength(200)]
        public string XStatusNote { get; set; }
        [Column("N_ConsultantID")]
        public int? NConsultantId { get; set; }
        [Column("N_ParalegalID")]
        public int? NParalegalId { get; set; }
        [Column("N_Point_EnglishID")]
        public int? NPointEnglishId { get; set; }
        [Column("N_Point_WorkID")]
        public int? NPointWorkId { get; set; }
        [Column("N_Point_QualificationID")]
        public int? NPointQualificationId { get; set; }
        [Column("N_Point_VsaClsID")]
        public int? NPointVsaClsId { get; set; }
        [Column("N_Point_OtherID")]
        public int? NPointOtherId { get; set; }
        [Column("N_Point_AgeValue")]
        public int? NPointAgeValue { get; set; }
        [Column("N_Point_EnglishValue")]
        public int? NPointEnglishValue { get; set; }
        [Column("N_Point_WorkValue")]
        public int? NPointWorkValue { get; set; }
        [Column("N_Point_QualificationValue")]
        public int? NPointQualificationValue { get; set; }
        [Column("N_Point_VsaClsValue")]
        public int? NPointVsaClsValue { get; set; }
        [Column("N_Point_OtherValue")]
        public int? NPointOtherValue { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("I_Image", TypeName = "image")]
        public byte[] IImage { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("D_FolloupDate", TypeName = "datetime")]
        public DateTime? DFolloupDate { get; set; }
        [Column("X_FolloupNote")]
        [StringLength(250)]
        public string XFolloupNote { get; set; }
        [Column("X_Note1")]
        [StringLength(500)]
        public string XNote1 { get; set; }
        [Column("X_Note2")]
        [StringLength(250)]
        public string XNote2 { get; set; }
        [Column("X_Note3")]
        [StringLength(250)]
        public string XNote3 { get; set; }
        [Column("X_VsaTRNno")]
        [StringLength(50)]
        public string XVsaTrnno { get; set; }
        [Column("X_VsaBCC")]
        [StringLength(50)]
        public string XVsaBcc { get; set; }
        [Column("N_MainStatus")]
        public int NMainStatus { get; set; }
        [Column("N_TotalContractAmount", TypeName = "money")]
        public decimal? NTotalContractAmount { get; set; }
    }
}

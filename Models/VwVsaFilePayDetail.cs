using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaFilePayDetail
    {
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("N_RegId")]
        public int? NRegId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Date", TypeName = "smalldatetime")]
        public DateTime DDate { get; set; }
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("N_FileStatusID")]
        public int? NFileStatusId { get; set; }
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
        [Column("N_PaymentID")]
        public int NPaymentId { get; set; }
        [Column("N_ItemCode")]
        public int? NItemCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Expr1 { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal NDiscount { get; set; }
        [Column("N_TaxCategoryId")]
        public int? NTaxCategoryId { get; set; }
        [Column("N_TaxPerc", TypeName = "money")]
        public decimal? NTaxPerc { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal? NTaxAmount { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_contryid")]
        public int? NContryid { get; set; }
        [Column("N_MainStatus")]
        public int NMainStatus { get; set; }
        [Column("FIle_statusID")]
        public int? FileStatusId { get; set; }
        [Column("File_StatusName")]
        [StringLength(500)]
        public string FileStatusName { get; set; }
        [Column("X_File_Description")]
        [StringLength(50)]
        public string XFileDescription { get; set; }
        [Column("X_StageName")]
        [StringLength(100)]
        public string XStageName { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("B_InvProcess")]
        public bool? BInvProcess { get; set; }
    }
}

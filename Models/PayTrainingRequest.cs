using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_TrainingRequest")]
    public partial class PayTrainingRequest
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_RequestID")]
        public int NRequestId { get; set; }
        [Column("X_RequestCode")]
        [StringLength(50)]
        public string XRequestCode { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_CourseID")]
        public int NCourseId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("X_Remarks")]
        [StringLength(300)]
        public string XRemarks { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("X_Venue")]
        [StringLength(200)]
        public string XVenue { get; set; }
        [Column("N_TrainingTypeID")]
        public int? NTrainingTypeId { get; set; }
        [Column("X_Subject")]
        [StringLength(200)]
        public string XSubject { get; set; }
        [Column("X_TrainingRsn")]
        [StringLength(500)]
        public string XTrainingRsn { get; set; }
        [Column("N_RegFee")]
        public int? NRegFee { get; set; }
        [Column("N_AccomodationFee")]
        public int? NAccomodationFee { get; set; }
        [Column("N_TravelCost")]
        public int? NTravelCost { get; set; }
        [Column("N_Otherfee")]
        public int? NOtherfee { get; set; }
    }
}

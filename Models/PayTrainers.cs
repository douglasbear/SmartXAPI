using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Trainers")]
    public partial class PayTrainers
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TraineeID")]
        public int NTraineeId { get; set; }
        [Column("X_TraineeCode")]
        [StringLength(20)]
        public string XTraineeCode { get; set; }
        [Column("X_TraineeName")]
        [StringLength(200)]
        public string XTraineeName { get; set; }
        [Column("X_TraineeAddress")]
        [StringLength(200)]
        public string XTraineeAddress { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(200)]
        public string XPhoneNo { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
    }
}

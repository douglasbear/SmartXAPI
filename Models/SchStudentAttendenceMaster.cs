﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentAttendence_Master")]
    public partial class SchStudentAttendenceMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_EntryID")]
        public int NEntryId { get; set; }
        [Column("X_EntryNo")]
        [StringLength(20)]
        public string XEntryNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_DivisionId")]
        public int? NDivisionId { get; set; }
        [Column("B_Email")]
        public bool? BEmail { get; set; }
        [Column("B_SMS")]
        public bool? BSms { get; set; }
    }
}

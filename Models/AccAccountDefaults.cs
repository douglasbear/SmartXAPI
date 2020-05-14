using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_AccountDefaults")]
    public partial class AccAccountDefaults
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_FieldDescr")]
        [StringLength(50)]
        public string XFieldDescr { get; set; }
        [Column("N_FieldValue")]
        public int? NFieldValue { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}

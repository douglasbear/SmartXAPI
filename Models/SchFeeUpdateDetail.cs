using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeUpdateDetail")]
    public partial class SchFeeUpdateDetail
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FeeUpdateID")]
        public int NFeeUpdateId { get; set; }
        [Key]
        [Column("N_FeeUpdateDetailID")]
        public int NFeeUpdateDetailId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}

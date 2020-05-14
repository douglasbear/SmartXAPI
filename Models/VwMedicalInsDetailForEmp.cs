using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMedicalInsDetailForEmp
    {
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_AdditionDate", TypeName = "datetime")]
        public DateTime? DAdditionDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("N_DependenceID")]
        public int? NDependenceId { get; set; }
        [Column("N_MedicalInsID")]
        public int NMedicalInsId { get; set; }
        [Column("N_PaycodeID")]
        public int? NPaycodeId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
    }
}

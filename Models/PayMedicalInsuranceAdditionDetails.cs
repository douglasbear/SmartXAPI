using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_MedicalInsuranceAdditionDetails")]
    public partial class PayMedicalInsuranceAdditionDetails
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_AdditionId")]
        public int NAdditionId { get; set; }
        [Key]
        [Column("N_AdditionDetailsId")]
        public int NAdditionDetailsId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_DependenceID")]
        public int NDependenceId { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("D_AdditionDate", TypeName = "datetime")]
        public DateTime? DAdditionDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
    }
}

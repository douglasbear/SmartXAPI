using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_MedicalInsuranceDeletionDetails")]
    public partial class PayMedicalInsuranceDeletionDetails
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_DeletionID")]
        public int NDeletionId { get; set; }
        [Key]
        [Column("N_DeletionDetailsID")]
        public int NDeletionDetailsId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_DependenceID")]
        public int NDependenceId { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("D_DeletionDate", TypeName = "datetime")]
        public DateTime DDeletionDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
    }
}

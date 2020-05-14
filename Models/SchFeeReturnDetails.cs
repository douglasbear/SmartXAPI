using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeReturnDetails")]
    public partial class SchFeeReturnDetails
    {
        [Key]
        [Column("N_ReturnID")]
        public int NReturnId { get; set; }
        [Key]
        [Column("N_ReturnDetailsID")]
        public int NReturnDetailsId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
        [Column("N_RecieptDetailsID")]
        public int? NRecieptDetailsId { get; set; }
        [Column("N_RecieptID")]
        public int? NRecieptId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}

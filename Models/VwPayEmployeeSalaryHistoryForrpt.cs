using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeSalaryHistoryForrpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_TransNo")]
        [StringLength(50)]
        public string XTransNo { get; set; }
        [Column("N_Type")]
        public int NType { get; set; }
    }
}

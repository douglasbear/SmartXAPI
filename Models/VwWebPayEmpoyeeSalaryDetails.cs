using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWebPayEmpoyeeSalaryDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_SalaryName")]
        [StringLength(100)]
        public string XSalaryName { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_Value", TypeName = "money")]
        public decimal? NValue { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("X_DescriptionLocale")]
        [StringLength(100)]
        public string XDescriptionLocale { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmpAccruls")]
    public partial class PayEmpAccruls
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_EmpAccID")]
        public int NEmpAccId { get; set; }
        [Column("N_EmpId")]
        public int NEmpId { get; set; }
        [Column("N_VacTypeID")]
        public int NVacTypeId { get; set; }
        [Column("N_Value", TypeName = "money")]
        public decimal? NValue { get; set; }

        [ForeignKey("NVacTypeId,NCompanyId")]
        [InverseProperty(nameof(PayVacationType.PayEmpAccruls))]
        public virtual PayVacationType N { get; set; }
        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.PayEmpAccruls))]
        public virtual AccCompany NCompany { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_FnYear")]
    public partial class AccFnYear
    {
        public AccFnYear()
        {
            AccPeriod = new HashSet<AccPeriod>();
            AccVoucherDetails = new HashSet<AccVoucherDetails>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_FnYearDescr")]
        [StringLength(20)]
        public string XFnYearDescr { get; set; }
        [Column("D_Start", TypeName = "smalldatetime")]
        public DateTime? DStart { get; set; }
        [Column("D_End", TypeName = "smalldatetime")]
        public DateTime? DEnd { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("B_TransferProcess")]
        public bool? BTransferProcess { get; set; }
        [Column("N_TaxType")]
        public int? NTaxType { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.AccFnYear))]
        public virtual AccCompany NCompany { get; set; }
        [InverseProperty("NFnYear")]
        public virtual ICollection<AccPeriod> AccPeriod { get; set; }
        [InverseProperty("NFnYear")]
        public virtual ICollection<AccVoucherDetails> AccVoucherDetails { get; set; }
    }
}

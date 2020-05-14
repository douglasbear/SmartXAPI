using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccVoucherDetailByMonthRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_Month")]
        [StringLength(30)]
        public string NMonth { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_GroupID")]
        public int NGroupId { get; set; }
        [Column("X_GroupCode")]
        [StringLength(50)]
        public string XGroupCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(100)]
        public string XGroupName { get; set; }
        [Column("X_ParentGroup")]
        [StringLength(152)]
        public string XParentGroup { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_MappingGroup")]
        public int? XMappingGroup { get; set; }
    }
}

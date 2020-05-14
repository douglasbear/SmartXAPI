using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDailyCollection
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CashAmount", TypeName = "money")]
        public decimal? NCashAmount { get; set; }
        [Column("N_BankAmount", TypeName = "money")]
        public decimal? NBankAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal? NetAmount { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_CashTypeID")]
        public int? NCashTypeId { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_Type")]
        public int NType { get; set; }
        [Column("PPaid", TypeName = "money")]
        public decimal? Ppaid { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("D_Day")]
        [StringLength(90)]
        public string DDay { get; set; }
        [Column(TypeName = "money")]
        public decimal? RetAmount { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("X_CashTypeBehaviour")]
        [StringLength(50)]
        public string XCashTypeBehaviour { get; set; }
    }
}

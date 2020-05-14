using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PayCodeGroupDetails")]
    public partial class PayPayCodeGroupDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_GroupId")]
        public int? NGroupId { get; set; }
        [Key]
        [Column("N_GroupDetailsID")]
        public int NGroupDetailsId { get; set; }
        [Column("N_PaycodeID")]
        public int? NPaycodeId { get; set; }
        [Column("N_PaycodeTypeID")]
        public int? NPaycodeTypeId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
    }
}

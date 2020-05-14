using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PayCodeItemwiseMappingDetails")]
    public partial class PayPayCodeItemwiseMappingDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_MappingId")]
        public int? NMappingId { get; set; }
        [Key]
        [Column("N_MappingDetailsID")]
        public int NMappingDetailsId { get; set; }
        [Column("N_PaycodeID")]
        public int? NPaycodeId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
    }
}

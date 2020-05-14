using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_CostCentreMaster")]
    public partial class AccCostCentreMaster
    {
        public AccCostCentreMaster()
        {
            InvCostCentreTransactions = new HashSet<InvCostCentreTransactions>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_CostCentreID")]
        public int NCostCentreId { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }

        [InverseProperty("NCostCentre")]
        public virtual ICollection<InvCostCentreTransactions> InvCostCentreTransactions { get; set; }
    }
}

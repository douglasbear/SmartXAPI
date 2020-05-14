using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_UserCategory")]
    public partial class SecUserCategory
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
        [Column("X_UserCategory")]
        [StringLength(50)]
        public string XUserCategory { get; set; }
        [Column("X_UserCategoryCode")]
        [StringLength(50)]
        public string XUserCategoryCode { get; set; }
        [Column("N_PwdExpPeriod")]
        public int? NPwdExpPeriod { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.SecUserCategory))]
        public virtual AccCompany NCompany { get; set; }
    }
}

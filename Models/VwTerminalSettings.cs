using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTerminalSettings
    {
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_ServiceCharge")]
        public double? NServiceCharge { get; set; }
        [Column("N_TerminalID")]
        public int? NTerminalId { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        public int? Expr1 { get; set; }
        [Column("X_TerminalName")]
        [StringLength(500)]
        public string XTerminalName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_PaymentMethodMaster")]
    public partial class AccPaymentMethodMaster
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_PaymentMethodID")]
        public int NPaymentMethodId { get; set; }
        [Column("X_PayMethod")]
        [StringLength(50)]
        public string XPayMethod { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("B_PaymentVoucher")]
        public bool? BPaymentVoucher { get; set; }
        [Column("B_ReceiptVoucher")]
        public bool? BReceiptVoucher { get; set; }
        [Column("B_VenderPayment")]
        public bool? BVenderPayment { get; set; }
        [Column("B_CustomerReceipt")]
        public bool? BCustomerReceipt { get; set; }
        [Column("B_SalesExePayment")]
        public bool? BSalesExePayment { get; set; }
        [Column("B_SalaryPayment")]
        public bool? BSalaryPayment { get; set; }
        [Column("B_GosiPayment")]
        public bool? BGosiPayment { get; set; }
        [Column("B_IsDefault")]
        public bool? BIsDefault { get; set; }
        [Column("B_Isfingerprint")]
        public bool? BIsfingerprint { get; set; }
        [Column("B_IsCheque")]
        public bool? BIsCheque { get; set; }
    }
}

using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using Domain.DomainModel.Entity.Lookup;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(AccountNumber), IsUnique = true, Name = "IDX_CUSTOMERACCOUNTNUMBER")]
    [Index(nameof(CustomerName), Name = "IDX_CUSTOMERCUSTOMERNAME")]
    [Index(nameof(CustomerTypeId), Name = "IDX_CUSTOMERCUSTOMERTYPE")]
    [Index(nameof(CustomerTypeId), Name = "IDX_CUSTOMERCUSTOMERTYPE")]
    [Index(nameof(PhoneNumber), Name = "IDX_CUSTOMERPHONENUMBER")]
    public class Customer : AddressBaseModel
    {
        #region Properties
        public int AccountNumber { get; set; }
        
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;
        
        [ForeignKey("FK_Customer_CustomerTypeId")]
        public int CustomerTypeId { get; set; }
        public CustomerType? CustomerType { get; set; }

        [ForeignKey("FK_Customer_PriceLevelId")]
        public int? PriceLevelId { get; set; }
        public PriceLevel? PriceLevel { get; set; }

        [ForeignKey("FK_Customer_PaymentTermId")]
        public int? PaymentTermId { get; set; }
        public PaymentTerm? PaymentTerm { get; set; }

        [ForeignKey("FK_Customer_SalesRepresentativeId")]
        public int? SalesRepresentativeOutId { get; set; }
        public SalesRepresentative? SalesRepresentativeOut { get; set; }

        [ForeignKey("FK_Customer_SalesRepresentativeId")]
        public int? SalesRepresentativeInId { get; set; }
        public SalesRepresentative? SalesRepresentativeIn { get; set; }

        [ForeignKey("FK_Customer_WarehouseId")]
        public int? DefaultWarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [MaxLength(200)]
        public string ContactName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? TaxRate { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? OverBalance { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? CreditLimit { get; set; }

        public bool IsBypassCreditLimit { get; set; } = false;
        
        public bool IsTaxable { get; set; } = false;
        
        public bool IsHoldAccount { get; set; } = false;

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Discount { get; set; }

        [MaxLength(15)]
        public string? SellersPermit { get; set; }
        
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? TrialBalance { get; set; }
        
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? AccountsReceivableBalance { get; set; }
        
        public string? CrossStreet { get; set; }

        public bool IsWithShippingCost { get; set; } = false;

        public bool IsWithHandlingCost { get; set; } = false;

        public bool IsAllowViewCA { get; set; } = false;

        public bool IsAllowViewVA { get; set; } = false;

        public bool IsAllowViewWAG { get; set; } = false;
        
        public bool IsOverrideCreditLimitRestriction { get; set; } = false;

        [MaxLength(500)]
        public string? ShipperAccountNumber { get; set; }

        public bool IsApplyPFGShippingCharge { get; set; } = false;

        public bool IsApplyPFGHandlingCharge { get; set; } = false;

        public bool IsPFGShippingHandlingLevel { get; set; } = false;

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? FreightPrepaidAmount { get; set; }
        #endregion
    }
}

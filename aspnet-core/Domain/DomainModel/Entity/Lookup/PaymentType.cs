using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;

namespace Domain.DomainModel.Entity.Lookup
{
    [Index(nameof(Name), nameof(Code), Name = "IDX_PAYMENTTYPELOOKUP")]
    public class PaymentType : LookupBaseModel
    {
    }
}

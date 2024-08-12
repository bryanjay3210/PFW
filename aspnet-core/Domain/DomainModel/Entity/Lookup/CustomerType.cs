using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;

namespace Domain.DomainModel.Entity.Lookup
{
    [Index(nameof(Name), nameof(Code), Name = "IDX_CUSTOMERTYPELOOKUP")]
    public class CustomerType : LookupBaseModel
    {
    }
}

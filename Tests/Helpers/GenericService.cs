using Domain.Enums;
using Domain.Models.Services;

namespace Tests.Helpers;

public class GenericService : BaseService
{
    public GenericService(int serviceId, string name, ServiceType type, decimal basePrice)
    {
        ServiceId = serviceId;
        Name = name;
        Type = type;
        BasePrice = basePrice;
    }
}

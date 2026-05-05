using THPARKING.Core.Models;

namespace THPARKING.Data.Repositories
{
    public interface IParkingCardRepository
    {
        ParkingCard FindByNormalizedCardCode(string normalizedCardCode);
    }
}

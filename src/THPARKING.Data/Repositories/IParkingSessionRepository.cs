using THPARKING.Core.Models;

namespace THPARKING.Data.Repositories
{
    public interface IParkingSessionRepository
    {
        ParkingSession FindOpenByNormalizedCardCode(string normalizedCardCode);

        void Add(ParkingSession session);

        void Update(ParkingSession session);
    }
}

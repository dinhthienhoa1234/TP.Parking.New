using THPARKING.Core.Enums;

namespace THPARKING.Business.Lanes
{
    public class LaneSwitchResult
    {
        public bool Success { get; private set; }

        public string LaneCode { get; private set; }

        public LaneSide Side { get; private set; }

        public LaneDirection OldDirection { get; private set; }

        public LaneDirection NewDirection { get; private set; }

        public string Message { get; private set; }

        public static LaneSwitchResult Ok(
            string laneCode,
            LaneSide side,
            LaneDirection oldDirection,
            LaneDirection newDirection,
            string message)
        {
            return new LaneSwitchResult
            {
                Success = true,
                LaneCode = laneCode,
                Side = side,
                OldDirection = oldDirection,
                NewDirection = newDirection,
                Message = message
            };
        }

        public static LaneSwitchResult Fail(
            string laneCode,
            LaneSide side,
            LaneDirection currentDirection,
            string message)
        {
            return new LaneSwitchResult
            {
                Success = false,
                LaneCode = laneCode,
                Side = side,
                OldDirection = currentDirection,
                NewDirection = currentDirection,
                Message = message
            };
        }
    }
}
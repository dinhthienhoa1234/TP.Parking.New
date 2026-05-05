using System;
using System.Collections.Generic;
using System.Linq;
using THPARKING.Core.Enums;

namespace THPARKING.Business.Lanes
{
    public class LaneRouterService
    {
        private readonly IList<ParkingLaneConfig> _lanes;
        private readonly IDictionary<string, LaneRuntimeState> _states;

        public LaneRouterService(IEnumerable<ParkingLaneConfig> lanes)
        {
            if (lanes == null)
                throw new ArgumentNullException(nameof(lanes));

            _lanes = lanes.Where(x => x != null).ToList();

            _states = _lanes
                .Where(x => !string.IsNullOrWhiteSpace(x.LaneCode))
                .GroupBy(x => x.LaneCode)
                .ToDictionary(
                    g => g.Key,
                    g => new LaneRuntimeState
                    {
                        LaneCode = g.Key
                    });
        }

        public ParkingLaneConfig GetLaneBySide(LaneSide side)
        {
            return _lanes.FirstOrDefault(x => x.Side == side && x.IsActive);
        }

        public ParkingLaneConfig GetLaneByReaderPort(string readerPortName)
        {
            if (string.IsNullOrWhiteSpace(readerPortName))
                return null;

            return _lanes.FirstOrDefault(x =>
                x.IsActive &&
                string.Equals(x.ReaderPortName, readerPortName, StringComparison.OrdinalIgnoreCase));
        }

        public LaneDirection GetDirectionByReaderPort(string readerPortName)
        {
            var lane = GetLaneByReaderPort(readerPortName);
            return lane == null ? LaneDirection.Unknown : lane.Direction;
        }

        public LaneRuntimeState GetState(string laneCode)
        {
            if (string.IsNullOrWhiteSpace(laneCode))
                return null;

            LaneRuntimeState state;
            return _states.TryGetValue(laneCode, out state) ? state : null;
        }

        public LaneSwitchResult SwitchLeftLane()
        {
            return SwitchLane(LaneSide.Left);
        }

        public LaneSwitchResult SwitchRightLane()
        {
            return SwitchLane(LaneSide.Right);
        }

        public LaneSwitchResult SwitchLane(LaneSide side)
        {
            var lane = GetLaneBySide(side);

            if (lane == null)
            {
                return LaneSwitchResult.Fail(
                    null,
                    side,
                    LaneDirection.Unknown,
                    "Không tìm thấy cấu hình làn.");
            }

            var state = GetState(lane.LaneCode);

            if (state != null && state.IsBusy)
            {
                return LaneSwitchResult.Fail(
                    lane.LaneCode,
                    side,
                    lane.Direction,
                    "Không thể đổi làn vì làn đang xử lý.");
            }

            if (state != null && state.PendingConfirm)
            {
                return LaneSwitchResult.Fail(
                    lane.LaneCode,
                    side,
                    lane.Direction,
                    "Không thể đổi làn vì làn đang chờ xác nhận.");
            }

            var oldDirection = lane.Direction;
            var newDirection = ToggleDirection(oldDirection);

            if (newDirection == LaneDirection.Unknown)
            {
                return LaneSwitchResult.Fail(
                    lane.LaneCode,
                    side,
                    oldDirection,
                    "Hướng làn không hợp lệ.");
            }

            lane.Direction = newDirection;

            return LaneSwitchResult.Ok(
                lane.LaneCode,
                side,
                oldDirection,
                newDirection,
                "Đổi hướng làn thành công.");
        }

        public static LaneDirection ToggleDirection(LaneDirection direction)
        {
            if (direction == LaneDirection.In)
                return LaneDirection.Out;

            if (direction == LaneDirection.Out)
                return LaneDirection.In;

            return LaneDirection.Unknown;
        }
    }
}
namespace THPARKING.Business.Lanes
{
    public class LaneRuntimeState
    {
        public string LaneCode { get; set; }

        public bool IsBusy { get; set; }

        public bool PendingConfirm { get; set; }

        public string LastCardCode { get; set; }

        public string LastMessage { get; set; }

        public void Reset()
        {
            IsBusy = false;
            PendingConfirm = false;
            LastCardCode = null;
            LastMessage = null;
        }
    }
}
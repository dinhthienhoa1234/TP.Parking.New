using System.Windows.Forms;
using THPARKING.App.WinForms.Bootstrap;

namespace THPARKING.App.WinForms
{
    public partial class Form1 : Form
    {
        private readonly ParkingAppRuntime _runtime;

        public Form1()
            : this(null)
        {
        }

        public Form1(ParkingAppRuntime runtime)
        {
            _runtime = runtime;

            InitializeComponent();

            ApplyRuntimeStatusToUi();
        }

        private void ApplyRuntimeStatusToUi()
        {
            if (_runtime == null)
            {
                Text = "TH.Parking - Runtime chưa khởi tạo";
                return;
            }

            Text = "TH.Parking - Runtime đã khởi tạo";
        }
    }
}
namespace THPARKING.Licensing.Core
{
    public class LicenseWarningPresenter
    {
        public string BuildWarningMessage(LicenseCheckResult result)
        {
            if (result == null)
                return "Không kiểm tra được trạng thái license.";

            if (result.IsValid && result.RemainingDays <= 3)
            {
                return "License còn " + result.RemainingDays + " ngày. Vui lòng gia hạn để tránh gián đoạn.";
            }

            if (!result.IsValid)
                return result.Message;

            return string.Empty;
        }
    }
}
namespace NMC.Client.Services
{
    public static class ApiConfig
    {
        public const int Port = 5000;
        public const int TimeoutSeconds = 60;

        public static string BaseAddress
        {
            get
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    return $"http://10.0.2.2:{Port}/";
                }

                return $"http://localhost:{Port}/";
            }
        }
    }
}

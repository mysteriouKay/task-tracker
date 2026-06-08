namespace TransportSystem.Services
{
    public class SmsService
    {
        private readonly string _username = "sandbox";
        private readonly string _apiKey = "atsk_174501ee22f87c788d51c5fb3cdd51122ee37bfe3dca48dd2d25e160bede0abf373f3385";
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            Console.WriteLine($"[SMS] Attempting to send to {phoneNumber}");
            try
            {
                var url = "https://api.sandbox.africastalking.com/version1/messaging";

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", _username),
                    new KeyValuePair<string, string>("to", phoneNumber),
                    new KeyValuePair<string, string>("message", message),
                });

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("apiKey", _apiKey);
                request.Headers.Add("Accept", "application/json");
                request.Content = content;

                Console.WriteLine($"[SMS] Sending request to {url}");
                var response = await _httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[SMS] Status: {response.StatusCode}");
                Console.WriteLine($"[SMS] Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMS] FAILED: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[SMS] Inner: {ex.InnerException.Message}");
            }
        }
    }
}
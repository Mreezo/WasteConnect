using Microsoft.Extensions.Caching.Memory;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace WasteConnect.Services
{
    public class TwilioOtpService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public TwilioOtpService(
            IConfiguration configuration,
            IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }

        public async Task SendReportOtpAsync(string phoneNumber)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:FromNumber"];

            if (string.IsNullOrWhiteSpace(accountSid) ||
                string.IsNullOrWhiteSpace(authToken) ||
                string.IsNullOrWhiteSpace(fromNumber))
            {
                throw new InvalidOperationException("Twilio settings are missing.");
            }

            var formattedPhone = FormatSouthAfricanNumber(phoneNumber);

            var otp = new Random().Next(100000, 999999).ToString();

            _cache.Set(
                $"ReportOtp_{formattedPhone}",
                otp,
                TimeSpan.FromMinutes(5));

            TwilioClient.Init(accountSid, authToken);

            await MessageResource.CreateAsync(
                body: $"Your WasteConnect report verification code is {otp}. It expires in 5 minutes.",
                from: new PhoneNumber(fromNumber),
                to: new PhoneNumber(formattedPhone));
        }

        public bool VerifyReportOtp(string phoneNumber, string otp)
        {
            var formattedPhone = FormatSouthAfricanNumber(phoneNumber);

            var savedOtp = _cache.Get<string>($"ReportOtp_{formattedPhone}");

            if (string.IsNullOrWhiteSpace(savedOtp))
                return false;

            if (savedOtp != otp)
                return false;

            _cache.Remove($"ReportOtp_{formattedPhone}");

            return true;
        }

        private string FormatSouthAfricanNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return phoneNumber;

            phoneNumber = phoneNumber.Trim().Replace(" ", "");

            if (phoneNumber.StartsWith("+"))
                return phoneNumber;

            if (phoneNumber.StartsWith("0"))
                return "+27" + phoneNumber.Substring(1);

            return phoneNumber;
        }
    }
}
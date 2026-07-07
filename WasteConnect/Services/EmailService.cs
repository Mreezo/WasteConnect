using Azure;
using Azure.Communication.Email;

namespace WasteConnect.Services
{
    public class EmailService
    {
        private readonly EmailClient _emailClient;
        private readonly string _senderAddress;

        public EmailService(IConfiguration configuration)
        {
            var connectionString =
                configuration["AzureCommunicationEmail:ConnectionString"];

            _senderAddress =
                configuration["AzureCommunicationEmail:SenderAddress"];

            _emailClient = new EmailClient(connectionString);
        }

        public async Task SendPasswordResetCodeAsync(string toEmail, string code)
        {
            var subject = "WasteConnect Password Reset Code";

            var htmlContent = $@"
                <div style='font-family:Arial,sans-serif;background:#f4f7f3;padding:30px;'>
                    <div style='max-width:520px;margin:auto;background:white;border-radius:16px;padding:30px;'>
                        <h2 style='color:#1f8f4d;'>WasteConnect Password Reset</h2>

                        <p>Hello,</p>

                        <p>Your password reset verification code is:</p>

                        <div style='font-size:32px;font-weight:bold;letter-spacing:6px;color:#102a43;background:#e7f7ec;padding:18px;border-radius:12px;text-align:center;'>
                            {code}
                        </div>

                        <p style='margin-top:25px;'>
                            This code will expire in 10 minutes.
                        </p>

                        <p>
                            If you did not request this, you can safely ignore this email.
                        </p>

                        <hr style='border:none;border-top:1px solid #e5e7eb;margin:25px 0;' />

                        <p style='font-size:13px;color:#64748b;'>
                            WasteConnect - Cleaner communities, better tomorrow.
                        </p>
                    </div>
                </div>";

            var plainTextContent =
                $"Your WasteConnect password reset code is {code}. This code expires in 10 minutes.";

            await _emailClient.SendAsync(
                WaitUntil.Completed,
                senderAddress: _senderAddress,
                recipientAddress: toEmail,
                subject: subject,
                htmlContent: htmlContent,
                plainTextContent: plainTextContent
            );
        }
    }
}
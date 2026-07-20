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

        public async Task SendCouncillorPasswordSetupAsync(
        string toEmail,
        string councillorName,
        int wardNumber,
        string setupLink)
        {
            var subject = "Welcome to WasteConnect – Create Your Password";

            var htmlContent = $@"
        <div style='font-family:Arial,sans-serif;background:#f4f7f3;padding:30px;'>
            <div style='max-width:560px;margin:auto;background:#ffffff;border-radius:16px;padding:32px;
                        box-shadow:0 8px 24px rgba(15,23,42,0.08);'>

                <h2 style='color:#1f8f4d;margin-bottom:8px;'>
                    Welcome to WasteConnect
                </h2>

                <p style='color:#475569;margin-top:0;'>
                    Councillor Account Invitation
                </p>

                <p>Hello {councillorName},</p>

                <p>
                    A WasteConnect councillor account has been created for you.
                </p>

                <div style='background:#f0f9f4;border-left:4px solid #1f8f4d;
                            padding:16px;border-radius:8px;margin:22px 0;'>
                    <strong>Assigned Ward:</strong> Ward {wardNumber}
                </div>

                <p>
                    Please click the button below to create your private password
                    and activate your account.
                </p>

                <div style='text-align:center;margin:30px 0;'>
                    <a href='{setupLink}'
                       style='display:inline-block;background:#1f8f4d;color:#ffffff;
                              text-decoration:none;padding:14px 26px;border-radius:10px;
                              font-weight:bold;'>
                        Create My Password
                    </a>
                </div>

                <p style='font-size:14px;color:#64748b;'>
                    For your security, do not share this link with anyone.
                </p>

                <p style='font-size:14px;color:#64748b;'>
                    If you were not expecting this invitation, please contact
                    the WasteConnect administrator.
                </p>

                <hr style='border:none;border-top:1px solid #e5e7eb;margin:25px 0;' />

                <p style='font-size:13px;color:#64748b;'>
                    WasteConnect – Cleaner communities, better tomorrow.
                </p>
            </div>
        </div>";

            var plainTextContent =
                $"Hello {councillorName}, a WasteConnect councillor account " +
                $"has been created for Ward {wardNumber}. " +
                $"Create your password using this link: {setupLink}";

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
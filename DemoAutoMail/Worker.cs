using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using DemoAutoMail.Data;
using DemoAutoMail.Models;

namespace DemoAutoMail
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly DbContextOptions<AutomailDbContext> _dbContextOptions;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            var optionsBuilder = new DbContextOptionsBuilder<AutomailDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConnStr"));
            _dbContextOptions = optionsBuilder.Options;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var dbContext = new AutomailDbContext(_dbContextOptions))
                    {
                        var pendingEmails = await dbContext.Automail.Where(e => !e.IsSend).ToListAsync();

                        foreach (var email in pendingEmails)
                        {
                            SendEmail(email);

                            email.IsSend = true;
                        }

                        await dbContext.SaveChangesAsync();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing and sending emails.");
                }
            }
        }


        private void SendEmail(Automail email)
        {
            try
            {
                var smtpClient = ConfigureSmtpClient();

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email.Sender),
                    Subject = email.Subject,
                    Body = email.Body,
                };

                mailMessage.To.Add(email.EmailTo);

                if (!string.IsNullOrEmpty(email.CC))
                {
                    var ccAddresses = email.CC.Split(';');
                    foreach (var cc in ccAddresses)
                    {
                        mailMessage.CC.Add(cc.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(email.BCC))
                {
                    var bccAddresses = email.BCC.Split(';');
                    foreach (var bcc in bccAddresses)
                    {
                        mailMessage.Bcc.Add(bcc.Trim());
                    }
                }

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to: {email.EmailTo}, Subject: {email.Subject}");
            }
        }



        private SmtpClient ConfigureSmtpClient()
        {
            var smtpClient = new SmtpClient
            {
                Host = _configuration["Smtp:Host"],
                Port = int.Parse(_configuration["Smtp:Port"]),
                EnableSsl = bool.Parse(_configuration["Smtp:EnableSsl"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"])
            };

            return smtpClient;
        }
    }
}

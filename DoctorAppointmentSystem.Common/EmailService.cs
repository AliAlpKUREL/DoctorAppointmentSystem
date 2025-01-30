using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DoctorAppointmentSystem.Common;

public class EmailService 
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _from;
    private readonly string _username;
    private readonly string _password;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration)
    {
        var smtpSettings = configuration.GetSection("Smtp");
        _host = smtpSettings["Host"];
        _port = int.Parse(smtpSettings["Port"]);
        _from = smtpSettings["From"];
        _username = smtpSettings["Username"];
        _password = smtpSettings["Password"];
        _enableSsl = bool.Parse(smtpSettings["EnableSsl"]);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_from),
            Subject = subject,
            Body = body,
            IsBodyHtml = false 
        };

        mailMessage.To.Add(toEmail);

        using var smtpClient = new SmtpClient(_host, _port);
        smtpClient.Credentials = new NetworkCredential(_username, _password);
        smtpClient.EnableSsl = _enableSsl;
        await smtpClient.SendMailAsync(mailMessage);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeniiApp.Mail
{
    public interface IEmailConfiguration
    {
        string SmtpServer { get; }
        int SmtpPort { get; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
    }
}

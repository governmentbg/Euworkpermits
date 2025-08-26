using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Data.Models.Common
{
    public enum EmailStatus
    {
        [Description("Предстоящо изпращане")]
        Pending = 1,

        [Description("Изпратен")]
        Sent = 2,

        [Description("Грешка")]
        UknownError = 3,
    }
    public class EMail
    {
        public long Id { get; set; }
        public string? Type { get; set; }
        public string? FeedBackName { get; set; }
        public string? Message { get; set; }
        public string? FeedBackEmail { get; set; }
        public string? MailTemplateName { get; set; }
        public string? Context { get; set; }
        public EmailStatus Status { get; set; }
        public int FailedAttempts { get; set; }
        public string? FailedAttemptsErrors { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}

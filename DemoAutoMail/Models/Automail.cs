using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAutoMail.Models
{
    public class Automail
    {
        [Key]
        public Guid ROWID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Sender { get; set; }
        public string EmailTo { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public bool IsSend { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

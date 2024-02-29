using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Core.ExtensionsMethods
{
    public class SmtpValues
    {
        public string SmtpServer{get;set;}
        public int SmtpPort{get;set;}
        public string SmtpUsername{get;set;}
        public string SmtpPassword{get;set;}
        public string SmtpDisplayName{get;set;}
        public bool SmtpEnableSsl { get; set; }
        public string SmtpDisplayAddress { get; set; }
    }
}

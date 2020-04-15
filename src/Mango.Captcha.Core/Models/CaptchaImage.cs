using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Captcha.Core.Models
{
    public class CaptchaImage
    {
        public Guid Id { get; set; }

        public CaptchaCategory Category { get; set; }
        public string Path { get; set; }

        public bool IsCheck { get; set; }
    }
}

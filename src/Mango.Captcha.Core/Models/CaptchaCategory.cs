using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Captcha.Core.Models
{
    public class CaptchaCategory
    {
        public string Name { get; set; }

        public IList<CaptchaImage> Images { get; set; }
    }
}

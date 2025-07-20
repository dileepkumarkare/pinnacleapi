using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
   
        public class ConfigEntity
        {
            [Key]
            public int? ConfigId { get; set; }
            public string ConfigKey { get; set; }
            public string ConfigValue { get; set; }
            public int AddedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }
        }
        [Keyless]
        public class ConfigKeyInfo
        {
            public string SMTP_HOST { get; set; }
            public string SMTP_PORT { get; set; }
            public string SMTP_USERNAME { get; set; }
            public string SMTP_PASSWORD { get; set; }
            public string HOME_URL { get; set; }
            public string HrMail { get; set; }
        }
    }


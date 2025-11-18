using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Domain.Entities
{

    [SugarTable("yjcabinUsers")]
    public class User : EntityBase
    {
        public string Account { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }

        public string? Avatar { get; set; }

        public string? Pk { get; set; }

    }
}

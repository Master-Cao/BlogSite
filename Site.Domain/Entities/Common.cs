using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Site.Domain.Entities
{
    public class Common : EntityBase
    {
        [SugarColumn(ColumnDataType = "varchar(100)")]
        public string CommonContent { get; set; }

        [SugarColumn(ColumnDataType = "varchar(25)")]
        public string BlogId { get; set; }
    }
}

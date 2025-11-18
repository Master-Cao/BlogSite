using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Domain.Entities
{
    public class DefaultImage : EntityBase
    {
        [SugarColumn(ColumnDataType = "varchar(255)")]
        public string Url { get; set; }
    }
}

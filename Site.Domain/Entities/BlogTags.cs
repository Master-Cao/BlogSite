using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Site.Domain.Entities
{
    public class BlogTags : EntityBase
    {
        [SugarColumn(ColumnDataType = "varchar(25)")]
        public string TagName { get; set; }

        [SugarColumn(ColumnDataType = "varchar(20)")]
        public string SubTagName { get; set; }

        [SugarColumn(ColumnDataType = "longtext")]
        public string Icon { get; set; }

        [SugarColumn(ColumnDataType = "varchar(125)")]
        public string Color { get; set; }
    }
}

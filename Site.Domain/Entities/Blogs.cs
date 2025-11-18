using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Site.Domain.Entities
{
    public class Blogs : EntityBase
    {
        [SugarColumn(ColumnDataType = "varchar(20)")]
        public string Title { get; set; }

        [SugarColumn(ColumnDataType = "varchar(200)")]
        public string Summary { get; set; }

        [SugarColumn(ColumnDataType = "longtext")]
        public string Content { get; set; }

        [SugarColumn(ColumnDataType = "varchar(100)")]
        public string? CoverImage { get; set; }

        [SugarColumn(ColumnDataType = "longtext")]
        public string ContentHtml { get; set; }

        [SugarColumn(ColumnDataType = "varchar(100)")]
        public string Tages { get; set; }

        [SugarColumn(ColumnDataType = "longtext")]
        public string? ImageIds { get; set; }
        
        public int State { get; set; }
        public int Common { get; set; }
        public int View { get; set; }

    }
}

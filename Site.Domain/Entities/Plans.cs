using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SqlSugar;

namespace Site.Domain.Entities
{
    public class Plans
    {
        [SugarTable("plans")]
        public class PlansModel : EntityBase
        {
            [SugarColumn(ColumnDataType = "varchar(20)")]
            public string Title { get; set; }

            [SugarColumn(ColumnDataType = "longtext")]
            public string? Content { get; set; }

            [SugarColumn(ColumnDataType = "varchar(125)")]
            public string Description { get; set; }
            
            [SugarColumn(ColumnDataType = "varchar(255)")]
            public string? CoverImage { get; set; }

            public bool IsComplete { get; set; } = false;

            public DateTime? Dealine { get; set; }

        }
    }
}

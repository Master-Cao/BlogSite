using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SqlSugar;

namespace Site.Domain.Entities
{
    public class EntityBase
    {
        private string _id = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "主键")]
        public string Id
        {
            get => _id;
            set => _id = string.IsNullOrEmpty(value) ? Guid.NewGuid().ToString("N") : value;
        }
        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnDescription = "是否删除")]
        public bool IsDeleted { get; set; } = false;
        /// <summary>
        /// 创建者Id
        /// </summary>
        [SugarColumn(ColumnDescription = "创建者Id")]
        public string? CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 删除者Id
        /// </summary>
        [SugarColumn(ColumnDescription = "删除者Id")]
        public string? DeleteUserId { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        [SugarColumn(ColumnDescription = "删除时间")]
        public DateTime? DeleteTime { get; set; }
    }
}

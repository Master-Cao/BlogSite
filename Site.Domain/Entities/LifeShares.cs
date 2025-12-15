using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SqlSugar;

namespace Site.Domain.Entities
{
    /// <summary>
    /// 生活分享实体
    /// </summary>
    [SugarTable("LifeShares")]
    public class LifeShares : EntityBase
    {
        /// <summary>
        /// 标题（最大25字符）
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(30)", IsNullable = false, ColumnDescription = "标题")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 内容（Markdown格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", IsNullable = false, ColumnDescription = "内容")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 封面图片URL
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true, ColumnDescription = "封面图片")]
        public string? CoverImage { get; set; }

        /// <summary>
        /// 图片列表（JSON格式存储）
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "图片列表")]
        public string? Images { get; set; }

        /// <summary>
        /// 分类：life/travel/food/thoughts/tech/other
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(20)", IsNullable = false, ColumnDescription = "分类")]
        public string Category { get; set; } = "life";

        /// <summary>
        /// 标签（逗号分隔）
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(200)", IsNullable = true, ColumnDescription = "标签")]
        public string? Tags { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        [SugarColumn(ColumnDescription = "浏览量")]
        public int ViewCount { get; set; } = 0;

        /// <summary>
        /// 点赞数
        /// </summary>
        [SugarColumn(ColumnDescription = "点赞数")]
        public int LikeCount { get; set; } = 0;

        /// <summary>
        /// 作者昵称（冗余字段，提升查询效率）
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(50)", IsNullable = true, ColumnDescription = "作者昵称")]
        public string? AuthorName { get; set; }

        /// <summary>
        /// 作者头像URL（冗余字段，提升查询效率）
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar(300)", IsNullable = true, ColumnDescription = "作者头像")]
        public string? AuthorAvatar { get; set; }
    }
}


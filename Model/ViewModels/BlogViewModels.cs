﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    /// <summary>
    /// 博客信息展示类
    /// </summary>
    public class BlogViewModels
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long bID { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string bsubmitter { get; set; }

        /// <summary>
        /// 博客标题
        /// </summary>
        public string btitle { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string digest { get; set; }

        /// <summary>
        /// 上一篇
        /// </summary>
        public string previous { get; set; }

        /// <summary>
        /// 上一篇id
        /// </summary>
        public long previousID { get; set; }

        /// <summary>
        /// 下一篇
        /// </summary>
        public string next { get; set; }

        /// <summary>
        /// 下一篇id
        /// </summary>
        public long nextID { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string bcategory { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string bcontent { get; set; }

        /// <summary>
        /// 访问量
        /// </summary>
        public int btraffic { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int bcommentNum { get; set; }

        /// <summary> 
        /// 修改时间
        /// </summary>
        public DateTime bUpdateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime bCreateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string bRemark { get; set; }
    }
}

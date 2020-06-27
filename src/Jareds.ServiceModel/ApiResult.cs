using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceModel
{
    /// <summary>
    /// 接口返回值
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public string RetCode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string RetMsg { get; set; }        
    }
    /// <summary>
    /// 接口返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 数据域
        /// </summary>
        public T Data { get; set; }        
    }
    /// <summary>
    /// 分页返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiPaginationResult<T> : ApiResult<Pagination<T>>
    {

    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pagination<T>
    {
        /// <summary>
        /// 数据总量
        /// </summary>
        public long Total { get; set; }
        /// <summary>
        /// 数据集合
        /// </summary>
        public List<T> Array { get; set; }
    }
}

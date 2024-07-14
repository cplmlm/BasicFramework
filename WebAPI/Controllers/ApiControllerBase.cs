using BasicCommonProject.Result;
using Microsoft.AspNetCore.Mvc;

namespace BasicCommonProject.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// 成功状态返回结果
        /// </summary>
        /// <param name="result">返回的数据</param>
        /// <returns></returns>
        protected ResponseResult<T> SuccessResult<T>(T result)
        {
            return ResponseResult<T>.SuccessResult(result);
        }

        /// <summary>
        /// 失败状态返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="msg">失败信息</param>
        /// <returns></returns>
        protected ResponseResult<T> FailResult<T>(string? msg = null)
        {
            return ResponseResult<T>.FailResult(msg);
        }

        /// <summary>
        /// 异常状态返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="msg">异常信息</param>
        /// <returns></returns>
        protected ResponseResult<T> ErrorResult<T>(string? msg = null)
        {
            return ResponseResult<T>.ErrorResult(msg);
        }

        /// <summary>
        /// 自定义状态返回结果
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected ResponseResult<T> Result<T>(ResultStatus status, T result, string? msg = null)
        {
            return ResponseResult<T>.Result(status, result, msg);
        }
    }

}

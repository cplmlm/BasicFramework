using BasicCommonProject.Result;
using Microsoft.AspNetCore.Mvc;

namespace BasicCommonProject.Controllers
{
    [Produces("application/json")]
    [Route("/v1/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ProducesResponseType(typeof(ApiResult<object>), 200)]
    [ApiController]
    public class BaseController : ControllerBase
    {

        protected IActionResult Success()
        {
            return Ok(new ApiResult<object> { Code = ResultStatus.Success });
        }

        protected IActionResult Success(string msg)
        {
            return Ok(new ApiResult<object> { Code = ResultStatus.Success, Message = msg });
        }

        protected IActionResult Success<T>(string msg, T data)
        {
            return Ok(new ApiResult<T> { Code = ResultStatus.Success, Message = msg, Data = data });
        }

        protected IActionResult Fail()
        {
            return Ok(new ApiResult<object> { Code = ResultStatus.Fail });
        }

        protected IActionResult Fail(string msg)
        {
            return Ok(new ApiResult<object> { Code = ResultStatus.Fail, Message = msg });
        }

        protected IActionResult Fail<T>(string msg, T data)
        {
            return Ok(new ApiResult<T> { Code = ResultStatus.Fail, Message = msg, Data = data });
        }

        protected IActionResult ApiResult(bool success, string msg)
        {
            return Ok(new ApiResult<object> { Code = success ? ResultStatus.Success : ResultStatus.Fail, Message = msg });
        }

        //protected IActionResult ApiResult<T>((bool success, string msg, T Data) result)
        //{
        //    return Ok(new ApiResult<T> { Code = result.Success ? ResultStatus.Success : ResultStatus.Fail, Message = result.msg, Data = result.Data });
        //}
    }
}

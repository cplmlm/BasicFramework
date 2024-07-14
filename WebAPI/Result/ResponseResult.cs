using Microsoft.AspNetCore.Mvc;

namespace BasicCommonProject.Result
{
    public class ResponseResult<T>
    {

        public ResultStatus ResultStatus { get; set; }

        private string? message;
        //public string? Message
        //{
        //    get => string.IsNullOrEmpty(message)?EnumHelper.GetDescription(ResultStatus):message;
        //    set => message = value;
        //}
        /// <summary>
        /// 状态结果
        /// </summary>
        public ResultStatus Status { get; set; } = ResultStatus.Success;


        private string? _msg;

        /// <summary>
        /// 消息描述
        /// </summary>
        public string? Message
        {
            get
            {
                return !string.IsNullOrEmpty(_msg) ? _msg : EnumHelper.GetDescription(Status);
            }
            set
            {
                _msg = value;
            }
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 成功状态返回结果
        /// </summary>
        /// <param name="result">返回的数据</param>
        /// <returns></returns>
        public static ResponseResult<T> SuccessResult(T data)
        {
            return new ResponseResult<T> { Status = ResultStatus.Success, Data = data };
        }

        public static ResponseResult<T> OkResult(T data)
        {
            return new ResponseResult<T> { Data = data, Status = ResultStatus.Success };
        }
        /// <summary>
        /// 失败状态返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="msg">失败信息</param>
        /// <returns></returns>
        public static ResponseResult<T> FailResult(string? msg = null)
        {
            return new ResponseResult<T> { Status = ResultStatus.Fail, Message = msg };
        }

        /// <summary>
        /// 异常状态返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="msg">异常信息</param>
        /// <returns></returns>
        public static ResponseResult<T> ErrorResult(string? msg = null)
        {
            return new ResponseResult<T> { Status = ResultStatus.Error, Message = msg };
        }

        /// <summary>
        /// 自定义状态返回结果
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ResponseResult<T> Result(ResultStatus status, T data, string? msg = null)
        {
            return new ResponseResult<T> { Status = status, Data = data, Message = msg };
        }
        /// <summary>
        /// 隐式将T转化为ResponseResult<T>
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ResponseResult<T>(T value)
        {
            return new ResponseResult<T> { Data = value };
        }
    }
}

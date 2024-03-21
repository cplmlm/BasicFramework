using System.ComponentModel;

namespace BasicCommonProject.Result
{
    public enum ResultStatus
    {
        [Description("请求成功")]
        Success = 1,
        [Description("请求失败")]
        Fail = 0,
        [Description("请求异常")]
        Error = -1
    }
}

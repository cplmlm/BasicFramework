using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace BasicCommonProject.Result
{
    public class ApiResult<T>
    {
        /// <summary>
        /// 响应代码
        /// </summary>
        [JsonProperty("code")]
        public ResultStatus Code { get; set; }

        /// <summary>
        /// 响应信息，调用失败时一定有响应信息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 响应数据，可能为空
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool success => this.Code == ResultStatus.Success;
    }
}

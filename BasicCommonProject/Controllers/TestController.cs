using Autho.JWT;
using BasicCommonProject.Result;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BasicCommonProject.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// 这个需要认证，角色必须是Admin，其他的不需要
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "SystemOrAdmin")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 这个也需要认证，只不过登录即可，不一定是Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 登录接口：随便输入字符，获取token，然后添加 Authoritarian
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetJWTToken(string name, string pass)
        {
            string jwtStr = string.Empty;
            bool suc = false;
            //这里就是用户登陆以后，通过数据库去调取数据，分配权限的操作
            //这里直接写死了


            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                return new JsonResult(new
                {
                    Status = false,
                    message = "用户名或密码不能为空"
                });
            }

            TokenModelJWT tokenModel = new TokenModelJWT();
            tokenModel.Uid = 1;
            tokenModel.Role = "Admin";

            jwtStr = JwtHelper.IssueJWT(tokenModel);
            suc = true;


            return Ok(new
            {
                success = suc,
                token = jwtStr
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Extensions.Authorizations.Policys;
using IServices;
using Model.ViewModels;
using Model;
using Common.Helper;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Extensions;
using Model.Models;
using System.Text;
using Org.BouncyCastle.Utilities.Encoders;
using Common.GlobalVar;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : BaseApiController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        readonly ISysUserInfoServices _sysUserInfoServices;
        readonly IUserRoleServices _userRoleServices;
        readonly IRoleServices _roleServices;
        private readonly PermissionRequirement _requirement;
        private readonly IRoleModulePermissionServices _roleModulePermissionServices;
        private readonly IRedisBasketRepository _redisBasketRepository;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="sysUserInfoServices"></param>
        /// <param name="userRoleServices"></param>
        /// <param name="roleServices"></param>
        /// <param name="requirement"></param>
        /// <param name="roleModulePermissionServices"></param>
        /// <param name="logger"></param>
        public WeatherForecastController(ISysUserInfoServices sysUserInfoServices, IUserRoleServices userRoleServices,
            IRoleServices roleServices, PermissionRequirement requirement, IRedisBasketRepository redisBasketRepository,
            IRoleModulePermissionServices roleModulePermissionServices, ILogger<WeatherForecastController> logger)
        {
            this._sysUserInfoServices = sysUserInfoServices;
            this._userRoleServices = userRoleServices;
            this._roleServices = roleServices;
            _requirement = requirement;
            _roleModulePermissionServices = roleModulePermissionServices;
            _logger = logger;
            _redisBasketRepository = redisBasketRepository;
        }
        /// <summary>
        /// 公钥
        /// </summary>
        public static string PublicKey = "044aa7882f7f7e949041dc234a8011cbd63234518fb4d3cc74ddcbd8bd7ad79ebca9eaa555474024708538ca926088bcc18fb513c52bc8d74710b24fb9e79a7802";

        /// <summary>
        /// 私钥
        /// </summary>
        public static string PrivateKey = "daf487999747ce3805e2bb03c470b64d75f7e4674980558f2fb33d476d156d73";
        private static readonly HexEncoder encoder = new HexEncoder();
        /// <summary>
        /// 公钥加密明文
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns>密文</returns>
        public static string Encrypt(string plainText)
        {
            return SM2Helper.Encrypt(PublicKey, plainText);
        }

        /// <summary>
        /// 私钥解密密文
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <returns>明文</returns>
        public static string Decrypt(string cipherText)
        {
            if (!cipherText.StartsWith("04")) cipherText = "04" + cipherText;//如果不是04开头加上04
            return SM2Helper.Decrypt(PrivateKey, cipherText);

        }
        public static byte[] Decode(string data)
        {

            MemoryStream memoryStream = new MemoryStream((data.Length + 1) / 2);
            encoder.DecodeString(data, memoryStream);
            return memoryStream.ToArray();
        }
        [HttpGet(Name = "GetWeatherForecast")]
        public void Get()
        {

            string plainText = "123456";
            Decode(PublicKey);
            string cipherText = Encrypt(plainText);
            string decryptText = Decrypt(cipherText);

            Sm2Util sm2Util = new Sm2Util(PublicKey, PrivateKey, Sm2Util.Mode.C1C2C3, false);
            //string data=Sm2Util.BytesToHexStr(sm2Util.Encrypt(Encoding.Default.GetBytes(plainText)));
            var output = Encoding.UTF8.GetString(sm2Util.Decrypt(Sm2Util.HexStrToBytes(cipherText)));

        }
        /// <summary>
        /// 获取博客列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBlogs")]
        public async Task<List<SysUserInfo>> GetBlogs()
        {
            List<SysUserInfo> blogArticleList = new List<SysUserInfo>();
            var a = await _redisBasketRepository.Get<object>("Redis.Blog");
            if (a != null)
            {
                blogArticleList = await _redisBasketRepository.Get<List<SysUserInfo>>("Redis.Blog");
            }
            else
            {
                blogArticleList = await _sysUserInfoServices.Query(d => d.LoginName == "test");
                await _redisBasketRepository.Set("Redis.Blog", blogArticleList, TimeSpan.FromHours(2));//缓存2小时
            }
            return blogArticleList;
        }
        /// <summary>
        /// 获取JWT的方法3：整个系统主要方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("JWTToken3.0")]
        public async Task<MessageModel<TokenInfoViewModel>> GetJwtToken3(string name = "", string pass = "")

        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
                return Failed<TokenInfoViewModel>("用户名或密码不能为空");

            // pass = MD5Helper.MD5Encrypt32(pass);

            var user = await _sysUserInfoServices.Query(d =>
                d.LoginName == name && d.LoginPWD == pass);
            if (user.Count > 0)
            {
                var userRoles = await _sysUserInfoServices.GetUserRoleNameStr(name, pass);
                //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(JwtRegisteredClaimNames.Jti, user.FirstOrDefault().Id.ToString()),
                 //   new Claim("TenantId", user.FirstOrDefault().TenantId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.DateToTimeStamp()),
                    new Claim(ClaimTypes.Expiration,DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString())
                };
                claims.AddRange(userRoles.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));


                // ids4和jwt切换
                // jwt
                if (!Permissions.IsUseIds4)
                {
                    var data = await _roleModulePermissionServices.RoleModuleMaps();
                    var list = (from item in data
                                where item.IsDeleted == false
                                orderby item.Id
                                select new PermissionItem
                                {
                                    Url = item.Module?.LinkUrl,
                                    Role = item.Role?.Name.ObjToString(),
                                }).ToList();

                    _requirement.Permissions = list;
                }

                var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
                return Success(token, "获取成功");
            }
            else
            {
                return Failed<TokenInfoViewModel>("认证失败");
            }
        }
        /// <summary>
        /// 测试 post 参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public object TestPostPara(string name)
        {
            return Ok(new { success = true, name = name });
        }
    }
}

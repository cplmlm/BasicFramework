using Repository.Base;
using IServices;
using Model.Models;
using Services.Base;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Common.Helper;

namespace Services
{
    [ServiceAttribute(ServiceLifetime.Transient)]
    /// <summary>
    /// sysUserInfoServices
    /// </summary>	
    public class SysUserInfoServices : BaseServices<SysUserInfo>, ISysUserInfoServices
    {
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        public SysUserInfoServices(IBaseRepository<UserRole> userRoleRepository, IBaseRepository<Role> roleRepository, IBaseRepository<SysUserInfo> BaseDal) : base(BaseDal)
        {
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public async Task<SysUserInfo> SaveUserInfo(string loginName, string loginPwd)
        {
            SysUserInfo sysUserInfo = new SysUserInfo(loginName, loginPwd);
            SysUserInfo model = new SysUserInfo();
            var userList = await base.Query(a => a.LoginName == sysUserInfo.LoginName && a.LoginPWD == sysUserInfo.LoginPWD);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(sysUserInfo);
                model = await base.QueryById(id);
            }

            return model;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public async Task<string> GetUserRoleNameStr(string loginName, string loginPwd)
        {
            string roleName = "";
            var user = (await base.Query(a => a.LoginName == loginName && a.LoginPWD == loginPwd)).FirstOrDefault();
            var roleList = await _roleRepository.Query(a => a.IsDeleted == false);
            if (user != null)
            {
                var userRoles = await _userRoleRepository.Query(ur => ur.UserId == user.Id);
                if (userRoles.Count > 0)
                {
                    var arr = userRoles.Select(ur => ur.RoleId.ObjToString()).ToList();
                    var roles = roleList.Where(d => arr.Contains(d.Id.ObjToString()));

                    roleName = string.Join(',', roles.Select(r => r.Name).ToArray());
                }
            }
            return roleName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public async Task<string> GetUserRoleNameStr1(string loginName, string loginPwd)
        {
       
                var userRoles = await _userRoleRepository.Query(ur => ur.UserId == 1);
           
            return "112";
        }
    }
}

//----------sysUserInfo结束----------

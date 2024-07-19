using Microsoft.Extensions.DependencyInjection;
using Common;
using IServices;
using Model.Models;
using Services.Base;
using System.Linq;
using System.Threading.Tasks;
using Repository.Base;

namespace Services
{
    [ServiceAttribute(ServiceLifetime.Transient)]
    /// <summary>
    /// RoleServices
    /// </summary>	
    public class RoleServices : BaseServices<Role>, IRoleServices
    {
        public RoleServices(IBaseRepository<Role> BaseDal) : base(BaseDal)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<Role> SaveRole(string roleName)
        {
            Role role = new Role(roleName);
            Role model = new Role();
            var userList = await base.Query(a => a.Name == role.Name && a.Enabled);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(role);
                model = await base.QueryById(id);
            }

            return model;

        }

        [Caching(AbsoluteExpiration = 30)]
        public async Task<string> GetRoleNameByRid(int rid)
        {
            return ((await base.QueryById(rid))?.Name);
        }
    }
}

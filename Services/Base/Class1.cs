using Model.Models;
using Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Base
{
    public class Class1<T> : Interface1<T>
    {
        private readonly IBaseRepository<SysUserInfo> baseRepository;

        public Class1(IBaseRepository<SysUserInfo> baseRepository)
        {
            this.baseRepository = baseRepository;
        }
        public async Task<List<SysUserInfo>> Method1(T t)
        {
          return await baseRepository.Query();
        }
    }
}

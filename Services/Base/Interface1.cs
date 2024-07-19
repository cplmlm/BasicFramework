using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Base
{
    public interface Interface1<T>
    {
        Task<List<SysUserInfo>> Method1(T t);
    }
}

using Model.ViewModels;

namespace IServices
{
    public interface ITest
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        void GetData();
        int Add(int a, int b);
       Task<BlogViewModels> GetBlog(int id);
    }
}

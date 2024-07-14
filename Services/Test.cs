using IServices;
using Model.Models;
using Model.ViewModels;
using AutoMapper;
using System.Data;
using Repository.Base;
namespace Services
{
    public class Test : ITest
    {
        IMapper mapper;
        private readonly IBaseRepository<Module> _userRoleRepository;
        public Test(IMapper mapper, IBaseRepository<Module> userRoleRepository)
        {
            this.mapper = mapper;
            _userRoleRepository = userRoleRepository;
        }
        public int Add(int a, int b)
        {
            return a + b;
        }

        public async Task<BlogViewModels> GetBlog(int id)
        {
            BlogArticle article = new BlogArticle();
            article.bID = id;
            article.btitle = "Test";
            article.bcontent = "Test";
            BlogViewModels blog = mapper.Map<BlogViewModels>(article);
            await Task.Delay(1000);
            return blog;
        }

        public void GetData()
        {
            int a = 10;
            int b = a;
        }
    }
}

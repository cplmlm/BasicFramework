using IServices;
using Model.Models;
using Model.ViewModels;
using AutoMapper;
namespace Services
{
    public class Test : ITest
    {
        IMapper mapper;
        public Test(IMapper mapper)
        {
            this.mapper = mapper;
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

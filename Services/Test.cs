using IServices;
namespace Services
{
    public class Test : ITest
    {
        public int Add(int a, int b)
        {
           return a + b;
        }

        public void GetData()
        {
            int a = 10;
            int b = a;
        }
    }
}

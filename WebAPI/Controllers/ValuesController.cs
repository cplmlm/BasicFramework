using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.GlobalVar;
using Services.Base;
using IServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ValuesController : ControllerBase
    {
        private readonly Interface1<string> _interface1;
        readonly ISysUserInfoServices _sysUserInfoServices;
        public ValuesController(Interface1<string> interface1, ISysUserInfoServices sysUserInfoServices)
        {
            this._interface1 = interface1;
            this._sysUserInfoServices = sysUserInfoServices;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
           var result = await _interface1.Method1(string.Empty);
            var result2 = _sysUserInfoServices.GetUserRoleNameStr1("test","123");
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

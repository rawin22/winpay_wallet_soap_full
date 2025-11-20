using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Tsg.UI.Main.APIControllers
{
    public class ApiTestController : ApiController
    {
        // GET: api/ApiTest
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ApiTest/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ApiTest
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ApiTest/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiTest/5
        public void Delete(int id)
        {
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountServer.Controllers
{
    public class TestReq
    {
        public string? Test_Req_String { get; set; }
        public int Test_Req_Int { get; set; }
    }

    public class TestRes
    {
        public string? Test_Res_String { get; set; }
        public int Test_Res_Int { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost]
        [Route("test")]
        public TestRes TestRequest([FromBody] TestReq req)
        {
            return new TestRes()
            {
                Test_Res_String = req.Test_Req_String,
                Test_Res_Int = req.Test_Req_Int + 1
            };
        }

        [HttpPost]
        [Route("simple")]
        public string SimpleRequest([FromBody] string req)
        {
            return "res";
        }
    }
}

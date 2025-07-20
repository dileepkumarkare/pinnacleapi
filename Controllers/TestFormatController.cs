using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Models;

namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestFormatController : ControllerBase
    {
        TestFormatModel model = new TestFormatModel();
        MasterModel master = new MasterModel();
    }
}

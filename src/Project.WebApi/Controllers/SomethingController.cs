using Microsoft.AspNetCore.Mvc;
using Project.Domain.SomethingContext.Models;
using Project.WebApi.Controllers.Models;

namespace Project.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SomethingController : ControllerBase
{
    [HttpPost]
    public ActionResult<SomethingRequest.Response> Try([FromBody] SomethingRequest.Body body)
    {
        var valueObject = SomethingValueObject.Create(body.String, body.Number, body.Boolean, body.DateTime).Value;
        return Ok(new SomethingRequest.Response(valueObject));
    }
}

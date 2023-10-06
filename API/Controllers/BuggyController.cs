using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        [HttpGet("not-found")]
        public ActionResult GetNotFound()
        {
            // return 404
            return NotFound();
        }

        [HttpGet("bad-request")]
        public ActionResult GetBadRequest()
        {
            // return 400
            // return BadRequest("This is a bad request");
            return BadRequest(new ProblemDetails { Title = "This a bad request" });
        }

        [HttpGet("unauthorised")]
        public ActionResult GetUnauthorised()
        {
            // return 401
            return Unauthorized();
        }

        [HttpGet("validation-error")]
        public ActionResult GetValidationError()
        {
            ModelState.AddModelError("Problem1", "This is the first error");
            ModelState.AddModelError("Problem2", "This is the second error");
            return ValidationProblem();
        }

        [HttpGet("server-error")]
        public ActionResult GetServerError()
        {
            // 500 : server error
            throw new Exception("This is a server error");
        }
    }
}
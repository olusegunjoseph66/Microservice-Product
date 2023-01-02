using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.DTOs.APIDataFormatters;

namespace Product.API.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
        protected new IActionResult Response(ApiResponse response)
        {
            if (response.StatusCode == "201")
                return Created("", response);

            if (response.StatusCode == "00")
                return Ok(response);

            if (response.StatusCode == "400")
                return BadRequest(response);

            if (response.StatusCode == "404")
                return NotFound(response);

            if (response.StatusCode == "409")
                return Conflict(response);

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}

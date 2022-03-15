using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teledrop.Models;

namespace Teledrop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileApiController : ControllerBase
    {
        private readonly TeledropDbContext _context;

        public ProfileApiController(TeledropDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileImageBase64(string account)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.Account == account);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile.ProfileImageBase64);
        }
    }
}

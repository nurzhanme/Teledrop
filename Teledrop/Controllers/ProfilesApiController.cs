using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teledrop.Models;

namespace Teledrop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesApiController : ControllerBase
    {
        private readonly TeledropDbContext _context;

        public ProfilesApiController(TeledropDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfileShortInfo(string account)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.Account == account);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Firstname = profile.Firstname,
                ProfileImageBase64 = profile.ProfileImageBase64
            });
        }
    }
}

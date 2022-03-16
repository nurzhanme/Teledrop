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
        [Route("name")]
        public async Task<IActionResult> GetName(string account)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.Account == account);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Firstname = profile.Firstname
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("image")]
        public async Task<IActionResult> GetImageBase64(string account)
        {
            var profile = await _context.ProfileImages.FirstOrDefaultAsync(x => x.Account == account);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Image = Convert.ToBase64String(profile.Image)
            });
        }
    }
}

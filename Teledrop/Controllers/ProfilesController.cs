#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Teledrop.Models;
using Teledrop.Services;

namespace Teledrop.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly TeledropDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly YoutubeService _youtubeService;
        public ProfilesController(TeledropDbContext context, IHttpClientFactory httpClientFactory, YoutubeService youtubeService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _youtubeService = youtubeService ?? throw new ArgumentNullException(nameof(youtubeService));
        }

        // GET: Profiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Profiles.ToListAsync());
        }

        // GET: Profiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        public async Task<IActionResult> AddYoutube(int? id)
        {
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            var res = await _youtubeService.Auth();
            if (!res.Item1) {
                return BadRequest();
            }

            var auth = new YoutubeAuth
            {
                Profile = profile,
                ProfileId = profile.Id,
                Token = res.Item2
            };

            var channelId = await _youtubeService.GetMyChannelId();
            profile.YoutubeChannelId = channelId;

            _context.Add(auth);
            _context.Update(profile);

            await _context.SaveChangesAsync();

            return View(profile);
        }

        // GET: Profiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Account,EvmAddress")] CreateProfileViewModel profileVM)
        {
            if (ModelState.IsValid)
            {
                var httpClient = _httpClientFactory.CreateClient();
                var imageByteArray = await httpClient.GetByteArrayAsync("https://thispersondoesnotexist.com/image");

                var profile = new Profile
                {
                    Firstname = RandomService.GetRandomName(),
                    Account = profileVM.Account,
                    EvmAddress = profileVM.EvmAddress
                };

                var profileImage = new ProfileImage
                {
                    Account = profileVM.Account,
                    Image = imageByteArray 
                };

                _context.Add(profile);
                _context.Add(profileImage);
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profileVM);
        }

        // GET: Profiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Firstname,ProfileImage,ProfileImageBase64,Email,Account,EvmAddress")] Profile profile)
        {
            if (id != profile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileExists(profile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        // GET: Profiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfileExists(int id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }
    }
}

#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teledrop.Models;
using Teledrop.Services;

namespace Teledrop.Controllers
{
    public class InstaAccountsController : Controller
    {
        private readonly TeledropDbContext _context;
        private readonly InstaService _insta;

        public InstaAccountsController(TeledropDbContext context, InstaService insta)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _insta = insta ?? throw new ArgumentNullException(nameof(insta));
        }

        // GET: InstaAccounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.InstaAccounts.ToListAsync());
        }

        // GET: InstaAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instaAccount = await _context.InstaAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instaAccount == null)
            {
                return NotFound();
            }

            return View(instaAccount);
        }

        // GET: InstaAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InstaAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,SessionData")] InstaAccount instaAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instaAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instaAccount);
        }

        // GET: InstaAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instaAccount = await _context.InstaAccounts.FindAsync(id);
            if (instaAccount == null)
            {
                return NotFound();
            }
            return View(instaAccount);
        }

        // POST: InstaAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,SessionData")] InstaAccount instaAccount)
        {
            if (id != instaAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instaAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstaAccountExists(instaAccount.Id))
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
            return View(instaAccount);
        }

        // GET: InstaAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instaAccount = await _context.InstaAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instaAccount == null)
            {
                return NotFound();
            }

            return View(instaAccount);
        }

        // POST: InstaAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instaAccount = await _context.InstaAccounts.FindAsync(id);
            _context.InstaAccounts.Remove(instaAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstaAccountExists(int id)
        {
            return _context.InstaAccounts.Any(e => e.Id == id);
        }
    }
}

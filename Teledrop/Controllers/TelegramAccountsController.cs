#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Teledrop.Models;
using Teledrop.Services;

namespace Teledrop.Controllers
{
    [Authorize]
    public class TelegramAccountsController : Controller
    {
        private readonly ILogger<TelegramAccountsController> _logger;
        private readonly TeledropDbContext _context;
        private readonly TelegramService _telegram;

        public TelegramAccountsController(ILogger<TelegramAccountsController> logger, TeledropDbContext context, TelegramService telegram)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _telegram = telegram ?? throw new ArgumentNullException(nameof(telegram));
        }

        // GET: TelegramAccounts
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.TelegramAccounts.ToListAsync());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
            
        }

        // GET: TelegramAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramAccount = await _context.TelegramAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telegramAccount == null)
            {
                return NotFound();
            }

            return View(telegramAccount);
        }

        // GET: TelegramAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TelegramAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Phonenumber")] TelegramAccount telegramAccount)
        {
            if (ModelState.IsValid)
            {
                var authResult = await _telegram.Auth(telegramAccount.Phonenumber);

                if (authResult == Enums.AuthorizationStateEnum.Unknown)
                {
                    return View(telegramAccount);
                }

                _context.Add(telegramAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Code), telegramAccount.Id);
            }
            return View(telegramAccount);
        }

        // GET: TelegramAccounts/Code/5
        public async Task<IActionResult> Code(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramAccount = await _context.TelegramAccounts.FindAsync(id);
            if (telegramAccount == null)
            {
                return NotFound();
            }
            return View(new PhoneCodeViewModel { Phonenumber = telegramAccount.Phonenumber });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Code(int id, [Bind("Phonenumber,Code")] PhoneCodeViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                var result = await _telegram.EnterCode(vm.Phonenumber, vm.Code);
                return result ? RedirectToAction(nameof(Index)) : View(vm);
            }
            return View(vm);
        }

        // GET: TelegramAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramAccount = await _context.TelegramAccounts.FindAsync(id);
            if (telegramAccount == null)
            {
                return NotFound();
            }
            return View(telegramAccount);
        }

        // POST: TelegramAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Phonenumber")] TelegramAccount telegramAccount)
        {
            if (id != telegramAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telegramAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelegramAccountExists(telegramAccount.Id))
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
            return View(telegramAccount);
        }

        // GET: TelegramAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramAccount = await _context.TelegramAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telegramAccount == null)
            {
                return NotFound();
            }

            return View(telegramAccount);
        }

        // POST: TelegramAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var telegramAccount = await _context.TelegramAccounts.FindAsync(id);
            _context.TelegramAccounts.Remove(telegramAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TelegramAccountExists(int id)
        {
            return _context.TelegramAccounts.Any(e => e.Id == id);
        }
    }
}

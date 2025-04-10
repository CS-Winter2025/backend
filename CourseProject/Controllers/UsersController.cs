using System.Security.Claims;
using CourseProject;
using CourseProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVCSampleApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly DatabaseContext _context;

        public UsersController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = new PasswordHasher().HashPassword(user.Password);
                user.Role = UserRole.NONE;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role.ToString());

                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);

                PasswordHasher hasher = new PasswordHasher();
                PasswordVerificationResult result = hasher.VerifyHashedPassword(user.Password, password);
                if (result == PasswordVerificationResult.Failed)
                {
                    ViewBag.Error = "Invalid login";
                    return View();
                }
                if (result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.Password = hasher.HashPassword(user.Password);
                    await _context.SaveChangesAsync();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role.ToString());

                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Invalid login";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        // GET: Users
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .Include(u => u.Resident) // Should be null if user part of the role
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Role")] UserView userView)
        {
            if (id != userView.Id)
            {
                return NotFound();
            }

            User user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                try
                {
                    user.Username = userView.Username;
                    user.Role = userView.Role;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

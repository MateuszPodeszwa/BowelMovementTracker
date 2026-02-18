using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BowelMovementTracker.Data;

namespace BowelMovementTracker.Controllers;

public class UsersController(BowelMovementTrackerContext context) : Controller
{
    // GET: Users
    public async Task<IActionResult> Index()
    {
        return View(await context.User.ToListAsync());
    }

    // GET: Users/Details/5
    [HttpGet]
    public async Task<IActionResult> Details( [FromRoute] Guid? id )
    {
        if (id == null) return NotFound();

        var user = await context.User.FirstOrDefaultAsync(m => m.UserIdentifier == id);

        if (user == null) return NotFound();

        return View(user);
    }

    // GET: Users/Create
    [HttpGet] public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserEmailAddress"), FromForm] User boundUser)
    {
        // 1. Remove required fields the form doesn't send
        ModelState.Remove("UserPasswordHash");
        ModelState.Remove("Diary");

        // 2. Hard-check the email so the DB doesn't crash if the HTML form is wrong
        if (string.IsNullOrWhiteSpace(boundUser.UserEmailAddress))
        {
            ModelState.AddModelError("UserEmailAddress", "Email is required.");
        }

        if (!ModelState.IsValid) return View(boundUser);

        // 3. Build the object tree
        var newUser = new User
        {
            UserEmailAddress = boundUser.UserEmailAddress, 
            UserPasswordHash = Guid.NewGuid().GetHashCode().ToString(),
        
            Diary = new Diary
            {
                Logs = [],
                User = null!, // Satisfies C# 'required', prevents EF Core duplication crash
                DiaryUserIdentifier = Guid.Empty // Satisfies C# 'required', EF Core overwrites this automatically
            }
        };

        context.Add(newUser);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Users/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit( [FromRoute] Guid? id)
    {
        if (id == null) return NotFound();

        var user = await context.User.FindAsync(id);

        if (user == null) return NotFound();
            
        return View(user);
    }

    // POST: Users/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("UserIdentifier,UserEmailAddress")] User user)
    {
        if (id != user.UserIdentifier)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(user);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserIdentifier))
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
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await context.User
            .FirstOrDefaultAsync(m => m.UserIdentifier == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var user = await context.User.FindAsync(id);
        if (user != null)
        {
            context.User.Remove(user);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserExists(Guid id)
    {
        return context.User.Any(e => e.UserIdentifier == id);
    }
}
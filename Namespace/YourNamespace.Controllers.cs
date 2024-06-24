
using System.Diagnostics;

namespace YourNamespace.Controllers
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class InfoController(YourDbContext context) : Controller
    {
        private readonly YourDbContext context = context;

        // Інші методи

        [HttpPost]
        public async Task<IActionResult> Rate(int id, double rating)
        {
            var info = await context.Infos.FindAsync(id);
            if (info == null)
            {
                return NotFound();
            }

            info.Rating = rating; // Оновлення рейтингу
            context.Update(info);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private IActionResult RedirectToAction(string v)
        {
            throw new NotImplementedException();
        }

        private IActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }

    internal class HttpPostAttribute : Attribute
    {
    }
}

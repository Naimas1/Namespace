
namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        private readonly YourDbContext _context;

        public HomeController(YourDbContext context)
        {
            _context = context;
        }

        public YourDbContext Get_context()
        {
            return _context;
        }
    }

    public interface IActionResult
    {
    }

    internal class YourDbContext
    {
        public object Infos { get; internal set; }

        internal async Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        internal void Update(object info)
        {
            throw new NotImplementedException();
        }
    }

    public class Controller
    {

        public async Task<IActionResult> Index(string searchString, YourDbContext _context)
        {
            var infos = from i in _context.Infos
                        select i;

            if (!string.IsNullOrEmpty(searchString))
            {
                infos = infos.Where(i => i.FirstName.Contains(searchString) ||
                                         i.LastName.Contains(searchString) ||
                                         i.Description.Contains(searchString) ||
                                         i.ProfessionName.Contains(searchString));
            }

            return View(await infos.ToListAsync());
        }
    }
}

namespace ReservationSystem.Controllers;

public class ReservationsController
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var reservations = _context.Reservations.ToList();
            return View(reservations);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Reservation reservation)
        {
            if (!ModelState.IsValid)
                return View(reservation);

            _context.Reservations.Add(reservation);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
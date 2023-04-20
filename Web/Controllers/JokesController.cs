using Marten;
using Marten.Linq;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class JokesController : Controller
    {
        private readonly IDocumentSession _session;

        public JokesController(IDocumentSession session)
        {
            _session = session;
        }

        // GET: Joke
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var jokes = await ((IMartenQueryable)_session.Query<Joke>()).ToListAsync<Joke>(cancellationToken);

            return View(jokes);
        }

        // GET: Joke/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return View(await _session.LoadAsync<Joke>(id));
        }

        // GET: Joke/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Joke/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Question,Answer")] Joke joke)
        {
            if (ModelState.IsValid)
            {
                _session.Store(joke);
                await _session.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(joke);
        }

        // GET: Joke/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            var joke = await _session.LoadAsync<Joke>(id);

            return joke == null ? NotFound() : View(joke);
        }

        // POST: Joke/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Question,Answer")] Joke joke)
        {
            if (id != joke.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(joke);
            
            _session.Store(joke);
            await _session.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Joke/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var joke = await _session.LoadAsync<Joke>(id);
            return joke == null ? NotFound() : View(joke);
        }

        // POST: Joke/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _session.Delete<Joke>(id);
            await _session.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ShowSearchForm()
        {
            return View();
        }

        public IActionResult ShowSearchResults(string searchPhrase)
        {
            
            var results = _session.Query<Joke>().Where(x => x.Question.Contains(searchPhrase)).ToList();
            
            return View("Index", results);
        }
    }
}

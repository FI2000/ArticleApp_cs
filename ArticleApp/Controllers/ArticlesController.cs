using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArticleApp.Data;
using ArticleApp.Models;
using System.Diagnostics;

namespace ArticleApp.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        public ArticlesController(AppDbContext context, ILogger logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Articles
        public async Task<IActionResult> Index(string orderBy = "views")
        {
            _logger.LogInformation("ArticleController: /GET Index Called");
            if (_context.Articles != null) {
                Problem("Entity set 'AppDbContext.Articles'  is null.");
            }

            IQueryable<Article> query = _context.Articles!;

            if (orderBy == "views")
            {
                query = query.OrderByDescending(a => a.Views);
            }
            else if (orderBy == "date")
            {
                query = query.OrderByDescending(a => a.PublishedOn);
            }

            ViewBag.OrderBy = orderBy;
            var articles = await query.ToListAsync();

            return View(articles);
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            article.Views++;
            _context.Update(article);
            await _context.SaveChangesAsync();

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            _logger.LogInformation("/GET Create Articles");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Author,Title,Content,ArticleTagsAsString,Category")] Article article)
        {

            string requestToken = HttpContext.Request.Headers["RequestVerificationToken"];
            string formToken = HttpContext.Request.Form["__RequestVerificationToken"];

            Debug.WriteLine($"Request Token: {requestToken}");
            Debug.WriteLine($"Form Token: {formToken}");

            Debug.WriteLine($"{article.Title} - {article.Author} - {article.ArticleTagsAsString} - {article.Content} - {article.Category} ");
            _logger.LogInformation("/POST Create Articles");
            _logger.LogInformation($"Received : {article}");
            if (ModelState.IsValid)
            {
                article.PublishedOn = DateTime.Now;
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            Debug.WriteLine("Failed");

            _logger.LogInformation("Model State is Invalid. -info");
            _logger.LogWarning("Model State is Invalid.");
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogWarning($"Error: {error.ErrorMessage}");
                }
             }
             return View(article);

        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Author,Title,Content,PublishedOn,ArticleTagsAsString,Category,Views")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
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
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Articles == null)
            {
                return Problem("Entity set 'AppDbContext.Articles'  is null.");
            }
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
          return (_context.Articles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

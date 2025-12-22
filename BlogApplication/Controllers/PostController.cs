using BlogApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SyncoSyntax.Models.ViewModels;
using System.CodeDom;

namespace BlogApplication.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Create()
        {
            var postViewModel = new PostViewModelcs();

            postViewModel.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View(postViewModel);
        }

    }
}

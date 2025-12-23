using BlogApplication.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SyncoSyntax.Models.ViewModels;

namespace BlogApplication.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // ✅ FIX
        private readonly string[] _allowedExtension = { ".jpg", ".jpeg", ".png" };

        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment) // ✅ FIX
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index(int? categoryId)
        {
            var postQuery = _context.Posts
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            var posts = postQuery.ToList();

            ViewBag.Categories = _context.Categories.ToList();

            return View(posts); // ✅ PASS MODEL
        }


        [HttpGet]
        public IActionResult Create()
        {
            var postViewModel = new PostViewModelcs
            {
                Categories = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList()
            };

            return View(postViewModel);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(PostViewModelcs postViewModel)
        {
            // Refill Categories for View (important on validation errors)
            postViewModel.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            if (!ModelState.IsValid)
                return View(postViewModel);

            if (postViewModel.FeatureImage == null)
            {
                ModelState.AddModelError("", "Feature image is required");
                return View(postViewModel);
            }

            var inputFileExtension = Path
                .GetExtension(postViewModel.FeatureImage.FileName)
                .ToLower();

            if (!_allowedExtension.Contains(inputFileExtension))
            {
                ModelState.AddModelError(
                    "",
                    "Invalid Image Format. Allowed: .jpg, .jpeg, .png"
                );
                return View(postViewModel);
            }

            postViewModel.Post.FeatureImagePath =
                await UploadFiletoFolder(postViewModel.FeatureImage);

            await _context.Posts.AddAsync(postViewModel.Post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private async Task<string> UploadFiletoFolder(IFormFile file)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var wwwRootPath = _webHostEnvironment.WebRootPath; // ✅ WORKS
            var imageFolderPath = Path.Combine(wwwRootPath, "images");

            if (!Directory.Exists(imageFolderPath))
                Directory.CreateDirectory(imageFolderPath);

            var filePath = Path.Combine(imageFolderPath, fileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return "/images/" + fileName;
        }
    }
}

using BlogApplication.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult Index()
        {
            return View();
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
        public async Task<IActionResult> Create(PostViewModelcs postViewModel)
        {
            if (!ModelState.IsValid)
                return View(postViewModel);

            var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();

            if (!_allowedExtension.Contains(inputFileExtension))
            {
                ModelState.AddModelError("", "Invalid Image Format. Allowed: .jpg, .jpeg, .png");
                return View(postViewModel);
            }

            postViewModel.Post.FeatureImagePath =
                await UploadFiletoFolder(postViewModel.FeatureImage);

            await _context.Posts.AddAsync(postViewModel.Post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
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

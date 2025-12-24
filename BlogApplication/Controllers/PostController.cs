using BlogApplication.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SyncoSyntax.Models;
using SyncoSyntax.Models.ViewModels;

namespace BlogApplication.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtension = { ".jpg", ".jpeg", ".png" };

        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ================= INDEX =================
        [HttpGet]
        public IActionResult Index(int? categoryId)
        {
            var posts = _context.Posts
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                posts = posts.Where(p => p.CategoryId == categoryId.Value);

            ViewBag.Categories = _context.Categories.ToList();
            return View(posts.ToList());
        }

        // ================= DETAILS =================
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var post = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new PostViewModelcs
            {
                Categories = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModelcs vm)
        {
            vm.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            if (!ModelState.IsValid)
                return View(vm);

            if (vm.FeatureImage == null)
            {
                ModelState.AddModelError("", "Feature image is required");
                return View(vm);
            }

            var ext = Path.GetExtension(vm.FeatureImage.FileName).ToLower();
            if (!_allowedExtension.Contains(ext))
            {
                ModelState.AddModelError("", "Only jpg, jpeg, png allowed");
                return View(vm);
            }

            vm.Post.FeatureImagePath =
                await UploadFiletoFolder(vm.FeatureImage);

            _context.Posts.Add(vm.Post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= ADD COMMENT =================
        [HttpPost]
        public IActionResult AddComment([FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // ✅ CORRECT: DateTime, not string
            comment.CommentDate = DateTime.Now;

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Json(new
            {
                UserName = comment.UserName,
                Content = comment.Content,
                CommentDate = comment.CommentDate.ToString("dd MMM yyyy")
            });
        }

        // ================= EDIT =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();

            var vm = new EditViewModel
            {
                Post = post,
                Categories = _context.Categories.Select(c =>
                    new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList()
            };

            return View(vm);
        }

        // ================= FILE UPLOAD =================
        private async Task<string> UploadFiletoFolder(IFormFile file)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filePath = Path.Combine(path, fileName);

            await using var fs = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fs);

            return "/images/" + fileName;
        }
    }
}

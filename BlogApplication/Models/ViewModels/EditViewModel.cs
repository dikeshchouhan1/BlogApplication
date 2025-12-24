using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SyncoSyntax.Models;

namespace SyncoSyntax.Models.ViewModels
{
    public class EditViewModel
    {
        public Post Post { get; set; } = new Post(); // ✅ prevent null

        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }
            = Enumerable.Empty<SelectListItem>();

        [ValidateNever]
        public IFormFile? FeatureImage { get; set; } // ✅ optional
    }
}

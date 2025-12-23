using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SyncoSyntax.Models.ViewModels
{
    public class PostViewModelcs
    {
        public Post Post { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }

        public IFormFile FeatureImage { get; set; }


    }
}

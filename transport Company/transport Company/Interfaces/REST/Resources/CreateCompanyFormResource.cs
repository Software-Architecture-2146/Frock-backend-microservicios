using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Frock_backend.transport_Company.Interfaces.REST.Resources
{
    public class CreateCompanyFormResource
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public IFormFile? LogoFile { get; set; }
    }
}
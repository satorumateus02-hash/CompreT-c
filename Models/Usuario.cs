using Microsoft.AspNetCore.Identity;

namespace HelpDeskMvc.Models
{
    public class Usuario : IdentityUser
    {
        public string? CodigoAcesso { get; set; }
        public string? PermissoesNav { get; set; }
    }
}
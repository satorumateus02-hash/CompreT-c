namespace HelpDeskMvc.Models
{
    // Versão simplificada para iniciantes: propriedades simples e anuláveis
    public class RegisterViewModel
    {
        public string? Usuario { get; set; }
        public string? Senha { get; set; }
        public string? ConfirmarSenha { get; set; }
        public string? Email { get; set; }
    }
}

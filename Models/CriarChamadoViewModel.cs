using System.ComponentModel.DataAnnotations;

namespace HelpDeskMvc.Models
{
public class CriarChamadoViewModel
{
    [Required(ErrorMessage = "Informe uma descrição.")]
    [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o nome do cliente.")]
    [StringLength(200, ErrorMessage = "O nome do cliente deve ter no máximo 200 caracteres.")]
    public string ClienteNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe um telefone.")]
    [StringLength(50, ErrorMessage = "Telefone inválido")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o endereço.")]
    [StringLength(500, ErrorMessage = "Endereço muito longo")]
    public string Endereco { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Complemento muito longo")]
    public string? Complemento { get; set; }
}


}
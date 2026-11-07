using System.ComponentModel.DataAnnotations;

namespace HelpDeskMvc.Models;

public class AbrirChamadoViewModel
{
    [Required(ErrorMessage = "Informe um título.")]
    [StringLength(150, ErrorMessage = "O título deve ter no máximo 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe uma descrição.")]
    [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o cliente.")]
    [StringLength(200, ErrorMessage = "O nome do cliente deve ter no máximo 200 caracteres.")]
    public string ClienteNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o telefone.")]
    [StringLength(50, ErrorMessage = "O telefone deve ter no máximo 50 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o status.")]
    [RegularExpression("^(Aberto|Em atendimento|Resolvido|Cancelado|Fechado)$", ErrorMessage = "Selecione um status válido.")]
    public string Status { get; set; } = "Aberto";

    public DateTime DataAbertura { get; set; } = DateTime.Now;
}

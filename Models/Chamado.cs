namespace HelpDeskMvc.Models
{
public class Chamado
{
public int Id { get; set; }

    public required string Titulo { get; set; }

    public required string Descricao { get; set; }

    public string Status { get; set; } = "Aberto";

    public DateTime DataAbertura { get; set; } = DateTime.Now;

    public DateTime? DataFechamento { get; set; }

    // Dados do cliente para atendimento domiciliar
    public string? ClienteNome { get; set; }

    public string? Telefone { get; set; }

    public string? Endereco { get; set; }

    public string? Complemento { get; set; }
}


}
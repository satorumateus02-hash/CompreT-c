namespace HelpDeskMvc.Models
{
    public class ListarChamadosViewModel
    {
        public string? Busca { get; set; }

        public string? Status { get; set; }

        public List<Chamado> Chamados { get; set; } = new();
    }
}

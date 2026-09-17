using System.ComponentModel.DataAnnotations;

namespace HelpDeskMvc.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public required string Nome { get; set; }

        [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
        public string? Descricao { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Preço inválido")]
        [DataType(DataType.Currency)]
        public decimal Preco { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade inválida")]
        public int Quantidade { get; set; }

        [StringLength(200, ErrorMessage = "O nome do fornecedor deve ter no máximo 200 caracteres")]
        public string? Fornecedor { get; set; }

        [StringLength(100, ErrorMessage = "A categoria deve ter no máximo 100 caracteres")]
        public string? Categoria { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}

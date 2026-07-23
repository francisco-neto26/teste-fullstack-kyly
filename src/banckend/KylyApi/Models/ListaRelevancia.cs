using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KylyApi.Models;

//tabela listas_relevancia
[Table("listas_relevancia")]
public class ListaRelevancia
{
    // ID incremental
    [Key]//PK da tabela
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    // ID do produto para validar pela FK
    [Required]
    [Column("produto_id")]
    public string ProdutoId { get; set; } = string.Empty;
    
    [Required]
    [Column("codigo_produto")]
    public string CodigoProduto { get; set; } = string.Empty;

    // Ordem de prioridade ex: 1, 2, 3
    [Required]
    [Column("prioridade")]
    public int Prioridade { get; set; }

    [Column("data_criacao")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    // Serve para mapear o relacionamento entre os objetos no C#, 
    // permitindo acessar os dados do Produto diretamente sem precisar fazer JOINs manuais no código.
    public virtual Produto? Produto { get; set; }
}
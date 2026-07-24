using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KylyApi.Models;

//Tabela produtos
[Table("produtos")]
public class Produto
{
    //Definido que não gera id, visto que ira vir do csv, o id ser a chave da tabela produtos
    [Key]//PK da tabela
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [Column("codigo_produto")]
    public string CodigoProduto { get; set; } = string.Empty;

    [Required]
    [Column("desc_produto")]
    public string DescProduto { get; set; } = string.Empty;

    [Required]
    [Column("codigo_cor")]
    public string CodigoCor { get; set; } = string.Empty; 

    [Required]
    [Column("desc_cor")]
    public string DescCor { get; set; } = string.Empty;

    [Required]
    [Column("codigo_tamanho")]
    public string CodigoTamanho { get; set; } = string.Empty;

    [Required]
    [Column("desc_tamanho")]
    public string DescTamanho { get; set; } = string.Empty;
}
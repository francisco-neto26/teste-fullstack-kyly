namespace KylyApi.DTOs;

// DTO para resposta de produto, usado para enviar dados do produto na API.
public class ProdutoResponse
{
    public string Id { get; set; } = string.Empty;
    public string CodigoProduto { get; set; } = string.Empty;
    public string DescProduto { get; set; } = string.Empty;
    public string CodigoCor { get; set; } = string.Empty;
    public string DescCor { get; set; } = string.Empty;
    public string CodigoTamanho { get; set; } = string.Empty;
    public string DescTamanho { get; set; } = string.Empty;
}
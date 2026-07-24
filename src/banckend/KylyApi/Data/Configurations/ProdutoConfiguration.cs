using KylyApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KylyApi.Data.Configurations;

//regras de como a tabela será criada e estruturada no banco de dados.
public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasMaxLength(100);

        builder.Property(p => p.CodigoProduto)
            .HasColumnName("codigo_produto")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.DescProduto)
            .HasColumnName("desc_produto")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.CodigoCor)
            .HasColumnName("codigo_cor")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.DescCor)
            .HasColumnName("desc_cor")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.CodigoTamanho)
            .HasColumnName("codigo_tamanho")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.DescTamanho)
            .HasColumnName("desc_tamanho")
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(p => p.CodigoProduto)
            .HasDatabaseName("idx_produtos_codigo");
    }
}
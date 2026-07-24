using KylyApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KylyApi.Data.Configurations;

//regras de como a tabela será criada e estruturada no banco de dados.
public class ListaRelevanciaConfiguration : IEntityTypeConfiguration<ListaRelevancia>
{
    public void Configure(EntityTypeBuilder<ListaRelevancia> builder)
    {
        builder.ToTable("listas_relevancia");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(l => l.ProdutoId)
            .HasColumnName("produto_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.CodigoProduto)
            .HasColumnName("codigo_produto")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(l => l.Prioridade)
            .HasColumnName("prioridade")
            .IsRequired();

        builder.Property(l => l.DataCriacao)
            .HasColumnName("data_criacao")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relacionamento FK -> ProdutoId liga em Produto.Id
        builder.HasOne(l => l.Produto)
            .WithMany()
            .HasForeignKey(l => l.ProdutoId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_listas_relevancia_produtos");

        builder.HasIndex(l => l.Prioridade)
            .HasDatabaseName("idx_listas_relevancia_prioridade");

        builder.HasIndex(l => l.CodigoProduto)
            .HasDatabaseName("idx_listas_relevancia_codigo");
    }
}
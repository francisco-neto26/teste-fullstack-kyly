using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Xunit;

namespace KylyApi.Tests.Integration;

public class ProdutosEndpointTests : IAsyncLifetime
{
    // 1. Cria um container temporário do PostgreSQL para o teste de integração
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;

    public async Task InitializeAsync()
    {
        // Carrega as variáveis de JWT do arquivo .env
        CarregarVariaveisEnv();

        // Inicializa o banco de dados de teste
        await _postgres.StartAsync();

        // Inicializa a API apontando a conexão do banco para o container temporário
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:DefaultConnection", _postgres.GetConnectionString());
        });
    }

    public async Task DisposeAsync()
    {
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }
        if (_postgres != null)
        {
            await _postgres.DisposeAsync();
        }
    }

    [Fact]
    public async Task Get_Produtos_SemToken_RetornaUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/produtos?termo=camiseta");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static void CarregarVariaveisEnv()
    {
        var diretorioAtual = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(diretorioAtual))
        {
            var caminhoEnv = Path.Combine(diretorioAtual, ".env");
            if (File.Exists(caminhoEnv))
            {
                foreach (var linha in File.ReadLines(caminhoEnv))
                {
                    if (string.IsNullOrWhiteSpace(linha) || linha.StartsWith("#"))
                        continue;

                    var partes = linha.Split('=', 2);
                    if (partes.Length == 2)
                    {
                        var chave = partes[0].Trim();
                        var valor = partes[1].Trim();

                        Environment.SetEnvironmentVariable(chave, valor);

                        if (chave == "JWT_SECRET_KEY") Environment.SetEnvironmentVariable("JwtSettings__SecretKey", valor);
                        if (chave == "JWT_ISSUER") Environment.SetEnvironmentVariable("JwtSettings__Issuer", valor);
                        if (chave == "JWT_AUDIENCE") Environment.SetEnvironmentVariable("JwtSettings__Audience", valor);
                    }
                }
                break;
            }

            diretorioAtual = Directory.GetParent(diretorioAtual)?.FullName;
        }
    }
}

using KylyApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KylyApi.Data;

public static class DataSeeder
{
    // Método principal para realizar o seed de dados no banco de dados
    public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        //escopo temporário para gerenciar o tempo de vida das dependências.
        using var scope = serviceProvider.CreateScope();

        //instância do contexto para manipular o banco de dados PostgreSQL.
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //serviço do Identity para gerenciar usuários (criar, buscar, validar senhas)
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        //serviço de logs para registrar mensagens ou erros no console/Docker.
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        //USUÁRIO PADRÃO (Para permitir o primeiro acesso)
        await SeedUsuarioPadraoAsync(userManager, logger, configuration);

        if (string.IsNullOrWhiteSpace(configuration["DIRETORIO_ARQUIVO"]))
        {
            logger.LogWarning("Variável de ambiente DIRETORIO_ARQUIVO não está definida. Importação de produtos não será realizada.");
            return;
        }

        //PRODUTOS (Lendo o sample_db.csv)
        await SeedProdutosAsync(context, logger, configuration);

        //LISTAS DE RELEVÂNCIA (Lendo lista_relevancia_1.txt e lista_relevancia_2.txt)
        await SeedListasRelevanciaAsync(context, logger, configuration);
    }

    private static async Task SeedUsuarioPadraoAsync(UserManager<ApplicationUser> userManager, ILogger logger, IConfiguration configuration)
    {
        var adminEmail = configuration["ADMIN_EMAIL"];
        var adminUsername = configuration["ADMIN_USER"];
        var adminPassword = configuration["ADMIN_PASSWORD"];

        //testa as variaveis de ambiente para garantir que o usuário padrão possa ser criado corretamente
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("Variáveis de ambiente ADMIN_EMAIL, ADMIN_USER ou ADMIN_PASSWORD não estão definidas. Usuário padrão não será criado.");
            return;
        }
        //verifica se o usuário padrão já existe no banco de dados, evitando duplicação
        var userExists = await userManager.FindByNameAsync(adminUsername);
        if (userExists == null)
        {
            logger.LogInformation("Criando usuário padrão 'admin'...");

            var adminUser = new ApplicationUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                NomeCompleto = "Administrador do Sistema",
                EmailConfirmed = true,
                DataCriacao = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                logger.LogInformation("Usuário padrão 'admin' criado com sucesso!");
            }
            else
            {
                logger.LogError("Erro ao criar usuário padrão: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task SeedProdutosAsync(AppDbContext context, ILogger logger, IConfiguration configuration)
    {
        var caminhoCsv = ObterCaminhoArquivo(configuration["DIRETORIO_ARQUIVO"], "sample_db.csv");

        if (!File.Exists(caminhoCsv))
        {
            logger.LogWarning("Arquivo de produtos CSV não encontrado em: {Caminho}", caminhoCsv);
            return;
        }

        //Lê as linhas do arquivo
        var linhas = await File.ReadAllLinesAsync(caminhoCsv);
        int totalProdutosCsv = linhas.Length - 1; // Desconta a linha do cabeçalho

        if (totalProdutosCsv <= 0)
        {
            logger.LogWarning("Arquivo CSV de produtos está vazio ou não contém dados válidos.");
            return;
        }

        //Conta rapidamente no banco os registros existentes para decidir se precisa importar ou não
        var totalProdutosBanco = await context.Produtos.CountAsync();

        // Se o número de produtos no banco for igual ou maior que o do CSV, não importa, criado somente para evitar demora ao subrir.
        // Não deveria ser assim, ja que o correto é validar se os produtos do CSV já existem no banco, usado assim pela performance.
        if (totalProdutosBanco >= totalProdutosCsv)
        {
            logger.LogInformation("Produtos já estão totalmente carregados no banco ({Qtd} itens). Pulando importação.", totalProdutosBanco);
            return;
        }

        logger.LogInformation("Iniciando importação de novos produtos do CSV...");

        var idsExistentes = (await context.Produtos
            .Select(p => p.Id)
            .ToListAsync())
            .ToHashSet();

        var produtos = new List<Produto>();

        for (int i = 1; i < linhas.Length; i++)
        {
            var linha = linhas[i].Trim();
            if (string.IsNullOrWhiteSpace(linha)) continue;

            var colunas = linha.Split(';');

            if (colunas.Length >= 7)
            {
                var id = colunas[0].Trim();

                // Garante que apenas produtos com IDs únicos sejam adicionados à lista, evitando duplicados.
                if (!string.IsNullOrEmpty(id) && idsExistentes.Add(id))
                {
                    produtos.Add(new Produto
                    {
                        Id = id,
                        CodigoProduto = colunas[1].Trim(),
                        DescProduto = colunas[2].Trim(),
                        CodigoCor = colunas[3].Trim(),
                        DescCor = colunas[4].Trim(),
                        CodigoTamanho = colunas[5].Trim(),
                        DescTamanho = colunas[6].Trim()
                    });
                }
            }
        }

        if (produtos.Any())
        {
            await context.Produtos.AddRangeAsync(produtos);
            await context.SaveChangesAsync();
            logger.LogInformation("{Qtd} novos produtos importados com sucesso!", produtos.Count);
        }
    }


    private static async Task SeedListasRelevanciaAsync(AppDbContext context, ILogger logger, IConfiguration configuration)
    {

        var caminhotxt = configuration["DIRETORIO_ARQUIVO"];

        var listaRelevancia = new List<ListaRelevancia>();

        // Importa Lista 1 (Prioridade 1)
        await ProcessarArquivoListaAsync(context, caminhotxt, "lista_relevancia_1.txt", 1, listaRelevancia, logger);

        // Importa Lista 2 (Prioridade 2)
        await ProcessarArquivoListaAsync(context, caminhotxt, "lista_relevancia_2.txt", 2, listaRelevancia, logger);

        if (listaRelevancia.Any())
        {
            await context.ListasRelevancia.AddRangeAsync(listaRelevancia);
            await context.SaveChangesAsync();
            logger.LogInformation("{Qtd} itens de relevância importados com sucesso!", listaRelevancia.Count);
        }
    }

    private static async Task ProcessarArquivoListaAsync(
       AppDbContext context,
       string caminhotxt,
       string nomeArquivo,
       int prioridade,
       List<ListaRelevancia> listas,
       ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(caminhotxt))
        {
            logger.LogWarning("O diretório dos arquivos de relevância não foi informado.");
            return;
        }

        var caminho = ObterCaminhoArquivo(caminhotxt, nomeArquivo);

        if (!File.Exists(caminho))
        {
            logger.LogWarning(
                "Arquivo de relevância {Nome} não encontrado em: {Caminho}",
                nomeArquivo,
                caminho);

            return;
        }

        var codigos = (await File.ReadAllLinesAsync(caminho))
            .Select(linha => linha.Trim())
            .Where(codigo => !string.IsNullOrWhiteSpace(codigo))
            .ToHashSet();

        var totalBanco = await context.ListasRelevancia
            .CountAsync(lista => lista.Prioridade == prioridade);

        // Segue a mesma regra do seed de produtos: se já possui a quantidade, não importa.
        if (totalBanco >= codigos.Count)
        {
            logger.LogInformation(
                "Lista {Nome} já está carregada no banco ({Qtd} itens).",
                nomeArquivo,
                totalBanco);

            return;
        }

        var produtos = await context.Produtos
            .Where(produto => codigos.Contains(produto.CodigoProduto))
            .Select(produto => new
            {
                produto.Id,
                produto.CodigoProduto
            })
            .ToListAsync();

        // Um código pode ter variações de cor/tamanho; seleciona um ID para satisfazer a FK.
        var produtosPorCodigo = produtos
            .GroupBy(produto => produto.CodigoProduto)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.OrderBy(produto => produto.Id).First());

        var codigosExistentes = (await context.ListasRelevancia
            .Where(lista => lista.Prioridade == prioridade)
            .Select(lista => lista.CodigoProduto)
            .ToListAsync())
            .ToHashSet();

        foreach (var codigo in codigos)
        {
            if (codigosExistentes.Contains(codigo))
            {
                continue;
            }

            if (!produtosPorCodigo.TryGetValue(codigo, out var produto))
            {
                logger.LogWarning(
                    "Produto com código {Codigo} não encontrado na lista {Nome}.",
                    codigo,
                    nomeArquivo);

                continue;
            }

            listas.Add(new ListaRelevancia
            {
                ProdutoId = produto.Id,
                CodigoProduto = codigo,
                Prioridade = prioridade
            });
        }
    }

    // Procura o arquivo tanto no diretório de execução local quanto na raiz do repositório/Docker
    private static string ObterCaminhoArquivo(string diretorio, string nomeArquivo)
    {
        if (string.IsNullOrWhiteSpace(diretorio))
        {
            throw new ArgumentException(
                "O diretório dos arquivos de importação não foi informado.",
                nameof(diretorio));
        }

        return Path.Combine(diretorio, nomeArquivo);
    }
}

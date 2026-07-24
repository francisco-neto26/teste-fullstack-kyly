using KylyApi.Data;
using System.Text;
using KylyApi.Models;
using KylyApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Habilita o uso da pasta e arquivos de Controllers, sem isso não é possível mapear as rotas e endpoints da API
builder.Services.AddControllers();

// Mapeia todas as rotas e tipos de dados criados, facilita a documentação e testes da API via Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Kyly API - Teste Fullstack";
        document.Info.Version = "v1";
        
        return Task.CompletedTask;
    });
});

// Configura o DbContext para usar PostgreSQL, utilizando a string de conexão definida no appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// configura o Identity para usar a classe ApplicationUser e IdentityRole, com persistência no banco via AppDbContext
// O Identity é o sistema de autenticação e autorização do ASP.NET Core, que gerencia usuários, senhas, roles e claims.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configurações de regras para senhas dos usuários
    options.Password.RequireDigit = true;            // Exige número
    options.Password.RequireLowercase = true;        // Exige letra minúscula
    options.Password.RequireUppercase = true;        // Exige letra maiúscula
    options.Password.RequireNonAlphanumeric = true; // Exige caractere especial (!@#$)
    options.Password.RequiredLength = 6;            // Tamanho mínimo de 6 caracteres
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


// configura o JWT Bearer Authentication, que é o mecanismo de autenticação baseado em tokens JWT
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"] 
    ?? throw new InvalidOperationException("A chave JWT_SECRET_KEY não foi configurada.");

// Registra os esquemas de autenticação JWT no pipeline
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;// Permite HTTP não seguro em ambiente de desenvolvimento local
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ClockSkew = TimeSpan.Zero // Elimina tolerância de horário para expiração precisa do token
    };
});
builder.Services.AddAuthorization();

// Registra o TokenService para que possa ser injetado nos Controllers (uma instância por requisição HTTP)
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();


// configura o CORS para permitir que o frontend acesse a API sem bloqueios de origem cruzada
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// pipeline de execução da aplicação
var app = builder.Build();

// Bloco de inicialização de infraestrutura: Garante criação automática do banco ao subir no Docker
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Cria automaticamente o banco de dados e as tabelas do Identity e da aplicação usando as migrations, caso ainda não existam. 
    dbContext.Database.Migrate();

    // Popula o banco de dados com dados iniciais (usuário padrão, produtos e listas de relevância).
    await DataSeeder.SeedAsync(scope.ServiceProvider, builder.Configuration);
}

// Configurações específicas para ambiente de desenvolvimento, como documentação via Swagger/OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Kyly API v1");
        options.RoutePrefix = "swagger"; 
    });

}
app.UseHttpsRedirection();

// Ativa a política de CORS liberando acesso ao Frontend
app.UseCors("DevPolicy");

// ativa a autenticação e autorização, garantindo que apenas usuários autenticados possam acessar os endpoints protegidos
// O UseAuthentication() deve vir antes do UseAuthorization(), pois primeiro precisamos autenticar o usuário e depois verificar suas permissões.
app.UseAuthentication();
app.UseAuthorization();

// Mapeia os endpoints criados nos Controllers
app.MapControllers();

// Inicia o servidor web
app.Run();
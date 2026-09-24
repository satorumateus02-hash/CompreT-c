using HelpDeskMvc.Data;
using HelpDeskMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Cria a aplicação web ASP.NET Core e prepara tudo o que ela vai precisar.
var builder = WebApplication.CreateBuilder(args);

// Define o usuário administrador padrão do sistema.
const string AdminUsername = "admin";
const string AdminPassword = "Admin@123";

// Conecta o projeto ao banco SQLite, que será usado para guardar usuários, produtos e outros dados.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Configura a autenticação e autorização do sistema, incluindo a parte de usuários e roles.
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Ativa o MVC para que as controllers e as views funcionem corretamente.
builder.Services.AddControllersWithViews();

// Habilita a sessão do usuário, para o sistema lembrar quem está logado por um tempo.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
});

// Monta a aplicação depois de configurar tudo.
var app = builder.Build();

// Quando a aplicação iniciar, garante que o banco tenha as tabelas necessárias e que o admin exista.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

    // Aplica as migrações do banco para manter a estrutura correta.
    db.Database.Migrate();

    // Cria o usuário admin se ele ainda não existir.
    var admin = await userManager.FindByNameAsync(AdminUsername);
    if (admin == null)
    {
        admin = new Usuario
        {
            UserName = AdminUsername,
            Email = "admin@compretec.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, AdminPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Não foi possível criar o usuário administrador principal: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}

// Se a aplicação não estiver em ambiente de desenvolvimento, usa a página de erro padronizada.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redireciona para HTTPS para deixar a aplicação mais segura.
app.UseHttpsRedirection();

// Permite servir arquivos estáticos como CSS, JS e imagens.
app.UseStaticFiles();

// Configura o roteamento das URLs para as controllers.
app.UseRouting();

// Ativa a sessão para guardar dados do usuário, como quem está logado.
app.UseSession();

// Ativa autenticação e autorização do sistema.
app.UseAuthentication();
app.UseAuthorization();

// Define a rota padrão: ao abrir o projeto, a página inicial será a de login.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

// Inicia a aplicação.
app.Run();
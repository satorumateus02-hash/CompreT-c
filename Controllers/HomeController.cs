using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HelpDeskMvc.Models;

namespace HelpDeskMvc.Controllers;

public class HomeController : Controller
{
    private static readonly string[] NavItensDisponiveis = ["Home", "Chamados", "Estoque", "Configuracoes"];

    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;

    // Guarda o estado global de cadastro do sistema: true significa liberado, false significa bloqueado.
    public static bool CadastroPermitidoGlobal { get; private set; } = true;

    // Nomes usados para guardar dados na sessão do usuário, como quem está logado e se o cadastro está permitido.
    private const string UsuarioSessionKey = "Usuario";
    private const string CadastroPermitidoSessionKey = "CadastroPermitido";
    private const string CodigoConfirmadoSessionKey = "CodigoConfirmado";
    private const string PermissoesNavSessionKey = "PermissoesNav";

    public HomeController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // Verifica se existe alguém logado na sessão.
    private bool UsuarioLogado()
    {
        if (HttpContext.User.Identity?.IsAuthenticated == true)
            return true;

        return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString(UsuarioSessionKey));
    }

    private static string[] ParsePermissoes(string? permissoes)
    {
        if (string.IsNullOrWhiteSpace(permissoes))
            return [];

        return permissoes
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string SerializarPermissoes(IEnumerable<string> permissoes)
    {
        return string.Join(",", permissoes
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase));
    }

    private bool UsuarioPodeVerNav(string item)
    {
        if (string.Equals(HttpContext.Session.GetString(UsuarioSessionKey), "admin", StringComparison.OrdinalIgnoreCase))
            return true;

        var permissoes = ParsePermissoes(HttpContext.Session.GetString(PermissoesNavSessionKey));
        return permissoes.Contains(item, StringComparer.OrdinalIgnoreCase);
    }

    // Verifica se a pessoa logada é o administrador principal.
    private bool AdminLogado()
    {
        if (HttpContext.User.Identity?.IsAuthenticated == true)
            return string.Equals(HttpContext.User.Identity.Name, "admin", StringComparison.OrdinalIgnoreCase);

        return UsuarioLogado() && string.Equals(HttpContext.Session.GetString(UsuarioSessionKey), "admin", StringComparison.OrdinalIgnoreCase);
    }

    // Lê se o cadastro está liberado na sessão ou no estado global.
    private bool CadastroPermitido()
    {
        var valor = HttpContext.Session.GetString(CadastroPermitidoSessionKey);
        if (valor == "true")
            return true;

        return CadastroPermitidoGlobal;
    }

    // Salva o estado do cadastro na sessão e também no estado global do sistema.
    private void DefinirCadastroPermitido(bool permitido)
    {
        CadastroPermitidoGlobal = permitido;
        HttpContext.Session.SetString(CadastroPermitidoSessionKey, permitido ? "true" : "false");
    }

    // Página principal do sistema. Só abre se o usuário estiver logado.
    public async Task<IActionResult> Index()
    {
        if (!UsuarioLogado())
            return RedirectToAction(nameof(Login));

        var usuarioAtual = HttpContext.Session.GetString(UsuarioSessionKey) ?? HttpContext.User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(usuarioAtual) && !string.Equals(usuarioAtual, "admin", StringComparison.OrdinalIgnoreCase))
        {
            var usuario = await _userManager.FindByNameAsync(usuarioAtual);
            var codigoConfirmado = HttpContext.Session.GetString(CodigoConfirmadoSessionKey);
            ViewBag.ExibirFormularioCodigo = usuario != null && (string.IsNullOrWhiteSpace(codigoConfirmado) || codigoConfirmado != "true");
            ViewBag.CodigoUsuario = usuario?.CodigoAcesso;
        }
        else
        {
            ViewBag.ExibirFormularioCodigo = false;
        }

        if (TempData.ContainsKey("Nome"))
            ViewBag.Nome = TempData["Nome"];
        if (TempData.ContainsKey("Usuario"))
            ViewBag.Usuario = TempData["Usuario"];

        if (TempData.ContainsKey("CodigoErro"))
            ViewBag.CodigoErro = TempData["CodigoErro"];

        return View();
    }

    // Página de privacidade. Também exige que o usuário esteja logado.
    public IActionResult Privacy()
    {
        if (!UsuarioLogado())
            return RedirectToAction(nameof(Login));

        return View();
    }

    // Exibe a tela de login. Se alguém já estiver logado, ele é enviado para a home.
    public IActionResult Login()
    {
        if (UsuarioLogado())
            return RedirectToAction(nameof(Index));

        DefinirCadastroPermitido(true);
        ViewBag.CadastroPermitido = true;
        return View();
    }

    // Tela de configurações do sistema, visível apenas para o administrador.
    public async Task<IActionResult> Configuracoes()
    {
        if (!AdminLogado())
            return RedirectToAction(nameof(Login));

        ViewBag.CadastroPermitido = CadastroPermitido();

        var usuariosListados = await _userManager.Users
            .Where(u => u.UserName != null && u.UserName != "admin")
            .OrderBy(u => u.UserName)
            .ToListAsync();

        ViewBag.Usuarios = usuariosListados;
        ViewBag.NavItens = NavItensDisponiveis;
        return View();
    }

    // Quando o administrador clica no botão, esse método alterna o estado do cadastro.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PermitirCadastro()
    {
        if (!AdminLogado())
            return RedirectToAction(nameof(Login));

        var permitir = !CadastroPermitido();
        DefinirCadastroPermitido(permitir);
        TempData["ConfiguracaoMensagem"] = permitir
            ? "Cadastro de usuários foi permitido."
            : "Cadastro de usuários foi bloqueado.";
        return RedirectToAction(nameof(Configuracoes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SalvarPermissoes(string userName, string[]? permissoes)
    {
        if (!AdminLogado())
            return RedirectToAction(nameof(Login));

        if (string.IsNullOrWhiteSpace(userName))
            return RedirectToAction(nameof(Configuracoes));

        var usuario = await _userManager.FindByNameAsync(userName);
        if (usuario == null)
            return RedirectToAction(nameof(Configuracoes));

        var selecao = permissoes ?? [];
        usuario.PermissoesNav = SerializarPermissoes(selecao);
        await _userManager.UpdateAsync(usuario);

        if (string.Equals(usuario.UserName, HttpContext.Session.GetString(UsuarioSessionKey), StringComparison.OrdinalIgnoreCase))
        {
            HttpContext.Session.SetString(PermissoesNavSessionKey, usuario.PermissoesNav ?? string.Empty);
        }

        TempData["ConfiguracaoMensagem"] = $"Permissões atualizadas para {usuario.UserName}.";
        return RedirectToAction(nameof(Configuracoes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ValidarCodigo(string codigo)
    {
        if (!UsuarioLogado())
            return RedirectToAction(nameof(Login));

        var usuarioAtual = HttpContext.Session.GetString(UsuarioSessionKey);
        if (string.IsNullOrWhiteSpace(usuarioAtual) || string.Equals(usuarioAtual, "admin", StringComparison.OrdinalIgnoreCase))
            return RedirectToAction(nameof(Index));

        var usuario = await _userManager.FindByNameAsync(usuarioAtual);
        if (usuario == null)
        {
            TempData["CodigoErro"] = "Usuário não encontrado.";
            return RedirectToAction(nameof(Index));
        }

        if (string.Equals(usuario.CodigoAcesso, codigo, StringComparison.OrdinalIgnoreCase))
        {
            // Marcar o código como confirmado na sessão
            HttpContext.Session.SetString(CodigoConfirmadoSessionKey, "true");
            HttpContext.Session.SetString(PermissoesNavSessionKey, usuario.PermissoesNav ?? "Home");

            // Remover o código do usuário para que não seja solicitado novamente
            usuario.CodigoAcesso = null;
            await _userManager.UpdateAsync(usuario);

            TempData["Nome"] = usuario.UserName;
            TempData["Usuario"] = usuario.UserName;
            return RedirectToAction(nameof(Index));
        }

        TempData["CodigoErro"] = "Código inválido. Verifique o código do admin.";
        return RedirectToAction(nameof(Index));
    }

    // Abre a tela de cadastro, mas só funciona se o cadastro estiver liberado.
    public IActionResult Register()
    {
        DefinirCadastroPermitido(true);
        return View("Register");
    }

    // Valida os dados do login usando o Identity do ASP.NET Core.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(HelpDeskMvc.Models.LoginViewModel model)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(model.Usuario))
            errors.Add("Usuário é obrigatório.");
        if (string.IsNullOrWhiteSpace(model.Senha))
            errors.Add("Senha é obrigatória.");

        if (errors.Count > 0)
        {
            TempData["LoginErrors"] = string.Join(" ", errors);
            return View(model);
        }

        var userName = model.Usuario ?? string.Empty;
        var password = model.Senha ?? string.Empty;

        var user = await _userManager.FindByNameAsync(userName);
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            HttpContext.Session.SetString(UsuarioSessionKey, user.UserName ?? user.Email ?? "usuario");
            HttpContext.Session.SetString(PermissoesNavSessionKey, user.PermissoesNav ?? (string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase) ? "Home,Chamados,Estoque,Configuracoes" : "Home"));
            HttpContext.Session.SetString(CodigoConfirmadoSessionKey, string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase) ? "true" : "false");
            TempData["Nome"] = user.UserName;
            TempData["Usuario"] = user.UserName;
            return RedirectToAction(nameof(Index));
        }

        TempData["LoginErrors"] = "Credenciais inválidas.";
        return View(model);
    }

    // Este método cria um novo usuário usando a estrutura real do Identity do ASP.NET Core.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(HelpDeskMvc.Models.RegisterViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Usuario))
            ModelState.AddModelError(nameof(model.Usuario), "Usuário é obrigatório.");

        if (string.IsNullOrWhiteSpace(model.Senha))
            ModelState.AddModelError(nameof(model.Senha), "Senha é obrigatória.");

        if (string.IsNullOrWhiteSpace(model.ConfirmarSenha))
            ModelState.AddModelError(nameof(model.ConfirmarSenha), "Confirmação da senha é obrigatória.");

        if (!string.IsNullOrWhiteSpace(model.Senha) && !string.IsNullOrWhiteSpace(model.ConfirmarSenha) && model.Senha != model.ConfirmarSenha)
            ModelState.AddModelError(nameof(model.ConfirmarSenha), "As senhas não conferem.");

        if (string.IsNullOrWhiteSpace(model.Email))
            ModelState.AddModelError(nameof(model.Email), "E-mail é obrigatório.");

        if (!ModelState.IsValid)
            return View("Register", model);

        var usuarioExistente = await _userManager.FindByNameAsync(model.Usuario!);
        if (usuarioExistente != null)
        {
            ModelState.AddModelError(nameof(model.Usuario), "Este usuário já existe.");
            return View("Register", model);
        }

        var novoUsuario = new Usuario
        {
            UserName = model.Usuario,
            Email = model.Email,
            CodigoAcesso = GerarCodigoAcesso(),
            PermissoesNav = "Home"
        };

        var resultado = await _userManager.CreateAsync(novoUsuario, model.Senha!);
        if (!resultado.Succeeded)
        {
            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, erro.Description);
            }

            return View("Register", model);
        }

        TempData["LoginErrors"] = "Cadastro realizado com sucesso! O código de acesso foi gerado e pode ser visualizado pelo administrador na área de configurações.";
        return RedirectToAction(nameof(Login));
    }

    private static string GerarCodigoAcesso()
    {
        var random = new Random();
        return random.Next(10000, 99999).ToString();
    }

    // Fecha a sessão e manda o usuário de volta para a tela de login.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    // Página de erro padrão do ASP.NET Core.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

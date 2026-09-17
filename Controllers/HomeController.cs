using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HelpDeskMvc.Models;

namespace HelpDeskMvc.Controllers;

public class HomeController : Controller
{
    public static bool CadastroPermitidoGlobal { get; private set; } = false;

    private const string UsuarioSessionKey = "Usuario";
    private const string CadastroPermitidoSessionKey = "CadastroPermitido";

    private bool UsuarioLogado()
    {
        return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString(UsuarioSessionKey));
    }

    private bool AdminLogado()
    {
        return UsuarioLogado() && HttpContext.Session.GetString(UsuarioSessionKey) == "admin";
    }

    private bool CadastroPermitido()
    {
        var valor = HttpContext.Session.GetString(CadastroPermitidoSessionKey);
        if (valor == "true")
            return true;

        return CadastroPermitidoGlobal;
    }

    private void DefinirCadastroPermitido(bool permitido)
    {
        CadastroPermitidoGlobal = permitido;
        HttpContext.Session.SetString(CadastroPermitidoSessionKey, permitido ? "true" : "false");
    }

    public IActionResult Index()
    {
        if (!UsuarioLogado())
            return RedirectToAction(nameof(Login));

        if (TempData.ContainsKey("Nome"))
            ViewBag.Nome = TempData["Nome"];
        if (TempData.ContainsKey("Usuario"))
            ViewBag.Usuario = TempData["Usuario"];

        return View();
    }

    public IActionResult Privacy()
    {
        if (!UsuarioLogado())
            return RedirectToAction(nameof(Login));

        return View();
    }

    public IActionResult Login()
    {
        if (UsuarioLogado())
            return RedirectToAction(nameof(Index));

        DefinirCadastroPermitido(false);
        ViewBag.CadastroPermitido = false;
        return View();
    }

    public IActionResult Configuracoes()
    {
        if (!AdminLogado())
            return RedirectToAction(nameof(Login));

        ViewBag.CadastroPermitido = CadastroPermitido();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PermitirCadastro()
    {
        if (!AdminLogado())
            return RedirectToAction(nameof(Login));

        DefinirCadastroPermitido(true);
        TempData["ConfiguracaoMensagem"] = "Cadastro de usuários foi permitido.";
        return RedirectToAction(nameof(Configuracoes));
    }

    // GET: exibe a mesma view de Login/Cadastro
    public IActionResult Register()
    {
        if (!CadastroPermitido())
        {
            TempData["LoginErrors"] = "Cadastro de novos usuários foi desabilitado. Use o usuário administrador principal.";
            return RedirectToAction(nameof(Login));
        }

        return View("Register");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(HelpDeskMvc.Models.LoginViewModel model)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(model.Usuario))
            errors.Add("Usuário é obrigatório.");
        if (string.IsNullOrWhiteSpace(model.Senha))
            errors.Add("Senha é obrigatória.");

        if (errors.Count > 0)
        {
            TempData["LoginErrors"] = string.Join(" ", errors);
            return View();
        }

        if (model.Usuario == "admin" && model.Senha == "Admin@123")
        {
            HttpContext.Session.SetString(UsuarioSessionKey, "admin");
            DefinirCadastroPermitido(false);
            TempData["Nome"] = "admin";
            TempData["Usuario"] = "admin";
            return RedirectToAction("Index");
        }

        TempData["LoginErrors"] = "Credenciais inválidas.";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(HelpDeskMvc.Models.RegisterViewModel model)
    {
        if (!CadastroPermitido())
        {
            TempData["LoginErrors"] = "Cadastro de novos usuários foi desabilitado. Use o usuário administrador principal.";
            return RedirectToAction(nameof(Login));
        }

        TempData["RegErrors"] = "Cadastro de novos usuários foi desabilitado. Use o usuário administrador principal.";
        return View("Register");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

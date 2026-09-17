using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskMvc.Data;
using HelpDeskMvc.Models;

namespace HelpDeskMvc.Controllers
{
public class EstoqueController : Controller
{
    private bool UsuarioLogado()
    {
        return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Usuario"));
    }

    private readonly AppDbContext _context;

    public EstoqueController(AppDbContext context)
    {
        _context = context;
    }

    // LISTAR PRODUTOS (com busca simples)
    public async Task<IActionResult> Index(string? search)
    {
        if (!UsuarioLogado())
            return RedirectToAction("Login", "Home");

        var query = _context.Produtos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => EF.Functions.Like(p.Nome, $"%{search}%") || EF.Functions.Like(p.Categoria ?? string.Empty, $"%{search}%"));
            ViewData["Search"] = search;
        }

        var produtos = await query
            .OrderBy(p => p.Nome)
            .ToListAsync();

        return View(produtos);
    }

    // VER DETALHES
    public async Task<IActionResult> Detalhes(int id)
    {
        if (!UsuarioLogado())
            return RedirectToAction("Login", "Home");

        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            return NotFound();

        return View(produto);
    }

    // FORMULÁRIO DE CADASTRO
    public IActionResult Criar()
    {
        if (!UsuarioLogado())
            return RedirectToAction("Login", "Home");

        return View();
    }

    // SALVAR PRODUTO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(CriarProdutoViewModel model)
    {
        if (!UsuarioLogado())
            return RedirectToAction("Login", "Home");

        if (!ModelState.IsValid)
            return View(model);

        var produto = new Produto
        {
            Nome = model.Nome,
            Descricao = model.Descricao,
            Preco = model.Preco,
            Quantidade = model.Quantidade,
            Fornecedor = model.Fornecedor,
            Categoria = model.Categoria,
            DataCadastro = DateTime.Now
        };

        _context.Produtos.Add(produto);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // FORMULÁRIO DE EDIÇÃO
    public async Task<IActionResult> Editar(int id)
    {
        if (!UsuarioLogado())
            return RedirectToAction("Login", "Home");

        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            return NotFound();

        return View(produto);
    }

    // SALVAR EDIÇÃO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Produto model)
    {
        if (!UsuarioLogado())
            return RedirectToAction("Login", "Home");

        if (id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            return NotFound();

        produto.Nome = model.Nome;
        produto.Descricao = model.Descricao;
        produto.Preco = model.Preco;
        produto.Quantidade = model.Quantidade;
        produto.Fornecedor = model.Fornecedor;
        produto.Categoria = model.Categoria;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // EXCLUIR PRODUTO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id)
    {
        if (!UsuarioLogado())
            return RedirectToAction("Login", "Home");

        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            return NotFound();

        _context.Produtos.Remove(produto);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}


}
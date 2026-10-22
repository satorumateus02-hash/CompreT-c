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

    // LISTAR PRODUTOS COM BUSCA E FILTROS
    public async Task<IActionResult> Index(
        string? search,
        string? categoria,
        string? fornecedor,
        decimal? precoMin,
        decimal? precoMax,
        int? quantidadeMin,
        bool? estoqueBaixo,
        string? ordenar)
    {
        if (!UsuarioLogado())
            return RedirectToAction("Login", "Home");

        var busca = search?.Trim();
        var categoriaFiltro = categoria?.Trim();
        var fornecedorFiltro = fornecedor?.Trim();
        var ordem = string.IsNullOrWhiteSpace(ordenar) ? "nome" : ordenar.Trim();

        IQueryable<Produto> query = _context.Produtos
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            query = query.Where(p =>
                EF.Functions.Like(p.Nome, $"%{busca}%") ||
                EF.Functions.Like(p.Descricao ?? string.Empty, $"%{busca}%") ||
                EF.Functions.Like(p.Categoria ?? string.Empty, $"%{busca}%") ||
                EF.Functions.Like(p.Fornecedor ?? string.Empty, $"%{busca}%"));
        }

        if (!string.IsNullOrWhiteSpace(categoriaFiltro))
        {
            query = query.Where(p => EF.Functions.Like(p.Categoria ?? string.Empty, $"%{categoriaFiltro}%"));
        }

        if (!string.IsNullOrWhiteSpace(fornecedorFiltro))
        {
            query = query.Where(p => EF.Functions.Like(p.Fornecedor ?? string.Empty, $"%{fornecedorFiltro}%"));
        }

        if (precoMin.HasValue)
        {
            query = query.Where(p => p.Preco >= precoMin.Value);
        }

        if (precoMax.HasValue)
        {
            query = query.Where(p => p.Preco <= precoMax.Value);
        }

        if (quantidadeMin.HasValue)
        {
            query = query.Where(p => p.Quantidade >= quantidadeMin.Value);
        }

        if (estoqueBaixo == true)
        {
            query = query.Where(p => p.Quantidade <= 5);
        }

        var produtos = ordem.ToLowerInvariant() switch
        {
            "preco" => await query.OrderBy(p => p.Preco).ToListAsync(),
            "data" => await query.OrderByDescending(p => p.DataCadastro).ToListAsync(),
            _ => await query.OrderBy(p => p.Nome).ToListAsync()
        };

        ViewData["Search"] = busca;
        ViewData["Categoria"] = categoriaFiltro;
        ViewData["Fornecedor"] = fornecedorFiltro;
        ViewData["PrecoMin"] = precoMin;
        ViewData["PrecoMax"] = precoMax;
        ViewData["QuantidadeMin"] = quantidadeMin;
        ViewData["EstoqueBaixo"] = estoqueBaixo == true;
        ViewData["Ordenar"] = ordem;

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
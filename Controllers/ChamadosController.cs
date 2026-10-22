using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskMvc.Models;
using HelpDeskMvc.Data;


namespace HelpDeskMvc.Controllers

{
    public class ChamadosController : Controller
    {
        private bool UsuarioLogado()
        {
            return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Usuario"));
        }

        // 🔵 Campo para acessar o banco de dados
        private readonly AppDbContext _context;

        // 🔵 Injeção do DbContext
        public ChamadosController(AppDbContext context)
        {
            _context = context;
        }

        // 📋 LISTAGEM COM BUSCA E FILTROS
        public async Task<IActionResult> Index(string? busca, string? status)
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            var textoBusca = busca?.Trim();
            var statusSelecionado = NormalizarStatus(status);

            IQueryable<Chamado> query = _context.Chamados
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(textoBusca))
            {
                query = query.Where(c =>
                    c.Titulo.Contains(textoBusca) ||
                    c.Descricao.Contains(textoBusca));
            }

            if (!string.IsNullOrWhiteSpace(statusSelecionado))
            {
                query = query.Where(c => c.Status == statusSelecionado);
            }

            var viewModel = new ListarChamadosViewModel
            {
                Busca = textoBusca,
                Status = statusSelecionado,
                Chamados = await query
                    .OrderByDescending(c => c.DataAbertura)
                    .ToListAsync()
            };

            ViewData["Busca"] = textoBusca;
            ViewData["Status"] = statusSelecionado;

            return View(viewModel);
        }

        private static string? NormalizarStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;

            var valor = status.Trim();

            return valor.ToLowerInvariant() switch
            {
                "aberto" => "Aberto",
                "emandamento" or "em andamento" or "em atendimento" => "Em atendimento",
                "resolvido" => "Resolvido",
                "cancelado" => "Cancelado",
                "fechado" => "Fechado",
                _ => null
            };
        }

        // 🔎 DETALHES
        public async Task<IActionResult> Detalhes(int id)
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            // 🔵 Busca o chamado pelo ID
            var chamado = await _context.Chamados
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chamado == null)
                return NotFound();

            return View(chamado);
        }

        // 📝 FORMULÁRIO DE CRIAÇÃO
        public IActionResult Criar()
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            return View();
        }

        // 📝 SALVAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(CriarChamadoViewModel model)
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
                return View(model);

            var chamado = new Chamado
            {
                Titulo = string.IsNullOrWhiteSpace(model.Titulo) ? model.ClienteNome : model.Titulo,
                Descricao = model.Descricao,
                Status = "Aberto",
                DataAbertura = DateTime.Now,
                DataFechamento = null,
                ClienteNome = model.ClienteNome,
                Telefone = model.Telefone,
                Endereco = model.Endereco,
                Complemento = model.Complemento
            };

            _context.Chamados.Add(chamado);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ✏️ FORMULÁRIO DE EDIÇÃO
        public async Task<IActionResult> Editar(int id)
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            var chamado = await _context.Chamados
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chamado == null)
                return NotFound();

            return View(chamado);
        }

        // ✏️ SALVAR EDIÇÃO (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Chamado model)
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var chamado = await _context.Chamados
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chamado == null)
                return NotFound();

            chamado.Titulo = model.Titulo;
            chamado.Descricao = model.Descricao;
            chamado.Status = model.Status;
            chamado.ClienteNome = model.ClienteNome;
            chamado.Telefone = model.Telefone;
            chamado.Endereco = model.Endereco;
            chamado.Complemento = model.Complemento;

            // Se o chamado foi fechado
            if (model.Status == "Fechado" && chamado.DataFechamento == null)
            {
                chamado.DataFechamento = DateTime.Now;
            }

            // Se voltar para Aberto
            if (model.Status != "Fechado")
            {
                chamado.DataFechamento = null;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 🗑️ EXCLUIR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            var chamado = await _context.Chamados
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chamado == null)
                return NotFound();

            _context.Chamados.Remove(chamado);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 🔌 API
        [HttpPost]
        [Route("api/chamados")]
        public async Task<IActionResult> CriarViaApi(
            [FromBody] CriarChamadoViewModel model)
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var chamado = new Chamado
            {
                Titulo = string.IsNullOrWhiteSpace(model.Titulo) ? model.ClienteNome : model.Titulo,
                Descricao = model.Descricao,
                Status = "Aberto",
                DataAbertura = DateTime.Now,
                DataFechamento = null,
                ClienteNome = model.ClienteNome,
                Telefone = model.Telefone,
                Endereco = model.Endereco,
                Complemento = model.Complemento
            };

            _context.Chamados.Add(chamado);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Detalhes),
                new { id = chamado.Id },
                chamado
            );
        }

        // ℹ️ SOBRE
        public IActionResult Sobre()
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            return View();
        }
    }
}

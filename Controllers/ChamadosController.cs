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

        // 📋 LISTAGEM
        public async Task<IActionResult> Index()
        {
            if (!UsuarioLogado())
                return RedirectToAction("Login", "Home");

            // 🔵 Busca todos os chamados no banco
            var chamados = await _context.Chamados.ToListAsync();

            return View(chamados);
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
                Titulo = model.ClienteNome,
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
                Titulo = model.ClienteNome,
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

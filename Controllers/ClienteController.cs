using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using farmacia.Models;

namespace farmacia.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AppDbContext _db;

        public ClienteController(AppDbContext context)
        {
            _db = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Cliente> listaClientes = new List<Cliente>();

            var clientes = await _db.clientes.Join(
                _db.pessoas, cliente => cliente.PessoaId,
                p => p.PessoaId,
                (cliente, p) => new
                {
                    PessoaId = p.PessoaId,
                    nome = p.nome,
                    sexo = p.sexo,
                    bi = p.bi,
                    telefone = p.telefone,
                    email = p.email
                }
            ).ToListAsync();

            foreach (var item in clientes)
            {
                Pessoa pessoa = new Pessoa();
                pessoa.PessoaId = item.PessoaId;
                pessoa.nome = item.nome;
                pessoa.sexo = item.sexo;
                pessoa.bi = item.bi;
                pessoa.telefone = item.telefone;
                pessoa.email = item.email;

                Cliente cliente = new Cliente();
                cliente.PessoaId = pessoa.PessoaId;
                cliente.pessoa = pessoa;

                listaClientes.Add(cliente);
            }
            return View(listaClientes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _db.clientes == null)
            {
                return NotFound();
            }
            var cliente = await _db.clientes
                .FirstOrDefaultAsync(m => m.ClienteId == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> Create(Cliente c)
        {
            Pessoa pessoa = new Pessoa();
            pessoa.nome = HttpContext.Request.Form["nome"];
            pessoa.sexo = HttpContext.Request.Form["sexo"];
            pessoa.bi = HttpContext.Request.Form["bi"];
            pessoa.telefone = Int32.Parse(HttpContext.Request.Form["telefone"]);
            pessoa.email = HttpContext.Request.Form["email"];

            _db.pessoas.Add(pessoa);
            await _db.SaveChangesAsync();

            Cliente cliente = new Cliente();
            cliente.PessoaId = pessoa.PessoaId;
            _db.clientes.Add(cliente);
            await _db.SaveChangesAsync();

            TempData["msg"] = "add";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            List<Cliente> listaClientes = new List<Cliente>();
            var clientes = await _db.clientes.Join(
                _db.pessoas, cliente => cliente.PessoaId,
                pessoa => pessoa.PessoaId,
                (cliente, pessoa) => new
                {
                    PessoaId = pessoa.PessoaId,
                    nome = pessoa.nome,
                    sexo = pessoa.sexo,
                    bi = pessoa.bi,
                    telefone = pessoa.telefone,
                    email = pessoa.email
                }
            ).Where(p => p.PessoaId == id)
            .ToListAsync();

            Pessoa pessoa = new Pessoa();
            pessoa.PessoaId = clientes[0].PessoaId;
            pessoa.nome = clientes[0].nome;
            pessoa.sexo = clientes[0].sexo;
            pessoa.bi = clientes[0].bi;
            pessoa.telefone = clientes[0].telefone;
            pessoa.email = clientes[0].email;

            Cliente cliente = new Cliente();
            cliente.PessoaId = pessoa.PessoaId;
            cliente.pessoa = pessoa;

            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Edit()
        {
            Pessoa pessoa = new Pessoa();
            pessoa.nome = HttpContext.Request.Form["nome"];
            pessoa.sexo = HttpContext.Request.Form["sexo"];
            pessoa.bi = HttpContext.Request.Form["bi"];
            pessoa.telefone = Int32.Parse(HttpContext.Request.Form["telefone"]);
            pessoa.email = HttpContext.Request.Form["email"];

            var clienteId = Int32.Parse(HttpContext.Request.Form["ClienteId"]);

            var exist = _db.pessoas.Where(x => x.PessoaId == clienteId).FirstOrDefault();
            //var cliente = _db.pessoas.Where(x => x.PessoaId == clienteId).FirstOrDefault();

            exist.nome = HttpContext.Request.Form["nome"];
            exist.sexo = HttpContext.Request.Form["sexo"];
            exist.bi = HttpContext.Request.Form["bi"];
            exist.telefone = Int32.Parse(HttpContext.Request.Form["telefone"]);
            exist.email = HttpContext.Request.Form["email"];

            _db.pessoas.Update(exist);

            _db.SaveChanges();

            TempData["msg"] = "edit";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _db.clientes == null)
            {
                return NotFound();
            }
            var pessoa = await _db.pessoas.FirstOrDefaultAsync(m => m.PessoaId == id);
            Cliente cliente = new Cliente();
            cliente.pessoa = pessoa;
            if (pessoa == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_db.clientes == null)
            {
                return Problem("Entity set 'AppDbContext.clientes'  is null.");
            }
            var cliente = _db.clientes.Single(c => c.PessoaId == id);
            var isExistIdCliente = _db.marcacoes.FirstOrDefault(m => m.ClienteId == cliente.ClienteId);

            if (isExistIdCliente == null)
            {
                var pessoa = _db.pessoas.Single(p => p.PessoaId == id);
                if (cliente != null && pessoa != null)
                {
                    _db.pessoas.Remove(pessoa);
                    _db.clientes.Remove(cliente);
                }

                TempData["msg"] = "delete1";

                _db.SaveChanges();
            }
            else
            {
                TempData["msg"] = "delete";
                RedirectToAction("Index", "Cliente");
            }

            return RedirectToAction(nameof(Index));
        }
        private bool ClienteExists(int id)
        {
            return (_db.clientes?.Any(e => e.ClienteId == id)).GetValueOrDefault();
        }

        
        [HttpGet]
        public JsonResult getTotalCliente()
        {
            var clientes = _db.clientes.Count();
            return Json(clientes);
        }
    }
}

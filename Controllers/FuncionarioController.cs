using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using farmacia.Models;

namespace farmacia.Controllers
{
    public class FuncionarioController : Controller
    {
        private readonly AppDbContext _db;
        public FuncionarioController(AppDbContext db)
        {
            this._db = db;
        }
        public async Task<IActionResult> Index()
        {
            var funcionarios = _db.funcionarios
            .Join(
                _db.pessoas,
                funcionario => funcionario.PessoaId,
                pessoa => pessoa.PessoaId,
                (funcionario, pessoa) => new
                {
                    pessoa = pessoa,
                    funcionario = funcionario
                }
            )
            .Join(
                _db.tipoFuncionarios,
                func => func.funcionario.TipoFuncionarioId,
                tipoFunc => tipoFunc.TipoFuncionarioId,
                (func, tipoFunc) => new
                {
                    dados = new
                    {
                        func.pessoa.nome,
                        func.pessoa.email,
                        func.pessoa.bi,
                        func.pessoa.telefone,
                        func.pessoa.PessoaId,
                        func.funcionario.FuncionarioId,
                        func.funcionario.numeroOrdem,
                        func.funcionario.nif,
                        tipoFunc.tipo
                    }
                }
            )
            .ToList();
            ViewBag.funcionarios = funcionarios;
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var tipoFuncionarios = _db.tipoFuncionarios.ToList();
            return View(tipoFuncionarios);
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

            Funcionario funcionario = new Funcionario();
            funcionario.numeroOrdem = HttpContext.Request.Form["ordem"];
            funcionario.nif = HttpContext.Request.Form["nif"];
            funcionario.TipoFuncionarioId = Int32.Parse(HttpContext.Request.Form["tipoFuncionario"]);
            funcionario.PessoaId = pessoa.PessoaId;

            _db.funcionarios.Add(funcionario);
            await _db.SaveChangesAsync();

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
            if (pessoa == null)
            {
                return NotFound();
            }
            return View(pessoa);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_db.clientes == null)
            {
                return Problem("Entity set 'AppDbContext.funcionario'  is null.");
            }
            var isExistIdPessoa = _db.funcionarios.FirstOrDefault(funcionario => funcionario.PessoaId == id);
            var isExistIdFuncionario = _db.servicoFuncionarios.FirstOrDefault(funcionario => funcionario.FuncionarioId == isExistIdPessoa.FuncionarioId);

            if (isExistIdFuncionario == null)
            {
                var funcionario = _db.funcionarios.Single(c => c.PessoaId == id);
                var pessoa = _db.pessoas.Single(p => p.PessoaId == id);

                if (funcionario != null && pessoa != null)
                {
                    _db.pessoas.Remove(pessoa);
                    _db.funcionarios.Remove(funcionario);
                }
                _db.SaveChanges();
            }
            else
            {
                //notificacao
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var funcionarios = _db.funcionarios
            .Join(
                _db.pessoas,
                funcionario => funcionario.PessoaId,
                pessoa => pessoa.PessoaId,
                (funcionario, pessoa) => new
                {
                    pessoa = pessoa,
                    funcionario = funcionario
                }
            )
            .Join(
                _db.tipoFuncionarios,
                func => func.funcionario.TipoFuncionarioId,
                tipoFunc => tipoFunc.TipoFuncionarioId,
                (func, tipoFunc) => new
                {
                    dados = new
                    {
                        func.pessoa.nome,
                        func.pessoa.email,
                        func.pessoa.sexo,
                        func.pessoa.bi,
                        func.pessoa.telefone,
                        func.pessoa.PessoaId,
                        func.funcionario.FuncionarioId,
                        func.funcionario.numeroOrdem,
                        func.funcionario.nif,
                        tipoFunc.tipo
                    }
                }
            )
            .Where(p => p.dados.PessoaId == id)
            .ToList();

            var tipoFuncionarios = _db.tipoFuncionarios.ToList();
            ViewBag.funcionarios = funcionarios;
            ViewBag.tipoFuncionarios = tipoFuncionarios;

            return View();
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(Cliente c)
        {
            var pessoaId = Int32.Parse(HttpContext.Request.Form["pessoaId"]);

            var pessoaExist = _db.pessoas.Where(x => x.PessoaId == pessoaId).FirstOrDefault();
            pessoaExist.nome = HttpContext.Request.Form["nome"];
            pessoaExist.sexo = HttpContext.Request.Form["sexo"];
            pessoaExist.bi = HttpContext.Request.Form["bi"];
            pessoaExist.telefone = Int32.Parse(HttpContext.Request.Form["telefone"]);
            pessoaExist.email = HttpContext.Request.Form["email"];
            _db.pessoas.Update(pessoaExist);

            var funcExist = _db.funcionarios.Where(x => x.PessoaId == pessoaId).FirstOrDefault();
            funcExist.numeroOrdem = HttpContext.Request.Form["ordem"];
            funcExist.nif = HttpContext.Request.Form["nif"];
            funcExist.TipoFuncionarioId = Int32.Parse(HttpContext.Request.Form["tipoFuncionario"]);
            _db.pessoas.Update(pessoaExist);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult getTotalFuncionario()
        {
            var funcionarios = _db.funcionarios.Count();
            return Json(funcionarios);
        }
    }
}
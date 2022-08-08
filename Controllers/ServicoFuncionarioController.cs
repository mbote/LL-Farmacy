using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using farmacia.Models;

namespace farmacia.Controllers
{
    public class ServicoFuncionarioController : Controller
    {
        private readonly AppDbContext _db;
        public ServicoFuncionarioController(AppDbContext db)
        {
            this._db = db;
        }

        public async Task<IActionResult> Index()
        {
            var servicos = await _db.servicos
            .Join(
                _db.servicoFuncionarios,
                servico => servico.ServicoId,
                servicoFuncionario => servicoFuncionario.ServicoId,
                (servico, servicoFuncionario) => new
                {
                    servico = servico,
                    funcionarioId = servicoFuncionario.FuncionarioId,
                    servicoFuncionario = servicoFuncionario
                }
            )
           .Join(
                _db.funcionarios,
                servicoFuncionario => servicoFuncionario.funcionarioId,
                funcionario1 => funcionario1.FuncionarioId,
                (servicoFuncionario, funcionario) => new
                {
                    servico = servicoFuncionario.servico,
                    funcionario = funcionario,
                    servicoFuncionario = servicoFuncionario.servicoFuncionario
                }
            )
            .Join(
                _db.pessoas,
                funcionario => funcionario.funcionario.PessoaId,
                pessoa => pessoa.PessoaId,
                (funcionario, pessoa) => new
                {
                    pessoa = pessoa,
                    servico = funcionario.servico,
                    funcionario = funcionario.funcionario,
                    servicoFuncionario = funcionario.servicoFuncionario
                }
            )
            .Join(
                _db.tipoFuncionarios,
                servico => servico.funcionario.TipoFuncionarioId,
                tipoFuncionario => tipoFuncionario.TipoFuncionarioId,
                (servico, tipoFuncionario) => new
                {
                    nomeFuncionario = servico.pessoa.nome,
                    nomeServico = servico.servico.servico,
                    tipoFuncionario = tipoFuncionario.tipo,
                    idServicoFuncionario = servico.servicoFuncionario.ServicoFuncionarioId
                }
            )
            .ToListAsync();

            ViewBag.servicos = servicos;
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var funcionarios = _db.funcionarios
            .Join(
                _db.pessoas,
                funcionario => funcionario.PessoaId,
                pessoa => pessoa.PessoaId,
                (funcionario, pessoa) => new
                {
                    nome = pessoa.nome,
                    idFuncionario = funcionario.FuncionarioId
                }
            ).ToList();
            ViewBag.funcionarios = funcionarios;

            var tipoFuncionarios = _db.tipoFuncionarios.ToList();
            ViewBag.tipoFuncionarios = tipoFuncionarios;

            var servicos = _db.servicos.ToList();
            ViewBag.servicos = servicos;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Servico s)
        {
            ServicoFuncionario servico = new ServicoFuncionario();
            servico.FuncionarioId = Int32.Parse(HttpContext.Request.Form["idFuncionario"]);
            servico.ServicoId = Int32.Parse(HttpContext.Request.Form["idServico"]);

            _db.Add(servico);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.servicoFuncionarios == null)
            {
                return NotFound();
            }

            if (_db.servicos == null)
            {
                return Problem("Entity set 'AppDbContext.servicoFuncionarios'  is null.");
            }
            var isExistServicoFuncionario = _db.agendas.FirstOrDefault(agenda => agenda.ServicoFuncionarioId == id);

            if (isExistServicoFuncionario == null)
            {
                var servico = await _db.servicoFuncionarios
                       .FirstOrDefaultAsync(m => m.ServicoFuncionarioId == id);
                _db.servicoFuncionarios.Remove(servico);
                await _db.SaveChangesAsync();
            }
            else
            {
                // notificacao
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int? id)
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
                _db.servicoFuncionarios,
                funcionario => funcionario.funcionario.FuncionarioId,
                servicoFuncionario => servicoFuncionario.FuncionarioId,
                (funcionario, servicoFuncionario) => new
                {
                    nome = funcionario.pessoa.nome,
                    idFuncionario = funcionario.funcionario.FuncionarioId,
                    idServicoFuncionario = servicoFuncionario.ServicoFuncionarioId
                }
            )
            .Where(p => p.idServicoFuncionario == id)
            .ToList();

            ViewBag.funcionarios = funcionarios;

            var tipoFuncionarios = _db.tipoFuncionarios.ToList();
            ViewBag.tipoFuncionarios = tipoFuncionarios;

            var servicos = _db.servicos.ToList();
            ViewBag.servicos = servicos;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Servico servico)
        {
            var servicoId = Int32.Parse(HttpContext.Request.Form["id"]);
            var exist = _db.servicoFuncionarios.Where(x => x.ServicoFuncionarioId == servicoId).FirstOrDefault();

            exist.ServicoId = Int32.Parse(HttpContext.Request.Form["idServico"]);

            if (exist != null)
            {
                _db.servicoFuncionarios.Update(exist);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
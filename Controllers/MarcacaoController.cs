using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using farmacia.Models;

namespace farmacia.Controllers
{
    public class MarcacaoController : Controller
    {
        private readonly AppDbContext _db;
        public MarcacaoController(AppDbContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var agenda = _db.marcacoes
            .Join(
               _db.agendas,
               marcacao => marcacao.AgendaId,
               agenda => agenda.AgendaId,
               (marcacao, agenda) => new
               {
                   marcacao = marcacao,
                   agenda = agenda,
               }
            )
           .Join(
                _db.horarios,
                agenda => agenda.agenda.HorarioId,
                horario => horario.HorarioId,
                (agenda, horario) => new
                {
                    agenda = agenda,
                    horario = horario
                }
            )
             .Join(
                _db.servicoFuncionarios,
                agenda => agenda.agenda.agenda.ServicoFuncionarioId,
                servicoFuncionario => servicoFuncionario.ServicoFuncionarioId,
                (agenda, servicoFuncionario) => new
                {
                    agenda = agenda,
                    servicoFuncionario = servicoFuncionario
                }
            )
            .Join(
                _db.servicos,
                servicoFuncionario => servicoFuncionario.servicoFuncionario.ServicoId,
                servico => servico.ServicoId,
                (servicoFuncionario, servico) => new
                {
                    servicoFuncionario = servicoFuncionario,
                    servico = servico
                }
            ).
            Join(
                _db.funcionarios,
                servicoFuncionario => servicoFuncionario.servicoFuncionario.servicoFuncionario.FuncionarioId,
                funcionario => funcionario.FuncionarioId,
                (servicoFuncionario, funcionario) => new
                {
                    servicoFuncionario = servicoFuncionario,
                    funcionario = funcionario
                }
            ).
              Join(
                _db.tipoFuncionarios,
                funcionario => funcionario.funcionario.TipoFuncionarioId,
                tipoFuncionarios => tipoFuncionarios.TipoFuncionarioId,
                (funcionario, tipoFuncionarios) => new
                {
                    funcionario = funcionario,
                    tipoFuncionarios = tipoFuncionarios
                }
            )
            .Join(
                _db.clientes,
                marcacao => marcacao.funcionario.servicoFuncionario.servicoFuncionario.agenda.agenda.marcacao.ClienteId,
                cliente => cliente.ClienteId,
                (marcacao, cliente) => new
                {
                    marcacao = marcacao,
                    cliente = cliente
                }
            )
            /*.Join(
                _db.pessoas,
                funcionario => funcionario.marcacao.funcionario.funcionario.PessoaId,
                pessoa => pessoa.PessoaId,
                (funcionario, pessoa) => new
                {
                    funcionario = funcionario,
                    pessoa = pessoa,
                    nomeFuncionario = new { pessoa.nome }
                }
            )*/
            .Join(
                _db.pessoas,
                cliente => cliente.cliente.PessoaId,
                pessoa => pessoa.PessoaId,
                (cliente, pessoa) => new
                {
                    nomeCliente = pessoa.nome,
                    nomeServico = cliente.marcacao.funcionario.servicoFuncionario.servico.servico,
                    //nomeFuncionario = pessoa.nome,
                    tipoFuncionario = cliente.marcacao.tipoFuncionarios.tipo,
                    horario = cliente.marcacao.funcionario.servicoFuncionario.servicoFuncionario.agenda.horario.dia + "  -  " + cliente.marcacao.funcionario.servicoFuncionario.servicoFuncionario.agenda.horario.hora,
                    idMarcacao = cliente.marcacao.funcionario.servicoFuncionario.servicoFuncionario.agenda.agenda.marcacao.MarcacaoId,
                }
            )
            .ToList();
            //HttpContext.Response.WriteAsJsonAsync(agenda);
            ViewBag.agenda = agenda;
            return View();
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return RedirectToAction("Create", "Cliente");
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
            var nomeCliente = HttpContext.Request.Form["consulta"];
            var idHorario = HttpContext.Request.Form["idHorario"];
            var HorarioId = Int32.Parse(idHorario[0]);

            var pessoa = _db.pessoas
            .Join(
                _db.clientes,
                pessoa => pessoa.PessoaId,
                cliente => cliente.PessoaId,
                (pessoa, cliente) => new
                {
                    nome = pessoa.nome,
                    idCliente = cliente.ClienteId
                }
            )
            .Where(p => p.nome == nomeCliente[0])
            .ToList();

            if (pessoa != null)
            {
                Marcacao marcacao = new Marcacao();
                marcacao.ClienteId = pessoa[0].idCliente;
                marcacao.AgendaId = HorarioId;
                _db.Add(marcacao);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.agendas == null)
            {
                return NotFound();
            }
            var marcacao = await _db.marcacoes
                .FirstOrDefaultAsync(m => m.MarcacaoId == id);

            if (_db.marcacoes == null)
            {
                return Problem("Entity set 'AppDbContext.agendas'  is null.");
            }

            _db.marcacoes.Remove(marcacao);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult getCliente()
        {
            var pessoa = _db.pessoas
            .Join(
                _db.clientes,
                pessoa => pessoa.PessoaId,
                cliente => cliente.PessoaId,
                (pessoa, cliente) => new
                {
                    pessoa = pessoa
                }
            ).
            Select(p => p.pessoa.nome)
            .ToList();
            return Json(pessoa);
        }

        [HttpGet]
        public JsonResult getServicos()
        {
            var servicos = _db.servicos.ToList();
            return Json(servicos);
        }


        [HttpGet]
        public JsonResult getServicoFuncionario(int? id)
        {
            var servicos = _db.servicoFuncionarios
            .Join(
                _db.funcionarios,
                servicoFuncionario => servicoFuncionario.FuncionarioId,
                funcionario => funcionario.FuncionarioId,
                (servicoFuncionario, funcionario) => new
                {
                    servicoFuncionario = servicoFuncionario,
                    funcionario = funcionario
                }
            )
            .Join(
                _db.pessoas,
                funcionario => funcionario.funcionario.PessoaId,
                pessoa => pessoa.PessoaId,
                (funcionario, pessoa) => new
                {
                    idServcioFuncionario = funcionario.servicoFuncionario.ServicoFuncionarioId,
                    nome = pessoa.nome,
                    idServico = funcionario.servicoFuncionario.ServicoId
                }
            )
            .Where(p => p.idServico == id)
            .ToList();
            return Json(servicos);
        }
        [HttpGet]
        public JsonResult getHorario(int? id)
        {
            var horarios = _db.servicoFuncionarios
            .Join(
                _db.agendas,
                servicoFuncionario => servicoFuncionario.ServicoFuncionarioId,
                agenda => agenda.ServicoFuncionarioId,
                (servicoFuncionario, agenda) => new
                {
                    servicoFuncionario = servicoFuncionario,
                    agenda = agenda
                }
            )
            .Join(
                _db.horarios,
                agenda => agenda.agenda.HorarioId,
                horario => horario.HorarioId,
                (agenda, horario) => new
                {
                    idAgenda = agenda.agenda.AgendaId,
                    horario = horario.dia + " às " + horario.hora,
                    idServicoFuncionario = agenda.agenda.ServicoFuncionarioId
                }
            )
            .Where(p => p.idServicoFuncionario == id)
            .ToList();
            return Json(horarios);
        }

        [HttpGet]
        public JsonResult getTotalMacarcoes()
        {
            var marcacoes = _db.marcacoes.Count();
            return Json(marcacoes);
        }
    }
}




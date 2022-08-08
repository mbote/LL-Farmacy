using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using farmacia.Models;

namespace farmacia.Controllers
{
    public class AgendaController : Controller
    {
        private readonly AppDbContext _db;
        public AgendaController(AppDbContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var agenda = _db.agendas
            .Join(
                _db.horarios,
                agenda => agenda.HorarioId,
                horario => horario.HorarioId,
                (agenda, horario) => new
                {
                    agenda = agenda,
                    horario = horario
                }
            )
            .Join(
               _db.servicoFuncionarios,
               agenda => agenda.agenda.ServicoFuncionarioId,
               servivoFuncionario => servivoFuncionario.ServicoFuncionarioId,
               (agenda, servivoFuncionario) => new
               {
                   agenda = agenda.agenda,
                   horario = agenda.horario,
                   servivoFuncionario = servivoFuncionario
               }
            )
            .Join(
                _db.servicos,
                servicoFuncionario => servicoFuncionario.servivoFuncionario.ServicoId,
                servico => servico.ServicoId,
                (servicoFuncionario, servico) => new
                {
                    servicoFuncionario = servicoFuncionario.servivoFuncionario,
                    servico = servico,
                    horario = servicoFuncionario.horario,
                    agenda = servicoFuncionario.agenda,
                }
            )
            .Join(
                _db.funcionarios,
                servicoFuncionario => servicoFuncionario.servicoFuncionario.FuncionarioId,
                funcionario => funcionario.FuncionarioId,
                (servicoFuncionario, funcionario) => new
                {
                    servicoFuncionario = servicoFuncionario.servicoFuncionario,
                    servico = servicoFuncionario.servico,
                    horario = servicoFuncionario.horario,
                    funcionario = funcionario,
                    agenda = servicoFuncionario.agenda,
                }
            )
            .Join(
                _db.tipoFuncionarios,
                funcionario => funcionario.funcionario.TipoFuncionarioId,
                tipoFuncionario => tipoFuncionario.TipoFuncionarioId,
                (funcionario, tipoFuncionario) => new
                {

                    servicoFuncionario = funcionario.servicoFuncionario,
                    servico = funcionario.servico,
                    horario = funcionario.horario,
                    funcionario = funcionario.funcionario,
                    tipoFuncionario = tipoFuncionario,
                    agenda = funcionario.agenda,
                }
            )
            .Join(
                _db.pessoas,
                funcionario => funcionario.funcionario.PessoaId,
                pessoa => pessoa.PessoaId,
                (funcionario, pessoa) => new
                {
                    nomeServico = funcionario.servico.servico,
                    nomeFuncionario = pessoa.nome,
                    tipoFuncionario = funcionario.tipoFuncionario.tipo,
                    horario = funcionario.horario.dia + "  as  " + funcionario.horario.hora,
                    idAgenda = funcionario.agenda.AgendaId
                }
            )
            .ToList();
            ViewBag.agenda = agenda;
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
                    idFuncionario = funcionario.FuncionarioId,
                    nome = pessoa.nome
                }
            )
            /*.Join(
                _db.servicoFuncionarios,
                funcionario => funcionario.funcionario.FuncionarioId,
                servicoFuncionario => servicoFuncionario.FuncionarioId,
                (funcionario, servicoFuncionario) => new {
                    idFuncionario = funcionario.funcionario.FuncionarioId,
                    nome = funcionario.pessoa.nome
                }
            )*/
            .ToList();
            ViewBag.funcionarios = funcionarios;

            var horarios = _db.horarios.ToList();
            ViewBag.horarios = horarios;

            return View();
        }


        [HttpGet]
        public JsonResult getServicoFuncionario(int? id)
        {
            var servicos = _db.servicoFuncionarios
            .Join(
                _db.servicos,
                servicoFuncionario => servicoFuncionario.ServicoId,
                servico => servico.ServicoId,
                (servicoFuncionario, servico) => new
                {
                    servicoFuncionario = servicoFuncionario,
                    servico = servico
                }
            )
            .Join(
                _db.funcionarios,
                servicoFuncionario => servicoFuncionario.servicoFuncionario.FuncionarioId,
                funcionario => funcionario.FuncionarioId,
                (servicoFuncionario, funcionario) => new
                {
                    idServico = servicoFuncionario.servico.ServicoId,
                    servico = servicoFuncionario.servico.servico,
                    idFuncionario = funcionario.FuncionarioId
                }
            )
            .Where(p => p.idFuncionario == id)
            .ToList();
            return Json(servicos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Servico s)
        {
            var FuncionarioId = Int32.Parse(HttpContext.Request.Form["idFuncionario"]);
            var ServicoId = Int32.Parse(HttpContext.Request.Form["idServico"]);
            var idHorario = Int32.Parse(HttpContext.Request.Form["idHorario"]);

            var servicoFuncionario = _db.servicoFuncionarios.FirstOrDefault(m => m.FuncionarioId == FuncionarioId && m.ServicoId == ServicoId);

            if (servicoFuncionario != null)
            {
                Agenda agenda = new Agenda();
                agenda.HorarioId = idHorario;
                agenda.ServicoFuncionarioId = servicoFuncionario.ServicoFuncionarioId;
                _db.Add(agenda);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.agendas == null)
            {
                return NotFound();
            }

            var isExistAgenda = _db.marcacoes.FirstOrDefault(marcacao => marcacao.AgendaId == id);

            if (isExistAgenda == null)
            {
                var agenda = await _db.agendas
                             .FirstOrDefaultAsync(m => m.AgendaId == id);

                if (_db.agendas == null)
                {
                    return Problem("Entity set 'AppDbContext.agendas'  is null.");
                }

                _db.agendas.Remove(agenda);

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
                    funcionario = funcionario,
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
            )
            .Join(
                _db.agendas,
                servicoFuncionario => servicoFuncionario.servicoFuncionario.servicoFuncionario.ServicoFuncionarioId,
                agenda => agenda.ServicoFuncionarioId,
                (servicoFuncionario, agenda) => new
                {
                    idAgenda = agenda.AgendaId,
                    nome = servicoFuncionario.servicoFuncionario.funcionario.pessoa.nome,
                    idFuncionario = servicoFuncionario.servicoFuncionario.funcionario.funcionario.FuncionarioId,
                    servico = servicoFuncionario.servico.servico,
                    idServico = servicoFuncionario.servico.ServicoId
                }
            )
            .Where(p => p.idAgenda == id)
            .ToList();

            ViewBag.funcionarios = funcionarios;
            var horarios = _db.horarios.ToList();
            ViewBag.horarios = horarios;
            ViewBag.idAgenda = id;

            //HttpContext.Response.WriteAsJsonAsync(funcionarios);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Servico servico)
        {
            var idAgenda = Int32.Parse(HttpContext.Request.Form["idAgenda"]);
            var exist = _db.agendas.Where(x => x.AgendaId == idAgenda).FirstOrDefault();

            if (exist != null)
            {
                exist.HorarioId = Int32.Parse(HttpContext.Request.Form["idHorario"]);
                _db.agendas.Update(exist);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

    }
}




using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using farmacia.Models;

namespace farmacia.Controllers
{
    public class ServicoController : Controller
    {
        private readonly AppDbContext _db;
        public ServicoController(AppDbContext db)
        {
            this._db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Servico> listaServicos = new List<Servico>();

            var servicos = await _db.servicos.Join(
                _db.tipoServicos, servico => servico.TipoServicoId,
                tipoServico => tipoServico.TipoServicoId,
                (servico, tipoServico) => new
                {
                    servicoId = servico.ServicoId,
                    servico = servico.servico,
                    tipoServico = tipoServico
                }
            ).ToListAsync();

            foreach (var serv in servicos)
            {
                Servico s = new Servico();
                s.ServicoId = serv.servicoId;
                s.servico = serv.servico;
                s.tipoServico = serv.tipoServico;
                listaServicos.Add(s);
            }
            return View(listaServicos);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var tipoServicos = _db.tipoServicos.ToList();
            return View(tipoServicos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Servico s)
        {
            Servico servico = new Servico();
            servico.servico = HttpContext.Request.Form["servico"];
            servico.TipoServicoId = Int32.Parse(HttpContext.Request.Form["tipoServico"]);

            _db.Add(servico);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.servicos == null)
            {
                return NotFound();
            }
            var servico = await _db.servicos
                .FirstOrDefaultAsync(m => m.ServicoId == id);
            if (servico == null)
            {
                return NotFound();
            }
            return View(servico);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.servicos == null)
            {
                return Problem("Entity set 'AppDbContext.clientes'  is null.");
            }
            var isExistServico = _db.servicoFuncionarios.FirstOrDefault(servico => servico.ServicoId == id);
            if (isExistServico == null)
            {
                var servico = await _db.servicos.FindAsync(id);
                if (servico != null)
                {
                    _db.servicos.Remove(servico);
                }
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
            var tipoServicos = _db.tipoServicos.ToList();

            if (id == null || _db.servicos == null)
            {
                return NotFound();
            }
            var servico = _db.servicos.Single(s => s.ServicoId == id);

            Servico serv = new Servico();
            serv.ServicoId = servico.ServicoId;
            serv.servico = servico.servico;
            serv.tipoServicos = new List<TipoServico>();

            foreach (var item in tipoServicos)
            {
                TipoServico tipo = new TipoServico();
                tipo.TipoServicoId = item.TipoServicoId;
                tipo.tipo = item.tipo;
                serv.tipoServicos.Add(tipo);
            }

            if (serv == null)
            {
                return NotFound();
            }
            return View(serv);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Servico servico)
        {
            var servicoId = Int32.Parse(HttpContext.Request.Form["ServicoId"]);
            var exist = _db.servicos.Where(x => x.ServicoId == servicoId).FirstOrDefault();

            exist.servico = HttpContext.Request.Form["servico"];
            exist.TipoServicoId = Int32.Parse(HttpContext.Request.Form["tipoServico"]);

            if (exist != null)
            {
                _db.servicos.Update(exist);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [HttpGet]
        public JsonResult getTotalServicos()
        {
            var servicos = _db.servicos.Count();
            return Json(servicos);
        }

        /* 



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _db.servicos == null)
            {
                return NotFound();
            }
            var servico = await _db.servicos.FindAsync(id);
            if (servico == null)
            {
                return NotFound();
            }
            return View(servico);
        }

            [HttpPost]
            public async Task<IActionResult> Edit(Servico servico)
            {
                var exist = _db.servicos.Where(x => x.ServicoId == servico.ServicoId).FirstOrDefault();
                if (exist != null)
                {
                    _db.servicos.Remove(exist);
                    _db.Add(servico);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            public async Task<IActionResult> Details(int? id)
            {
                if (id == null || _db.servicos == null)
                {
                    return NotFound();
                }
                var servico = await _db.servicos
                    .FirstOrDefaultAsync(m => m.ServicoId == id);
                if (servico == null)
                {
                    return NotFound();
                }
                return View(servico);
            }

            [HttpGet]
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null || _db.servicos == null)
                {
                    return NotFound();
                }
                var servico = await _db.servicos
                    .FirstOrDefaultAsync(m => m.ServicoId == id);
                if (servico == null)
                {
                    return NotFound();
                }
                return View(servico);
            }


            private bool SercicoExists(int id)
            {
                return (_db.tipoServicos?.Any(e => e.TipoServicoId == id)).GetValueOrDefault();
            }

            */

    }
}
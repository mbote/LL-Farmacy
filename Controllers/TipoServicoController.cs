using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using farmacia.Models;

namespace farmacia.Controllers
{
    public class TipoServicoController : Controller
    {
        private readonly AppDbContext _db;
        public TipoServicoController(AppDbContext db)
        {
            this._db = db;
        }
        public async Task<IActionResult> Index()
        {
            var tipoServico = await _db.tipoServicos.ToListAsync();
            return View(tipoServico);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TipoServico tipoServico)
        {
            _db.Add(tipoServico);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _db.tipoServicos == null)
            {
                return NotFound();
            }
            var tipoSev = await _db.tipoServicos.FindAsync(id);
            if (tipoSev == null)
            {
                return NotFound();
            }
            return View(tipoSev);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TipoServico tipoServico)
        {
            var exist = _db.tipoServicos.Where(x => x.TipoServicoId == tipoServico.TipoServicoId).FirstOrDefault();
            if (exist != null)
            {
                _db.tipoServicos.Remove(exist);
                _db.Add(tipoServico);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _db.tipoServicos == null)
            {
                return NotFound();
            }
            var tipoSev = await _db.tipoServicos
                .FirstOrDefaultAsync(m => m.TipoServicoId == id);
            if (tipoSev == null)
            {
                return NotFound();
            }
            return View(tipoSev);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.tipoServicos == null)
            {
                return NotFound();
            }
            var tipoServ = await _db.tipoServicos
                .FirstOrDefaultAsync(m => m.TipoServicoId == id);
            if (tipoServ == null)
            {
                return NotFound();
            }
            return View(tipoServ);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.tipoServicos == null)
            {
                return Problem("Entity set 'AppDbContext.clientes'  is null.");
            }

            var isExistServico = _db.servicos.FirstOrDefault(servico => servico.TipoServicoId == id);

            if (isExistServico == null)
            {
                var tipoServ = await _db.tipoServicos.FindAsync(id);
                if (tipoServ != null)
                {
                    _db.tipoServicos.Remove(tipoServ);
                }
                await _db.SaveChangesAsync();
            }
            else
            {
                // notoficacao
            }
            return RedirectToAction(nameof(Index));
        }
        private bool ClienteExists(int id)
        {
            return (_db.tipoServicos?.Any(e => e.TipoServicoId == id)).GetValueOrDefault();
        }

    }
}
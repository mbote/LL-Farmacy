using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using farmacia.Models;

namespace farmacia.Controllers
{
    public class TipoFuncionarioController : Controller
    {
        private readonly AppDbContext _db;
        public TipoFuncionarioController(AppDbContext db)
        {
            this._db = db;
        }
        public async Task<IActionResult> Index()
        {
            var tipoFuncionario = await _db.tipoFuncionarios.ToListAsync();
            return View(tipoFuncionario);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TipoFuncionario tipoFuncionario)
        {
            _db.Add(tipoFuncionario);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _db.tipoFuncionarios == null)
            {
                return NotFound();
            }
            var tipoFuncionario = await _db.tipoFuncionarios.FindAsync(id);
            if (tipoFuncionario == null)
            {
                return NotFound();
            }
            return View(tipoFuncionario);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TipoFuncionario tipoFuncionario)
        {
            var exist = _db.tipoFuncionarios.Where(x => x.TipoFuncionarioId == tipoFuncionario.TipoFuncionarioId).FirstOrDefault();
            if (exist != null)
            {
                _db.tipoFuncionarios.Remove(exist);
                _db.Add(tipoFuncionario);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.tipoFuncionarios == null)
            {
                return NotFound();
            }
            var tipoServ = await _db.tipoFuncionarios
                .FirstOrDefaultAsync(m => m.TipoFuncionarioId == id);
            if (tipoServ == null)
            {
                return NotFound();
            }
            return View(tipoServ);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_db.tipoFuncionarios == null)
            {
                return Problem("Entity set 'AppDbContext.clientes'  is null.");
            }
            var tipoFuncionarioId = await _db.funcionarios.FirstOrDefaultAsync(funcionario => funcionario.TipoFuncionarioId == id);

            if (tipoFuncionarioId == null)
            {
                var tipoFuncionario = await _db.tipoFuncionarios.FindAsync(id);
                if (tipoFuncionario != null)
                {
                    _db.tipoFuncionarios.Remove(tipoFuncionario);
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                //notificacao
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using farmacia.Models;

namespace farmacia.Controllers
{
    public class HorarioController : Controller
    {
        private readonly AppDbContext _context;
        public HorarioController(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index()
        {
            var horario = await _context.horarios.ToListAsync();
            return View(horario);
        }
        [HttpGet]
        public IActionResult CriarHorario()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CriarHorario([Bind("PreferenciaId, dia,hora")] Horario horario)
        {
            _context.Add(horario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> EditarHorario(int? id)
        {
            if (id == null || _context.horarios == null)
            {
                return NotFound();
            }
            var preferencia = await _context.horarios.FindAsync(id);
            if (preferencia == null)
            {
                return NotFound();
            }
            return View(preferencia);
        }
        [HttpPost]
        public async Task<IActionResult> EditarHorario(Horario horario)
        {
            try
            {
                var exist = _context.horarios.Where(x => x.HorarioId == horario.HorarioId).FirstOrDefault();

                if (exist != null)
                {
                    _context.horarios.Remove(exist);
                    _context.Add(horario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!horarioExists(horario.HorarioId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return View(horario);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.horarios == null)
            {
                return NotFound();
            }

            var horario = await _context.horarios
                .FirstOrDefaultAsync(m => m.HorarioId == id);
            if (horario == null)
            {
                return NotFound();
            }
            return View(horario);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.clientes == null)
            {
                return Problem("Entity set 'AppDbContext.preferencia'  is null.");
            }

            var isExistAgenda = _context.agendas.FirstOrDefault(a => a.HorarioId == id);

            if (isExistAgenda == null)
            {
                var preferencias = await _context.horarios.FindAsync(id);
                if (preferencias != null)
                {
                    _context.horarios.Remove(preferencias);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                //notificacao
            }

            return RedirectToAction(nameof(Index));
        }
        private bool horarioExists(int id)
        {
            return (_context.horarios?.Any(e => e.HorarioId == id)).GetValueOrDefault();
        }
    }
}


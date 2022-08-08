using System.ComponentModel.DataAnnotations.Schema;

namespace farmacia.Models{
    public class Agenda{
        public int AgendaId{get; set;}
       
        [ForeignKey("HorarioId")]
        public int HorarioId{get; set;}
        
        [ForeignKey("ServicoFuncionarioId")]
        public int ServicoFuncionarioId{get; set;}

        public ICollection<Marcacao>? marcacao {get; set;}
    }
}
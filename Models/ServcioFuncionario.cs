using System.ComponentModel.DataAnnotations.Schema;
namespace farmacia.Models{
    public class ServicoFuncionario{
        public int ServicoFuncionarioId{get; set;}
       
        [ForeignKey("FuncionarioId")]
        public int FuncionarioId{get; set;}
      
        [ForeignKey("ServicoId")]
        public int ServicoId{get; set;}

        public ICollection<Agenda>?  agenda {get; set;}
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace farmacia.Models
{
    public class TipoServico
    {
        public  int TipoServicoId { get; set; }
        public  string?  tipo { get; set; }

        public ICollection<Servico>?  Servicos { get; set; }
    }
}
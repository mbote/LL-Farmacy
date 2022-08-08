using System.ComponentModel.DataAnnotations.Schema;

namespace farmacia.Models
{
    public class Servico
    {
        public int ServicoId { get; set; }
        public string?  servico { get; set; }

        [ForeignKey("TipoServicoId")]
        public int? TipoServicoId { get; set; }

        [NotMapped]
        public TipoServico?  tipoServico { get; set; }

        [NotMapped]
        public List<TipoServico>?  tipoServicos { get; set; }

        public ICollection<ServicoFuncionario>?  servicoFuncionario { get; set; }

    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace farmacia.Models
{
    public class Marcacao
    {
        public int MarcacaoId { get; set; }

        [ForeignKey("ClienteId")]
        public int? ClienteId { get; set; }

        [ForeignKey("AgendaId")]
        public int? AgendaId { get; set; }
    }
}
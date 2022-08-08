using System.ComponentModel.DataAnnotations.Schema;
namespace farmacia.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }

        [ForeignKey("PessoaId")]
        public int PessoaId { get; set; }

        public Pessoa?  pessoa {get; set;}

        public ICollection<Marcacao>?  marcacao { get; set; }
    }
}
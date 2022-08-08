using System.ComponentModel.DataAnnotations.Schema;

namespace farmacia.Models
{
    public class Funcionario
    {
        public int FuncionarioId { get; set; }

        [ForeignKey("PessoaId")]
        public int PessoaId { get; set; }
       
        public string?  numeroOrdem { get; set; }
        public string?  nif { get; set; }

        [ForeignKey("TipoFuncionarioId")]
        public int TipoFuncionarioId { get; set; }

        public ICollection<ServicoFuncionario>?  servicoFuncionario { get; set; }
    }
}
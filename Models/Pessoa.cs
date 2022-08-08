namespace farmacia.Models
{
    public class Pessoa
    {
        public int PessoaId { get; set; }
        public string?  nome { get; set; }
        public string?  sexo { get; set; }
        public string?  bi { get; set; }
        public int telefone { get; set; }
        public string?  email { get; set; }
    
      //  public string dataNascimeto { get; set; }

        public ICollection<Cliente>?  cliente { get; set; }
        public ICollection<Funcionario>?  funcionario { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;

namespace farmacia.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options): base(options)
        {}
        public DbSet<Cliente> clientes {get; set;}
        public DbSet<TipoServico> tipoServicos {get; set;}
        public DbSet<Servico> servicos {get; set;}
        public DbSet<Horario> horarios {get; set;}
        public DbSet<Marcacao> marcacoes {get; set;}
        public DbSet<Agenda> agendas {get; set;}
        public DbSet<Pessoa> pessoas {get; set;}
        public DbSet<TipoFuncionario> tipoFuncionarios {get; set;}
        public DbSet<Funcionario> funcionarios {get; set;}
        public DbSet<ServicoFuncionario> servicoFuncionarios {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Cliente>().ToTable("Cliente");
            modelBuilder.Entity<TipoServico>().ToTable("TipoServico");
            modelBuilder.Entity<Horario>().ToTable("Horario");
            modelBuilder.Entity<Marcacao>().ToTable("Marcacao");
            modelBuilder.Entity<Agenda>().ToTable("Agenda");
            modelBuilder.Entity<Pessoa>().ToTable("Pessoa");
            modelBuilder.Entity<TipoFuncionario>().ToTable("TipoFuncionario");
            modelBuilder.Entity<Funcionario>().ToTable("Funcionario");
            modelBuilder.Entity<ServicoFuncionario>().ToTable("ServicoFuncionario");
        }
    }
}
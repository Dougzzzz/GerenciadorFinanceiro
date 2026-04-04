using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorFinanceiro.Domain.Entidades
{
    public class Categoria
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public TipoTransacao Tipo { get; private set; }

        public Categoria(string nome, TipoTransacao tipo)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Tipo = tipo;
        }
    }

    public enum TipoTransacao
    {
        Receita,
        Despesa
    }
}

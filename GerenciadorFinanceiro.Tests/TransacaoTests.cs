using GerenciadorFinanceiro.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorFinanceiro.Tests
{
    public class TransacaoTests
    {
        [Fact]
        public void Transacao_ComValorPositivo_DeveSerClassificadaComoReceita()
        {
            decimal valorPositivo = 5000m; 
            Guid categoriaId = Guid.NewGuid();
            Guid contaId = Guid.NewGuid();

            var transacao = new Transacao(DateTime.Now, "Salário", valorPositivo, categoriaId, contaId, null);

            Assert.Equal(TipoTransacao.Receita, transacao.Tipo);
        }

        [Fact]
        public void Transacao_ComValorNegativo_DeveSerClassificadaComoDespesa()
        {
            decimal valorNegativo = -150.75m;
            Guid categoriaId = Guid.NewGuid();
            Guid cartaoId = Guid.NewGuid(); 

            var transacao = new Transacao(DateTime.Now, "Mercado", valorNegativo, categoriaId, null, cartaoId);

            Assert.Equal(TipoTransacao.Despesa, transacao.Tipo);
        }
    }
}

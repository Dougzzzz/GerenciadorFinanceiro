using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Filtros;

namespace GerenciadorFinanceiro.Tests.Domain
{
    public class FiltroTransacaoTests
    {
        private readonly List<Transacao> _transacoesMock;

        public FiltroTransacaoTests()
        {
            // Criando dados mock para os testes
            var categoriaId1 = Guid.NewGuid();
            var categoriaId2 = Guid.NewGuid();
            var contaId = Guid.NewGuid();

            _transacoesMock =
            [
                new(new DateTime(2024, 1, 10), "Aluguel", -1500m, categoriaId1, contaId, null),
                new(new DateTime(2024, 1, 15), "Salário", 5000m, categoriaId2, contaId, null),
                new(new DateTime(2024, 2, 01), "Mercado", -300m, categoriaId1, contaId, null),
                new(new DateTime(2024, 2, 10), "Freelance", 1200m, categoriaId2, contaId, null),
                new(new DateTime(2024, 2, 20), "Internet", -100m, categoriaId1, contaId, null),
            ];
        }

        [Fact]
        public void Deve_Ordenar_De_Forma_Crescente_Quando_Direcao_Ascendente_For_Informada()
        {
            // Arrange
            var filtro = new FiltroTransacao { OrdenarPor = "Valor", Direcao = "Asc" };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal(-1500m, resultado.First().Valor);
            Assert.Equal(5000m, resultado.Last().Valor);
        }

        [Fact]
        public void Deve_Ordenar_De_Forma_Decrescente_Quando_Direcao_Descendente_For_Informada()
        {
            // Arrange
            var filtro = new FiltroTransacao { OrdenarPor = "Valor", Direcao = "Desc" };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal(5000m, resultado.First().Valor);
            Assert.Equal(-1500m, resultado.Last().Valor);
        }

        [Fact]
        public void Deve_Ordenar_Pela_Propriedade_Especificada_Quando_Informar_Nome_Da_Coluna()
        {
            // Arrange
            var filtro = new FiltroTransacao { OrdenarPor = "Descricao", Direcao = "Asc" };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal("Aluguel", resultado.First().Descricao);
            Assert.Equal("Salário", resultado.Last().Descricao);
        }

        [Fact]
        public void Deve_Aplicar_Ordenacao_Padrao_Quando_Nenhum_Parametro_De_Ordenacao_For_Informado()
        {
            // Arrange
            var filtro = new FiltroTransacao(); // Sem parâmetros

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert - Padrão: Data mais recente primeiro
            Assert.Equal(new DateTime(2024, 2, 20), resultado.First().Data);
        }

        [Fact]
        public void Deve_Retornar_Todas_As_Transacoes_Quando_Nenhum_Filtro_For_Aplicado()
        {
            // Arrange
            var filtro = new FiltroTransacao();

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal(5, resultado.Count);
        }

        [Fact]
        public void Deve_Filtrar_Apenas_Transacoes_A_Partir_Da_Data_Inicial_Quando_Somente_DataInicial_For_Informada()
        {
            // Arrange
            var filtro = new FiltroTransacao { DataInicial = new DateTime(2024, 2, 1) };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal(3, resultado.Count);
            Assert.All(resultado, t => Assert.True(t.Data >= filtro.DataInicial));
        }

        [Fact]
        public void Deve_Filtrar_Apenas_Transacoes_Ate_A_Data_Final_Quando_Somente_DataFinal_For_Informada()
        {
            // Arrange
            var filtro = new FiltroTransacao { DataFinal = new DateTime(2024, 1, 31) };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, t => Assert.True(t.Data <= filtro.DataFinal));
        }

        [Fact]
        public void Deve_Filtrar_Transacoes_Dentro_Do_Periodo_Quando_DataInicial_E_DataFinal_Forem_Informadas()
        {
            // Arrange
            var filtro = new FiltroTransacao
            {
                DataInicial = new DateTime(2024, 1, 15),
                DataFinal = new DateTime(2024, 2, 10),
            };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal(3, resultado.Count); // 15/01, 01/02, 10/02
        }

        [Fact]
        public void Deve_Filtrar_Transacoes_Pelo_Tipo_Quando_TipoTransacao_For_Informado()
        {
            // Arrange
            var filtro = new FiltroTransacao { Tipo = TipoTransacao.Receita };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, t => Assert.Equal(TipoTransacao.Receita, t.Tipo));
        }

        [Fact]
        public void Deve_Filtrar_Transacoes_Pela_Categoria_Quando_CategoriaId_For_Informado()
        {
            // Arrange
            var categoriaId = _transacoesMock.First().CategoriaId;
            var filtro = new FiltroTransacao { CategoriaId = categoriaId };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal(3, resultado.Count); // Mercado, Aluguel, Internet
            Assert.All(resultado, t => Assert.Equal(categoriaId, t.CategoriaId));
        }

        [Fact]
        public void Deve_Retornar_Lista_Vazia_Quando_Filtro_Nao_Corresponder_A_Nenhuma_Transacao()
        {
            // Arrange
            var filtro = new FiltroTransacao { DataInicial = new DateTime(2025, 1, 1) };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Empty(resultado);
        }

        [Fact]
        public void Deve_Aplicar_Multiplos_Filtros_Corretamente_Quando_Mais_De_Um_Criterio_For_Informado()
        {
            // Arrange
            var filtro = new FiltroTransacao
            {
                Tipo = TipoTransacao.Receita,
                DataInicial = new DateTime(2024, 2, 1),
            };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Single(resultado); // Apenas "Freelance" em Fevereiro
            Assert.Equal("Freelance", resultado.First().Descricao);
        }

        [Fact]
        public void Deve_Filtrar_E_Ordenar_Corretamente_Quando_Parametros_De_Filtro_E_Ordenacao_Forem_Informados_Juntos()
        {
            // Arrange
            var filtro = new FiltroTransacao
            {
                Tipo = TipoTransacao.Despesa,
                OrdenarPor = "Valor",
                Direcao = "Desc", // Do menos negativo (-100) para o mais negativo (-1500)
            };

            // Act
            var resultado = filtro.Aplicar(_transacoesMock.AsQueryable()).ToList();

            // Assert
            Assert.Equal(3, resultado.Count);
            Assert.Equal(-100m, resultado.First().Valor); // Maior valor (menos negativo)
            Assert.Equal(-1500m, resultado.Last().Valor); // Menor valor (mais negativo)
        }
    }
}

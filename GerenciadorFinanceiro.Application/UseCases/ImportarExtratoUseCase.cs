using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;

namespace GerenciadorFinanceiro.Application.UseCases
{
    public class ImportarExtratoUseCase
    {
        private readonly ITransacaoRepository _repository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ICartaoCreditoRepository _cartaoRepository;
        private readonly IExtratoReader _reader;

        public ImportarExtratoUseCase(
            ITransacaoRepository repository,
            ICategoriaRepository categoriaRepository,
            ICartaoCreditoRepository cartaoRepository,
            IExtratoReader reader)
        {
            _repository = repository;
            _categoriaRepository = categoriaRepository;
            _cartaoRepository = cartaoRepository;
            _reader = reader;
        }

        public async Task ExecutarAsync(Stream arquivo, Guid categoriaPadraoId, Guid? contaId, Guid? cartaoId)
        {
            var dtos = await _reader.LerArquivoAsync(arquivo);

            foreach (var dto in dtos)
            {
                Guid categoriaId = categoriaPadraoId;
                Guid? cartaoIdFinal = cartaoId;

                // Se o CSV trouxer um nome de categoria, tenta encontrar ou criar
                if (!string.IsNullOrWhiteSpace(dto.categoria))
                {
                    var tipo = dto.valor < 0 ? TipoTransacao.Despesa : TipoTransacao.Receita;
                    var categoriaExistente = await _categoriaRepository.ObterPorNomeAsync(dto.categoria, tipo);

                    if (categoriaExistente != null)
                    {
                        categoriaId = categoriaExistente.Id;
                    }
                    else
                    {
                        // Cria nova categoria automaticamente
                        var novaCategoria = new Categoria(dto.categoria, tipo);
                        await _categoriaRepository.AdicionarAsync(novaCategoria);
                        categoriaId = novaCategoria.Id;
                    }
                }

                // Se o CSV trouxer um nome de cartão, tenta encontrar ou criar (caso não tenha sido passado um cartaoId fixo)
                if (cartaoIdFinal == null && !string.IsNullOrWhiteSpace(dto.nomeCartao))
                {
                    var cartaoExistente = await _cartaoRepository.ObterPorNomeAsync(dto.nomeCartao);
                    if (cartaoExistente != null)
                    {
                        cartaoIdFinal = cartaoExistente.Id;
                    }
                    else
                    {
                        // Cria novo cartão automaticamente (valores default para limites e dias)
                        var novoCartao = new CartaoCredito(dto.nomeCartao, 0, 1, 10);
                        await _cartaoRepository.AdicionarAsync(novoCartao);
                        cartaoIdFinal = novoCartao.Id;
                    }
                }

                var transacao = new Transacao(
                    dto.data,
                    dto.descricao,
                    dto.valor,
                    categoriaId,
                    contaId,
                    cartaoIdFinal,
                    dto.categoria,
                    dto.nomeCartao,
                    dto.finalCartao,
                    dto.parcela,
                    dto.cotacao);

                await _repository.AdicionarAsync(transacao);
            }
        }
    }
}

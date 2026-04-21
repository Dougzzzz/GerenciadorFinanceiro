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
        private readonly IContaBancariaRepository _contaRepository;
        private readonly IExtratoReaderFactory _readerFactory;

        public ImportarExtratoUseCase(
            ITransacaoRepository repository,
            ICategoriaRepository categoriaRepository,
            ICartaoCreditoRepository cartaoRepository,
            IContaBancariaRepository contaRepository,
            IExtratoReaderFactory readerFactory)
        {
            _repository = repository;
            _categoriaRepository = categoriaRepository;
            _cartaoRepository = cartaoRepository;
            _contaRepository = contaRepository;
            _readerFactory = readerFactory;
        }

        public async Task ExecutarAsync(Stream arquivo, Guid? categoriaPadraoId, Guid? contaId, Guid? cartaoId)
        {
            if (contaId == null && cartaoId == null)
            {
                throw new ArgumentException("É necessário informar uma Conta Bancária ou um Cartão de Crédito para a importação.");
            }

            // Determina o provedor com base na conta ou cartão
            ProvedorExtrato provedor = ProvedorExtrato.Generico;
            if (cartaoId.HasValue)
            {
                var cartao = await _cartaoRepository.ObterPorIdAsync(cartaoId.Value);
                if (cartao != null)
                {
                    provedor = cartao.Provedor;
                }
            }
            else if (contaId.HasValue)
            {
                var conta = await _contaRepository.ObterPorIdAsync(contaId.Value);
                if (conta != null)
                {
                    provedor = conta.Provedor;
                }
            }

            IEnumerable<DTOs.TransacaoDto> dtos;
            try
            {
                var reader = _readerFactory.ObterReader(provedor, cartaoId.HasValue);
                dtos = await reader.LerArquivoAsync(arquivo);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Falha ao ler o arquivo: {ex.Message}", ex);
            }

            if (!dtos.Any())
            {
                throw new ArgumentException("Nenhuma transação válida foi encontrada no arquivo enviado.");
            }

            // Busca ou cria a categoria de fallback "Outros"
            var categoriaOutros = await _categoriaRepository.ObterPorNomeAsync("Outros", TipoTransacao.Despesa);
            if (categoriaOutros == null)
            {
                categoriaOutros = new Categoria("Outros", TipoTransacao.Despesa);
                await _categoriaRepository.AdicionarAsync(categoriaOutros);
            }

            foreach (var dto in dtos)
            {
                Guid categoriaId = categoriaPadraoId ?? categoriaOutros.Id;
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

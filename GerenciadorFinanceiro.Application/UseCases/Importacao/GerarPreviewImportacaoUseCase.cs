using GerenciadorFinanceiro.Application.DTOs.Importacao;
using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;

namespace GerenciadorFinanceiro.Application.UseCases.Importacao;

/// <summary>
/// FASE 1 da importação: Lê o CSV e retorna um preview para o utilizador revisar.
/// </summary>
public class GerarPreviewImportacaoUseCase
{
    private readonly IExtratoReaderFactory _readerFactory;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ICartaoCreditoRepository _cartaoRepository;
    private readonly IContaBancariaRepository _contaRepository;
    private readonly ICategoriaSimilaridadeService _similaridadeService;

    public GerarPreviewImportacaoUseCase(
        IExtratoReaderFactory readerFactory,
        ICategoriaRepository categoriaRepository,
        ICartaoCreditoRepository cartaoRepository,
        IContaBancariaRepository contaRepository,
        ICategoriaSimilaridadeService similaridadeService)
    {
        _readerFactory = readerFactory;
        _categoriaRepository = categoriaRepository;
        _cartaoRepository = cartaoRepository;
        _contaRepository = contaRepository;
        _similaridadeService = similaridadeService;
    }

    public async Task<ImportacaoPreviewResultadoDto> ExecutarAsync(Stream arquivoCsv, Guid? contaId, Guid? cartaoId)
    {
        // 1. Determina o provedor
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

        // 2. Faz o parsing do CSV
        var reader = _readerFactory.ObterReader(provedor);
        var linhas = await reader.LerArquivoAsync(arquivoCsv);

        // 3. Busca categorias existentes
        var categoriasExistentes = await _categoriaRepository.ObterTodasAsync();

        // 4. Gera preview com sugestões
        var previews = linhas.Select(linha =>
        {
            var sugestoes = _similaridadeService
                .BuscarSimilares(linha.descricao, categoriasExistentes)
                .ToList();

            return new TransacaoPreviewDto
            {
                Descricao = linha.descricao,
                Valor = linha.valor,
                Data = linha.data,
                CategoriaOriginalCsv = linha.categoria,
                CategoriasSugeridas = sugestoes,

                // Pré-seleciona se similaridade >= 90%
                CategoriaEscolhidaId = sugestoes
                    .FirstOrDefault(s => s.Similaridade >= 0.9)?.CategoriaId,
            };
        }).ToList();

        return new ImportacaoPreviewResultadoDto
        {
            Transacoes = previews,
            LinhasComErro = 0,
        };
    }
}

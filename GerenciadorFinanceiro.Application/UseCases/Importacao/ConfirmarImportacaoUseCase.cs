using GerenciadorFinanceiro.Application.DTOs.Importacao;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;

namespace GerenciadorFinanceiro.Application.UseCases.Importacao;

/// <summary>
/// FASE 2 da importação: Recebe as decisões do utilizador e persiste no banco.
/// </summary>
public class ConfirmarImportacaoUseCase
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmarImportacaoUseCase(
        ICategoriaRepository categoriaRepository,
        ITransacaoRepository transacaoRepository,
        IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _transacaoRepository = transacaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultadoImportacaoDto> ExecutarAsync(
        IEnumerable<TransacaoPreviewDto> transacoesConfirmadas,
        Guid? contaId,
        Guid? cartaoId)
    {
        await _unitOfWork.IniciarTransacaoAsync();

        try
        {
            var transacoesCriadas = 0;

            // Cache local para não duplicar categorias criadas no mesmo lote
            var novasCategoriasCache = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

            foreach (var preview in transacoesConfirmadas)
            {
                var categoriaId = await ResolverCategoriaAsync(preview, novasCategoriasCache);

                var transacao = new Transacao(
                    preview.Data,
                    preview.Descricao,
                    preview.Valor,
                    categoriaId,
                    contaId,
                    cartaoId,
                    preview.CategoriaOriginalCsv ?? string.Empty,
                    string.Empty, // nomeCartao
                    string.Empty, // finalCartao
                    string.Empty, // parcela
                    1m);         // cotacao

                await _transacaoRepository.AdicionarAsync(transacao);
                transacoesCriadas++;
            }

            await _unitOfWork.CommitAsync();

            return new ResultadoImportacaoDto
            {
                Sucesso = true,
                TotalImportado = transacoesCriadas,
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            return new ResultadoImportacaoDto
            {
                Sucesso = false,
                MensagemErro = $"Erro ao persistir importação: {ex.Message}",
                TotalImportado = 0,
            };
        }
    }

    private async Task<Guid> ResolverCategoriaAsync(TransacaoPreviewDto preview, Dictionary<string, Guid> cache)
    {
        // 1. Utilizador escolheu uma categoria existente
        if (preview.CategoriaEscolhidaId.HasValue)
        {
            return preview.CategoriaEscolhidaId.Value;
        }

        // 2. Utilizador quer criar uma nova categoria
        var nomeNova = preview.NovaCategoriaPersonalizada
                       ?? preview.CategoriaOriginalCsv
                       ?? "Outros";

        // Verifica no cache se já criamos esta categoria neste lote
        if (cache.TryGetValue(nomeNova, out var idExistenteNoLote))
        {
            return idExistenteNoLote;
        }

        var tipo = preview.Valor < 0 ? TipoTransacao.Despesa : TipoTransacao.Receita;

        // Verifica no banco (segurança extra)
        var existenteNoBanco = await _categoriaRepository.ObterPorNomeAsync(nomeNova, tipo);
        if (existenteNoBanco != null)
        {
            cache[nomeNova] = existenteNoBanco.Id;
            return existenteNoBanco.Id;
        }

        var novaCategoria = new Categoria(nomeNova, tipo);
        await _categoriaRepository.AdicionarAsync(novaCategoria);

        cache[nomeNova] = novaCategoria.Id;
        return novaCategoria.Id;
    }
}

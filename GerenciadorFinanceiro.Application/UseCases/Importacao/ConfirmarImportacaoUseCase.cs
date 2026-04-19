using GerenciadorFinanceiro.Application.DTOs.Importacao;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;

namespace GerenciadorFinanceiro.Application.UseCases.Importacao
{
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
                var transacoesIgnoradas = 0;

                // Cache local para não duplicar categorias criadas no mesmo lote
                var novasCategoriasCache = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

                foreach (var preview in transacoesConfirmadas)
                {
                    // 1. Instancia transação temporária para gerar o Hash e verificar duplicidade
                    var transacao = new Transacao(
                        preview.Data,
                        preview.Descricao,
                        preview.Valor,
                        Guid.Empty, // Categoria temporária
                        contaId,
                        cartaoId,
                        preview.CategoriaOriginalCsv ?? string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        1m);

                    // 2. Verifica se esta transação já existe no sistema (Prevenção de Duplicados)
                    if (await _transacaoRepository.ExisteChaveExclusivaAsync(transacao.ChaveExclusiva))
                    {
                        transacoesIgnoradas++;
                        continue;
                    }

                    // 3. Resolve a categoria real (existente ou cria nova)
                    var categoriaId = await ResolverCategoriaAsync(preview, novasCategoriasCache);
                    
                    // 4. Atualiza a transação com a categoria correta antes de salvar
                    transacao.Atualizar(
                        transacao.Data,
                        transacao.Descricao,
                        transacao.Valor,
                        categoriaId,
                        transacao.ContaBancariaId,
                        transacao.CartaoCreditoId,
                        transacao.Categoria,
                        transacao.NomeCartao,
                        transacao.FinalCartao,
                        transacao.Parcela,
                        transacao.Cotacao);

                    await _transacaoRepository.AdicionarAsync(transacao);
                    transacoesCriadas++;
                }

                await _unitOfWork.CommitAsync();

                return new ResultadoImportacaoDto
                {
                    Sucesso = true,
                    TotalImportado = transacoesCriadas,
                    TotalIgnorado = transacoesIgnoradas
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
                    TotalIgnorado = 0
                };
            }
        }

        private async Task<Guid> ResolverCategoriaAsync(TransacaoPreviewDto preview, Dictionary<string, Guid> cache)
        {
            if (preview.CategoriaEscolhidaId.HasValue)
            {
                return preview.CategoriaEscolhidaId.Value;
            }

            var nomeNova = preview.NovaCategoriaPersonalizada
                           ?? preview.CategoriaOriginalCsv
                           ?? "Outros";

            if (cache.TryGetValue(nomeNova, out var idExistenteNoLote))
            {
                return idExistenteNoLote;
            }

            var tipo = preview.Valor < 0 ? TipoTransacao.Despesa : TipoTransacao.Receita;

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
}

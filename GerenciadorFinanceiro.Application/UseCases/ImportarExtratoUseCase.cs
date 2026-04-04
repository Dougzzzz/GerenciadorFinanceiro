using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;

namespace GerenciadorFinanceiro.Application.UseCases
{
    public class ImportarExtratoUseCase
    {
        private readonly ITransacaoRepository _repository;
        private readonly IExtratoReader _reader;

        public ImportarExtratoUseCase(ITransacaoRepository repository, IExtratoReader reader)
        {
            _repository = repository;
            _reader = reader;
        }

        public async Task ExecutarAsync(Stream arquivoExtrato, Guid categoriaPadraoId, Guid? contaId = null, Guid? cartaoId = null)
        {
            var transacoesDto = await _reader.LerArquivoAsync(arquivoExtrato);

            foreach (var dto in transacoesDto)
            {
                var transacao = new Transacao(
                    dto.Data,
                    dto.Descricao,
                    dto.Valor,
                    categoriaPadraoId,
                    contaId,
                    cartaoId,
                    dto.Categoria,
                    dto.NomeCartao,
                    dto.FinalCartao,
                    dto.Parcela,
                    dto.Cotacao
                );

                await _repository.AdicionarAsync(transacao);
            }
        }
    }
}

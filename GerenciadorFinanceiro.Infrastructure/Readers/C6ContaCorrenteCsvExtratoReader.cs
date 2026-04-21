using System.Globalization;
using System.Text;
using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Application.Interfaces;

namespace GerenciadorFinanceiro.Infrastructure.Readers
{
    public class C6ContaCorrenteCsvExtratoReader : IExtratoReader
    {
        public async Task<IEnumerable<TransacaoDto>> LerArquivoAsync(Stream arquivo)
        {
            var transacoes = new List<TransacaoDto>();
            using var reader = new StreamReader(arquivo, Encoding.UTF8, true);

            string? linha;
            bool cabecalhoEncontrado = false;
            int idxData = -1, idxTitulo = -1, idxDescricao = -1, idxEntrada = -1, idxSaida = -1;

            while ((linha = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(linha))
                {
                    continue;
                }

                if (!cabecalhoEncontrado && linha.Contains("Data Lançamento") && (linha.Contains("Entrada") || linha.Contains("Saída")))
                {
                    var colunasHeader = linha.Split(',');
                    idxData = Array.IndexOf(colunasHeader, "Data Lançamento");
                    idxTitulo = Array.IndexOf(colunasHeader, "Título");
                    idxDescricao = Array.IndexOf(colunasHeader, "Descrição");
                    idxEntrada = Array.IndexOf(colunasHeader, "Entrada(R$)");
                    idxSaida = Array.IndexOf(colunasHeader, "Saída(R$)");
                    cabecalhoEncontrado = true;
                    continue;
                }

                if (!cabecalhoEncontrado)
                {
                    continue;
                }

                var colunas = linha.Split(',');
                if (colunas.Length <= Math.Max(idxData, Math.Max(idxEntrada, idxSaida)))
                {
                    continue;
                }

                if (!DateTime.TryParse(colunas[idxData], new CultureInfo("pt-BR"), DateTimeStyles.None, out var data))
                {
                    continue;
                }

                data = DateTime.SpecifyKind(data, DateTimeKind.Utc);

                // No C6, o campo Título as vezes é melhor que a Descrição (ex: "Pix recebido de...")
                var titulo = idxTitulo >= 0 ? colunas[idxTitulo].Trim() : string.Empty;
                var desc = idxDescricao >= 0 ? colunas[idxDescricao].Trim() : string.Empty;
                var descricaoFinal = !string.IsNullOrEmpty(titulo) ? titulo : desc;

                // Tenta parse com ponto e depois com vírgula para ser resiliente
                static decimal ParseValor(string text)
                {
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        return 0;
                    }

                    if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var v1))
                    {
                        return v1;
                    }

                    if (decimal.TryParse(text, NumberStyles.Any, new CultureInfo("pt-BR"), out var v2))
                    {
                        return v2;
                    }

                    return 0;
                }

                decimal entrada = ParseValor(colunas[idxEntrada]);
                decimal saida = ParseValor(colunas[idxSaida]);

                decimal valorFinal = 0;
                if (entrada > 0)
                {
                    valorFinal = entrada;
                }
                else if (saida > 0)
                {
                    valorFinal = -saida;
                }
                else
                {
                    continue;
                }

                transacoes.Add(new TransacaoDto(data, descricaoFinal, valorFinal));
            }

            return transacoes;
        }
    }
}

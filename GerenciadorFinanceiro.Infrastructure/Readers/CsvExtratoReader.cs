using System.Globalization;
using System.Text;
using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Application.Interfaces;

namespace GerenciadorFinanceiro.Infrastructure.Readers
{
    public class CsvExtratoReader : IExtratoReader
    {
        public async Task<IEnumerable<TransacaoDto>> LerArquivoAsync(Stream arquivo)
        {
            var transacoes = new List<TransacaoDto>();

            // Usa UTF8 com detecção automática de BOM para evitar caracteres estranhos como na imagem
            using var reader = new StreamReader(arquivo, Encoding.UTF8, true);

            // Lê o cabeçalho e identifica índices das colunas relevantes
            var headerLine = await reader.ReadLineAsync();
            if (headerLine == null)
            {
                return transacoes;
            }

            // Não usar ',' como separador para não quebrar valores decimais com vírgula
            var headers = headerLine.Split([';', '\t'], StringSplitOptions.None)
                .Select(h => RemoveDiacritics(h).Trim().ToLowerInvariant())
                .ToArray();

            int idxData = Array.FindIndex(headers, h => h.Contains("data"));
            int idxDescricao = Array.FindIndex(headers, h => h.Contains("descri"));
            int idxValorReais = Array.FindIndex(headers, h => h.Contains("valor") && h.Contains("r$"));
            int idxValorDolar = Array.FindIndex(headers, h => h.Contains("us$") || h.Contains("us"));
            int idxCategoria = Array.FindIndex(headers, h => h.Contains("categoria"));
            int idxNomeCartao = Array.FindIndex(headers, h => h.Contains("nome"));
            int idxFinalCartao = Array.FindIndex(headers, h => h.Contains("final"));
            int idxParcela = Array.FindIndex(headers, h => h.Contains("parcela"));
            int idxCotacao = Array.FindIndex(headers, h => h.Contains("cota"));

            var cultureBr = new CultureInfo("pt-BR");

            while (!reader.EndOfStream)
            {
                var linha = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(linha))
                {
                    continue;
                }

                var colunas = linha.Split([';', '\t']);
                string Obter(int idx) => (idx >= 0 && idx < colunas.Length) ? colunas[idx].Trim() : string.Empty;

                var dataText = Obter(idxData);
                bool dataValida = DateTime.TryParse(dataText, cultureBr, DateTimeStyles.None, out DateTime data);

                if (dataValida)
                {
                    data = DateTime.SpecifyKind(data, DateTimeKind.Utc);
                }

                var descricao = Obter(idxDescricao);

                var valorReaisText = Obter(idxValorReais);
                bool valorValido = TryParseDecimalFlex(valorReaisText, out decimal valor);

                if (!dataValida || !valorValido)
                {
                    continue;
                }

                var categoria = Obter(idxCategoria);
                var nomeCartao = Obter(idxNomeCartao);
                var finalCartao = Obter(idxFinalCartao);
                var parcela = Obter(idxParcela);

                TryParseDecimalFlex(Obter(idxCotacao), out decimal cotacao);

                transacoes.Add(new TransacaoDto(data, descricao, valor, categoria, nomeCartao, finalCartao, parcela, cotacao));
            }

            return transacoes;
        }

        private static bool TryParseDecimalFlex(string text, out decimal result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;

            // Limpa símbolos monetários e espaços
            var cleaned = text.Replace("R$", "").Replace("$", "").Replace(" ", "").Trim();

            // Lógica Flexível:
            // 1. Se tem os dois (ex: 1.234,56 ou 1,234.56), o último é o decimal
            if (cleaned.Contains('.') && cleaned.Contains(','))
            {
                if (cleaned.LastIndexOf(',') > cleaned.LastIndexOf('.'))
                {
                    // Padrão BR: 1.234,56 -> remove ponto, troca vírgula por ponto
                    cleaned = cleaned.Replace(".", "").Replace(',', '.');
                }
                else
                {
                    // Padrão US: 1,234.56 -> remove vírgula
                    cleaned = cleaned.Replace(",", "");
                }
            }
            else if (cleaned.Contains(','))
            {
                // Só tem vírgula: trata como decimal (ex: 1234,56)
                cleaned = cleaned.Replace(',', '.');
            }
            // Se só tem ponto, mantém (ex: 1234.56)

            return decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}

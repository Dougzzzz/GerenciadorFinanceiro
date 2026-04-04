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

            using var reader = new StreamReader(arquivo, Encoding.Default);

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

            var culture = new CultureInfo("pt-BR");

            while (!reader.EndOfStream)
            {
                var linha = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(linha))
                {
                    continue;
                }

                // Suporta CSV com ; ou \t (não usamos ',' pois pode aparecer em valores)
                var colunas = linha.Split([';', '\t']);

                // Função local para obter coluna segura
                string Obter(int idx) => (idx >= 0 && idx < colunas.Length) ? colunas[idx].Trim() : string.Empty;

                // Lê data
                var dataText = Obter(idxData);
                bool dataValida = DateTime.TryParse(dataText, culture, DateTimeStyles.None, out DateTime data);
                
                // PostgreSQL exige DateTime em UTC para colunas 'timestamp with time zone'
                if (dataValida)
                {
                    data = DateTime.SpecifyKind(data, DateTimeKind.Utc);
                }

                // Lê descrição
                var descricao = Obter(idxDescricao);

                // Ignora coluna em US$ — tenta pegar valor em R$
                decimal valor = 0m;
                var valorReaisText = Obter(idxValorReais);
                var valorDolarText = Obter(idxValorDolar);

                bool valorValido = false;
                if (!string.IsNullOrWhiteSpace(valorReaisText))
                {
                    // Remove possíveis símbolos e espaços e pontos de milhar
                    var cleaned = valorReaisText.Replace("R$", string.Empty).Replace(" ", string.Empty).Trim();
                    cleaned = cleaned.Replace(".", string.Empty); // remove milhares
                    valorValido = decimal.TryParse(cleaned, NumberStyles.Any, culture, out valor);
                }

                if (!valorValido && !string.IsNullOrWhiteSpace(valorDolarText))
                {
                    // Caso não tenha valor em reais, tenta ler o valor em dólar (mas conforme solicitado, será ignorado)
                    // Apenas tentamos parse para evitar erros, mas não usamos o valor em dólar
                    var cleaned = valorDolarText.Replace("$", string.Empty).Replace(" ", string.Empty).Trim();
                    decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
                }

                if (!dataValida || !valorValido)
                {
                    // Se não conseguiu interpretar data ou valor em reais, pula a linha
                    continue;
                }

                // Lê campos opcionais
                var categoria = Obter(idxCategoria);
                var nomeCartao = Obter(idxNomeCartao);
                var finalCartao = Obter(idxFinalCartao);
                var parcela = Obter(idxParcela);
                decimal cotacao = 0m;
                var cotacaoText = Obter(idxCotacao);
                if (!string.IsNullOrWhiteSpace(cotacaoText))
                {
                    var cleaned = cotacaoText.Replace("R$", string.Empty).Replace(" ", string.Empty).Trim();
                    cleaned = cleaned.Replace(".", string.Empty);
                    decimal.TryParse(cleaned, NumberStyles.Any, culture, out cotacao);
                }

                transacoes.Add(new TransacaoDto(data, descricao, valor, categoria, nomeCartao, finalCartao, parcela, cotacao));
            }

            return transacoes;
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

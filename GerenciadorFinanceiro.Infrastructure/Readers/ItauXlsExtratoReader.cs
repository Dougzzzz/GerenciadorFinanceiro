using System.Globalization;
using System.Text;
using ExcelDataReader;
using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Application.Interfaces;

namespace GerenciadorFinanceiro.Infrastructure.Readers
{
    /// <summary>
    /// Leitor de extratos bancários do Itaú em formato .xls (Excel).
    /// </summary>
    public class ItauXlsExtratoReader : IExtratoReader
    {
        public ItauXlsExtratoReader()
        {
            // Necessário para o ExcelDataReader lidar com encodings antigos/Windows
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<IEnumerable<TransacaoDto>> LerArquivoAsync(Stream arquivo)
        {
            var transacoes = new List<TransacaoDto>();

            // O ExcelDataReader não é nativamente assíncrono para XLS antigo, 
            // mas rodamos em uma Task para não bloquear a thread se necessário.
            return await Task.Run(() =>
            {
                IExcelDataReader reader;
                try
                {
                    reader = ExcelReaderFactory.CreateReader(arquivo);
                }
                catch (ExcelDataReader.Exceptions.HeaderException)
                {
                    // Fallback para CSV caso seja um XLS "falso" do Itaú (que na verdade é um CSV/TSV)
                    // Reposiciona o stream para o início para tentar ler como CSV
                    arquivo.Position = 0;
                    var config = new ExcelReaderConfiguration
                    {
                        FallbackEncoding = Encoding.GetEncoding("Windows-1252"),
                    };
                    reader = ExcelReaderFactory.CreateCsvReader(arquivo, config);
                }

                using (reader)
                {
                    // Pular as primeiras 10 linhas (cabeçalho e lixo visual do Itaú)
                    for (int i = 0; i < 10; i++)
                    {
                        if (!reader.Read())
                        {
                            break;
                        }
                    }

                    while (reader.Read())
                    {
                        var dataStr = reader.GetValue(0)?.ToString();
                        var descricao = reader.GetValue(1)?.ToString();
                        var valorObj = reader.GetValue(3);

                        if (string.IsNullOrWhiteSpace(dataStr) || string.IsNullOrWhiteSpace(descricao))
                        {
                            continue;
                        }

                        // Filtrar linhas de saldo que não são transações
                        if (descricao.Equals("SALDO ANTERIOR", StringComparison.OrdinalIgnoreCase) ||
                            descricao.Contains("SALDO TOTAL DISPON", StringComparison.OrdinalIgnoreCase) ||
                            descricao.Contains("S A L D O", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (!DateTime.TryParseExact(dataStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
                        {
                            continue;
                        }

                        // Garante que a data é UTC para o banco de dados
                        data = DateTime.SpecifyKind(data, DateTimeKind.Utc);

                        decimal valor;
                        try
                        {
                            // O ExcelDataReader pode retornar o valor como double ou string formatada
                            // O Itaú costuma trazer valores negativos para saídas e positivos para entradas no XLS.
                            valor = Convert.ToDecimal(valorObj, CultureInfo.GetCultureInfo("pt-BR"));
                        }
                        catch
                        {
                            continue;
                        }

                        // No Itaú (XLS), o sinal já vem correto: Saída = Negativo, Entrada = Positivo.
                        // Isso alinha perfeitamente com a regra de negócio do sistema.
                        transacoes.Add(new TransacaoDto(data, descricao.Trim(), valor));
                    }
                }

                return transacoes;
            });
        }
    }
}

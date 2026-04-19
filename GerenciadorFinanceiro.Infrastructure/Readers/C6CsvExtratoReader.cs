namespace GerenciadorFinanceiro.Infrastructure.Readers
{
    /// <summary>
    /// Leitor específico para o C6 Bank.
    /// No C6, compras/gastos aparecem como valores POSITIVOS e pagamentos/estornos como NEGATIVOS.
    /// </summary>
    public class C6CsvExtratoReader : CsvExtratoReader
    {
        protected override decimal ProcessarValor(decimal valor) =>

            // O sistema exige: Gastos = Negativo, Entradas = Positivo.
            // CSV C6: Compras = 84,90 (Positivo), Pagamento = -27,77 (Negativo).
            // Invertemos o sinal para alinhar com o padrão do sistema.
            -valor;
    }
}

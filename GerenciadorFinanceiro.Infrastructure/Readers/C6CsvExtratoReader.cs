namespace GerenciadorFinanceiro.Infrastructure.Readers
{
    public class C6CsvExtratoReader : CsvExtratoReader
    {
        protected override decimal ProcessarValor(decimal valor) =>

            // No C6 Bank, compras aparecem como positivas e pagamentos/estornos como negativas.
            // O sistema exige o oposto: Gastos (Saídas) = Negativo, Entradas = Positivo.
            -valor;
    }
}

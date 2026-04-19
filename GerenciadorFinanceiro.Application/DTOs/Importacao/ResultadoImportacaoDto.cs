namespace GerenciadorFinanceiro.Application.DTOs.Importacao
{
    /// <summary>
    /// Resultado da persistência da importação.
    /// </summary>
    public class ResultadoImportacaoDto
    {
        /// <summary>Gets a value indicating whether the import was successful.</summary>
        public bool Sucesso { get; init; }

        /// <summary>Gets the total number of transactions imported.</summary>
        public int TotalImportado { get; init; }

        /// <summary>Gets the total number of duplicate transactions ignored.</summary>
        public int TotalIgnorado { get; init; }

        /// <summary>Gets the error message if the import failed.</summary>
        public string? MensagemErro { get; init; }
    }
}

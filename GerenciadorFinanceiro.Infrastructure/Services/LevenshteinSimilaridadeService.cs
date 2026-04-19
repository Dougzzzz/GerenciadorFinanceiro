using GerenciadorFinanceiro.Application.DTOs.Importacao;
using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Entidades;
using System.Text;
using System.Text.RegularExpressions;

namespace GerenciadorFinanceiro.Infrastructure.Services
{
    /// <summary>
    /// Implementação do serviço de similaridade usando o algoritmo de Distância de Levenshtein.
    /// </summary>
    public class LevenshteinSimilaridadeService : ICategoriaSimilaridadeService
    {
        public IEnumerable<CategoriaSugestaoDto> BuscarSimilares(
            string descricao,
            IEnumerable<Categoria> categorias,
            double limiarMinimo = 0.5)
        {
            var descricaoNormalizada = Normalizar(descricao);

            return categorias
                .Select(categoria =>
                {
                    var nomeNormalizado = Normalizar(categoria.Nome);
                    var similaridade = CalcularSimilaridade(descricaoNormalizada, nomeNormalizado);

                    return new CategoriaSugestaoDto
                    {
                        CategoriaId = categoria.Id,
                        NomeCategoria = categoria.Nome,
                        Similaridade = similaridade,
                    };
                })
                .Where(s => s.Similaridade >= limiarMinimo)
                .OrderByDescending(s => s.Similaridade)
                .Take(3);
        }

        private static double CalcularSimilaridade(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return 1.0;
            }

            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            {
                return 0.0;
            }

            var distancia = CalcularDistanciaLevenshtein(a, b);
            var tamanhoDoPadraoMaior = Math.Max(a.Length, b.Length);

            return 1.0 - ((double)distancia / tamanhoDoPadraoMaior);
        }

        private static int CalcularDistanciaLevenshtein(string a, string b)
        {
            var matriz = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++)
            {
                matriz[i, 0] = i;
            }

            for (int j = 0; j <= b.Length; j++)
            {
                matriz[0, j] = j;
            }

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    var custo = a[i - 1] == b[j - 1] ? 0 : 1;

                    matriz[i, j] = Math.Min(
                        Math.Min(
                            matriz[i - 1, j] + 1,
                            matriz[i, j - 1] + 1),
                        matriz[i - 1, j - 1] + custo);
                }
            }

            return matriz[a.Length, b.Length];
        }

        private static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            var resultado = texto.ToLowerInvariant().Trim();

            // Remove sufixos comuns de empresas
            resultado = Regex.Replace(resultado, @"\b(s/a|ltda|eireli|me|epp|sa)\b", string.Empty, RegexOptions.IgnoreCase);

            // Remove caracteres não-alfanuméricos
            resultado = Regex.Replace(resultado, @"[^a-z0-9\s]", " ");

            // Remove espaços múltiplos
            resultado = Regex.Replace(resultado, @"\s+", " ").Trim();

            return RemoverAcentos(resultado);
        }

        private static string RemoverAcentos(string texto)
        {
            var normalizado = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalizado)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                    != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}

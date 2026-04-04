using GerenciadorFinanceiro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorFinanceiro.Application.Interfaces
{
    public interface IExtratoReader
    {
        Task<IEnumerable<TransacaoDto>> LerArquivoAsync(Stream arquivo);
    }
}

using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Interfaces;
using GerenciadorFinanceiro.Infrastructure.Data;
using GerenciadorFinanceiro.Infrastructure.Readers;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using GerenciadorFinanceiro.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorFinanceiro.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITransacaoRepository, TransacaoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IContaBancariaRepository, ContaBancariaRepository>();
            services.AddScoped<ICartaoCreditoRepository, CartaoCreditoRepository>();
            services.AddScoped<IMetaGastoRepository, MetaGastoRepository>();
            services.AddScoped<IExtratoReaderFactory, ExtratoReaderFactory>();
            services.AddScoped<ICategoriaSimilaridadeService, LevenshteinSimilaridadeService>();

            return services;
        }
    }
}

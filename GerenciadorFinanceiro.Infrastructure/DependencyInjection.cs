using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Interfaces;
using GerenciadorFinanceiro.Infrastructure.Readers;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorFinanceiro.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ITransacaoRepository, TransacaoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IContaBancariaRepository, ContaBancariaRepository>();
            services.AddScoped<ICartaoCreditoRepository, CartaoCreditoRepository>();

            services.AddScoped<IExtratoReaderFactory, ExtratoReaderFactory>();

            return services;
        }
    }
}

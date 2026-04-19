using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Application.UseCases.Importacao;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorFinanceiro.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ImportarExtratoUseCase>();
            services.AddScoped<GerarPreviewImportacaoUseCase>();
            services.AddScoped<ConfirmarImportacaoUseCase>();
            services.AddScoped<ObterResumoMensalUseCase>();
            services.AddScoped<IValidarMetaGastoUseCase, ValidarMetaGastoUseCase>();

            return services;
        }
    }
}

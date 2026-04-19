# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["GerenciadorFinanceiro.Api/GerenciadorFinanceiro.Api.csproj", "GerenciadorFinanceiro.Api/"]
COPY ["GerenciadorFinanceiro.Application/GerenciadorFinanceiro.Application.csproj", "GerenciadorFinanceiro.Application/"]
COPY ["GerenciadorFinanceiro.Infrastructure/GerenciadorFinanceiro.Infrastructure.csproj", "GerenciadorFinanceiro.Infrastructure/"]
COPY ["GerenciadorFinanceiro.Domain/GerenciadorFinanceiro.Domain.csproj", "GerenciadorFinanceiro.Domain/"]

RUN dotnet restore "GerenciadorFinanceiro.Api/GerenciadorFinanceiro.Api.csproj"

# Copiar o restante do código e compilar
COPY . .
WORKDIR "/src/GerenciadorFinanceiro.Api"
RUN dotnet build "GerenciadorFinanceiro.Api.csproj" -c Release -o /app/build

# Publicar a aplicação
FROM build AS publish
RUN dotnet publish "GerenciadorFinanceiro.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GerenciadorFinanceiro.Api.dll"]

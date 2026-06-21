# 1. Aşama: Build (Derleme) ortamı
# Projen .NET 9 hedefine sahip olduğu için resmi .NET 9 SDK imajını kullanıyoruz.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Docker Cache Optimizasyonu: Önce SADECE solution ve proje dosyalarını kopyalıyoruz.
# Kodlar değişse bile bağımlılıklar değişmediği sürece 'dotnet restore' adımı cache'den hızlıca geçilir.
COPY ["src/OleAI.Backend.sln", "src/"]
COPY ["src/OleAI.Backend.Core/OleAI.Backend.Core.csproj", "src/OleAI.Backend.Core/"]
COPY ["src/OleAI.Backend.Domain/OleAI.Backend.Domain.csproj", "src/OleAI.Backend.Domain/"]
COPY ["src/OleAI.Backend.Persistence/OleAI.Backend.Persistence.csproj", "src/OleAI.Backend.Persistence/"]
COPY ["src/OleAI.Backend.Application/OleAI.Backend.Application.csproj", "src/OleAI.Backend.Application/"]
COPY ["src/OleAI.Backend.Presentation/OleAI.Backend.Presentation.csproj", "src/OleAI.Backend.Presentation/"]

# Bağımlılıkları yükle (Sadece uygulamanın çalışması için gereken projeler)
RUN dotnet restore "src/OleAI.Backend.Presentation/OleAI.Backend.Presentation.csproj"

# Şimdi asıl kaynak kodların tamamını kopyala
COPY . .

# Projeyi derle ve publish al (Çalışmaya hazır dosyaları /app/publish dizinine çıkar)
WORKDIR "/app/src/OleAI.Backend.Presentation"
RUN dotnet publish "OleAI.Backend.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Aşama: Runtime (Çalışma) ortamı
# Sunucuya SDK yüklemek gereksiz boyut ve güvenlik açığı yaratır. Sadece hafif Runtime imajını kullanıyoruz.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Güvenlik için root yetkilerini bırakıp kısıtlı "app" kullanıcısına (UID 1654) geçiyoruz.
USER $APP_UID
# Build aşamasında ürettiğimiz publish dosyalarını bu hafif imaja kopyala
COPY --from=build /app/publish .

# Container Apps & Docker Compose Health Check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "OleAI.Backend.Presentation.dll"]
# Temel imaj olarak .NET 8 SDK imajını kullanıyoruz
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

# Gerekli araçları yükleyelim
RUN apt-get update && apt-get install -y \
    git \
    curl \
    unzip \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Kullanıcı oluştur ve gerekli izinleri ayarla
# RUN useradd -m -d /home/devcontainer -s /bin/bash devcontainer \
#     && chown -R devcontainer:devcontainer /home/devcontainer

# USER devcontainer

# Çalışma dizinini ayarla
WORKDIR /workspace

# .NET CLI'yı doğrula
RUN dotnet --info

# Uygulama geliştirme için default cmd
CMD ["/bin/bash"]

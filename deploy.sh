#!/bin/bash
set -e

echo "🔨 Build kontrol ediliyor..."
dotnet build SiparisUygulama/SiparisUygulama.csproj -c Release --no-restore -q
if [ $? -ne 0 ]; then
  echo "❌ Build hatası, deploy iptal."
  exit 1
fi

echo "🐳 Docker image build ediliyor (linux/amd64)..."
docker buildx build --platform linux/amd64 \
  -t siparisregistry.azurecr.io/web:latest \
  ./SiparisUygulama --push -q

echo "🔄 Azure App Service yeniden başlatılıyor..."
az webapp restart --name siparis-web-yg --resource-group siparis-rg

echo ""
echo "✅ Deploy tamamlandı!"
echo "🌐 https://siparis-web-yg.azurewebsites.net"

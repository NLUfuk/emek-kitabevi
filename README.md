# EMEK-KİTABEVİ

Docker containerize edilmiş, .NET Core backend ve React frontend ile geliştirilmiş kitap yönetim sistemi.

## 🚀 Özellikler

- ✅ Kullanıcı girişi ve yetkilendirme (JWT)
- ✅ Kitap yönetimi (CRUD işlemleri)
- ✅ ISBN, Barcode, Title ile arama
- ✅ Fiyat güncelleme ve geçmiş takibi
- ✅ Stok yönetimi (ekleme/çıkarma)
- ✅ Satış/Alış/İade işlemleri
- ✅ Gelir/Gider raporları
- ✅ Satış raporları ve analizler
- ✅ Detaylı loglama ve audit trail

## 🛠️ Teknoloji Stack

**Backend:**
- .NET 9.0 (ASP.NET Core Web API)
- Entity Framework Core 9.0
- SQL Server 2022
- JWT Authentication
- Swagger/OpenAPI

**Frontend:**
- React 18 + TypeScript
- Material-UI v5
- React Router v6
- Axios
- Recharts

**Infrastructure:**
- Docker & Docker Compose

## 📋 Gereksinimler

- .NET 9.0 SDK
- Node.js 20+
- Docker Desktop (veya Docker Engine)
- SQL Server Management Studio (SSMS) - opsiyonel

## 🚀 Hızlı Başlangıç

### 1. Repository'yi klonlayın

```bash
git clone <repository-url>
cd emek-kitabevi
```

### 2. Docker ile çalıştırma

```bash
cd docker
docker-compose up -d
```

Bu komut şunları başlatır:
- SQL Server (port 1433)
- Backend API (port 5000)
- Frontend (port 3000)

### 3. Veritabanı Migration

```bash
cd src/backend
dotnet ef database update --project EmekKitabevi.Infrastructure --startup-project EmekKitabevi.API
```

### 4. Erişim

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433

## 🔧 Geliştirme

### Backend Geliştirme

```bash
cd src/backend
dotnet restore
dotnet build
dotnet run --project EmekKitabevi.API
```

### Frontend Geliştirme

```bash
cd src/frontend
npm install
npm run dev
```

### Migration Oluşturma

```bash
cd src/backend
dotnet ef migrations add MigrationName --project EmekKitabevi.Infrastructure --startup-project EmekKitabevi.API
```

## 📁 Proje Yapısı

```
emek-kitabevi/
├── src/
│   ├── backend/
│   │   ├── EmekKitabevi.API/          # Web API
│   │   ├── EmekKitabevi.Application/ # Business logic
│   │   ├── EmekKitabevi.Domain/       # Domain entities
│   │   └── EmekKitabevi.Infrastructure/ # Data access
│   └── frontend/                      # React app
├── docker/                            # Docker configs
├── scripts/                           # Utility scripts
└── PROJECT_PLAN.md                    # Detaylı plan
```

## 🔐 Varsayılan Kullanıcı

İlk kurulumda admin kullanıcısı oluşturulacak (Faz 2'de eklenecek).

## 📝 API Dokümantasyonu

Swagger UI üzerinden tüm API endpoint'lerini görüntüleyebilirsiniz:
http://localhost:5000/swagger

## 🐳 Docker Komutları

```bash
# Servisleri başlat
docker-compose up -d

# Servisleri durdur
docker-compose down

# Logları görüntüle
docker-compose logs -f

# Servisleri yeniden başlat
docker-compose restart
```

## 📄 Lisans

Bu proje özel bir projedir.

## 👥 Katkıda Bulunma

Proje geliştirme aşamasındadır. Detaylı plan için `PROJECT_PLAN.md` dosyasına bakın.

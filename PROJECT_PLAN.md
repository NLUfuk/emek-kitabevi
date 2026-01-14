# EMEK-KİTABEVİ - PROJE PLANI

## 📋 PROJE ÖZETİ

Docker containerize edilmiş, .NET Core backend ve React frontend ile geliştirilmiş bir kitap yönetim sistemi. SQL Server veritabanı, JWT authentication, Swagger API dokümantasyonu ve kapsamlı raporlama özellikleri içerir.

---

## 🏗️ MİMARİ YAPISI

### Teknoloji Stack

**Backend:**
- .NET 8.0 (ASP.NET Core Web API)
- Entity Framework Core 8.0
- SQL Server (Docker container)
- JWT Authentication
- Swagger/OpenAPI
- Serilog (Logging)
- AutoMapper (DTO mapping)

**Frontend:**
- React 18+ (TypeScript)
- React Router v6
- Axios (HTTP client)
- Material-UI v5 veya Ant Design (UI framework)
- React Query / SWR (state management)
- Chart.js / Recharts (rapor grafikleri)

**Infrastructure:**
- Docker & Docker Compose
- SQL Server 2022 (Linux container)

---

## 📁 PROJE YAPISI

```
emek-kitabevi/
├── src/
│   ├── backend/
│   │   ├── EmekKitabevi.API/              # Web API projesi
│   │   │   ├── Controllers/
│   │   │   ├── Middleware/
│   │   │   ├── Program.cs
│   │   │   └── appsettings.json
│   │   ├── EmekKitabevi.Application/      # Business logic
│   │   │   ├── Services/
│   │   │   ├── DTOs/
│   │   │   ├── Mappings/
│   │   │   └── Validators/
│   │   ├── EmekKitabevi.Domain/           # Domain entities
│   │   │   ├── Entities/
│   │   │   ├── Enums/
│   │   │   └── Interfaces/
│   │   └── EmekKitabevi.Infrastructure/   # Data access
│   │       ├── Data/
│   │       ├── Repositories/
│   │       └── Migrations/
│   └── frontend/
│       ├── public/
│       ├── src/
│       │   ├── components/
│       │   ├── pages/
│       │   ├── services/
│       │   ├── hooks/
│       │   ├── utils/
│       │   └── App.tsx
│       ├── package.json
│       └── Dockerfile
├── docker/
│   ├── docker-compose.yml
│   ├── Dockerfile.api
│   └── Dockerfile.frontend
├── scripts/
│   ├── init-db.sql
│   └── seed-data.sql
├── .env.example
├── .gitignore
└── README.md
```

---

## 🗄️ VERİTABANI ŞEMASI

### Tablolar

#### 1. **Users** (Kullanıcılar)
```sql
- Id (UUID, PK)
- Username (nvarchar(100), unique)
- Email (nvarchar(255), unique)
- PasswordHash (nvarchar(500))
- FullName (nvarchar(200))
- Role (nvarchar(50)) -- Admin, User
- CreatedAt (datetime2)
- UpdatedAt (datetime2)
- IsActive (bit)
```

#### 2. **Books** (Kitaplar)
```sql
- Id (UUID, PK)
- ISBN (nvarchar(20), unique, nullable)
- Barcode (nvarchar(50), unique, nullable)
- Title (nvarchar(500))
- Author (nvarchar(300))
- Publisher (nvarchar(200))
- Category (nvarchar(100))
- CurrentPrice (decimal(18,2))
- StockQuantity (int)
- MinStockLevel (int)
- Description (nvarchar(max), nullable)
- CreatedAt (datetime2)
- UpdatedAt (datetime2)
- CreatedBy (UUID, FK -> Users)
- IsActive (bit)
```

#### 3. **PriceHistory** (Fiyat Geçmişi)
```sql
- Id (UUID, PK)
- BookId (UUID, FK -> Books)
- OldPrice (decimal(18,2))
- NewPrice (decimal(18,2))
- ChangedBy (UUID, FK -> Users)
- ChangeReason (nvarchar(500), nullable)
- ChangedAt (datetime2)
```

#### 4. **Transactions** (İşlemler - Satış/Alış)
```sql
- Id (UUID, PK)
- TransactionType (nvarchar(20)) -- Sale, Purchase, Return
- BookId (UUID, FK -> Books)
- Quantity (int)
- UnitPrice (decimal(18,2))
- TotalAmount (decimal(18,2))
- TransactionDate (datetime2)
- CreatedBy (UUID, FK -> Users)
- Notes (nvarchar(1000), nullable)
```

#### 5. **StockMovements** (Stok Hareketleri)
```sql
- Id (UUID, PK)
- BookId (UUID, FK -> Books)
- MovementType (nvarchar(20)) -- In, Out, Adjustment
- Quantity (int)
- PreviousStock (int)
- NewStock (int)
- Reason (nvarchar(500))
- CreatedBy (UUID, FK -> Users)
- CreatedAt (datetime2)
```

#### 6. **AuditLogs** (Genel Audit Log)
```sql
- Id (UUID, PK)
- EntityType (nvarchar(100))
- EntityId (UUID)
- Action (nvarchar(50)) -- Create, Update, Delete
- OldValues (nvarchar(max), nullable)
- NewValues (nvarchar(max), nullable)
- ChangedBy (UUID, FK -> Users)
- ChangedAt (datetime2)
- IpAddress (nvarchar(50), nullable)
- UserAgent (nvarchar(500), nullable)
```

---

## 🔌 API ENDPOINT'LERİ

### Authentication
```
POST   /api/auth/login
POST   /api/auth/register (opsiyonel)
POST   /api/auth/refresh-token
GET    /api/auth/me
```

### Books (Kitaplar)
```
GET    /api/books                    # Liste (filtreleme, sayfalama)
GET    /api/books/{id}               # Detay
POST   /api/books                    # Yeni kitap
PUT    /api/books/{id}               # Güncelleme
DELETE /api/books/{id}               # Silme
GET    /api/books/search             # ISBN/Barcode/Title ile arama
PUT    /api/books/{id}/price         # Fiyat güncelleme
PUT    /api/books/{id}/stock         # Stok güncelleme
GET    /api/books/{id}/price-history # Fiyat geçmişi
GET    /api/books/low-stock          # Düşük stoklu kitaplar
```

### Transactions (İşlemler)
```
GET    /api/transactions             # Liste (filtreleme, tarih aralığı)
GET    /api/transactions/{id}        # Detay
POST   /api/transactions/sale        # Satış işlemi
POST   /api/transactions/purchase    # Alış işlemi
POST   /api/transactions/return      # İade işlemi
```

### Reports (Raporlar)
```
GET    /api/reports/revenue          # Gelir raporu (tarih aralığı)
GET    /api/reports/expense          # Gider raporu (tarih aralığı)
GET    /api/reports/sales            # Satış raporu (detaylı)
GET    /api/reports/sales-summary    # Satış özeti
GET    /api/reports/profit-loss      # Kar/Zarar raporu
GET    /api/reports/stock-status     # Stok durumu raporu
```

### Logs (Loglar)
```
GET    /api/logs/audit               # Audit loglar (filtreleme)
GET    /api/logs/price-changes       # Fiyat değişiklik logları
GET    /api/logs/stock-movements     # Stok hareket logları
GET    /api/logs/user-activity       # Kullanıcı aktivite logları
```

---

## 🔐 GÜVENLİK & AUTHENTICATION

### JWT Token Yapısı
- Access Token: 15 dakika geçerlilik
- Refresh Token: 7 gün geçerlilik
- Claims: UserId, Username, Role, Email

### Authorization
- Role-based access control (RBAC)
- Admin: Tüm işlemler
- User: Sınırlı yetkiler (rapor görüntüleme, satış)

### Güvenlik Önlemleri
- Password hashing (BCrypt/Argon2)
- SQL Injection koruması (EF Core parameterized queries)
- CORS yapılandırması
- Rate limiting (opsiyonel)
- Input validation (FluentValidation)

---

## 📊 RAPORLAMA ÖZELLİKLERİ

### Gelir Raporu
- Tarih aralığına göre toplam gelir
- Günlük/Haftalık/Aylık/Yıllık gruplama
- Kitap bazında gelir dağılımı
- Grafik görselleştirme

### Gider Raporu
- Alış işlemleri toplamı
- Tarih aralığına göre giderler
- Kategori bazında gider analizi

### Satış Raporu
- Satış işlemleri detay listesi
- En çok satan kitaplar
- Satış trend analizi
- Müşteri bazında satış (opsiyonel)

### Kar/Zarar Raporu
- Gelir - Gider = Net Kar
- Dönemsel kar/zarar analizi
- Kitap bazında karlılık

---

## 📝 LOGLAMA STRATEJİSİ

### Log Seviyeleri
- **Information**: Normal işlemler (kitap ekleme, güncelleme)
- **Warning**: Düşük stok uyarıları, kritik olmayan hatalar
- **Error**: Sistem hataları, exception'lar
- **Audit**: Tüm değişiklikler (Create, Update, Delete)

### Log Detayları
- **Fiyat Güncelleme**: Eski fiyat, yeni fiyat, değişiklik nedeni, kullanıcı, zaman
- **Stok Hareketleri**: Önceki stok, yeni stok, miktar, hareket tipi, neden
- **Kullanıcı Aktiviteleri**: Login/logout, işlem yapılan endpoint'ler, IP adresi

### Log Depolama
- SQL Server'da AuditLogs tablosu
- Dosya tabanlı log (Serilog) - opsiyonel
- Log retention policy (90 gün)

---

## 🐳 DOCKER YAPILANDIRMASI

### docker-compose.yml Yapısı
```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql

  api:
    build:
      context: ./src/backend
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__DefaultConnection=...
    depends_on:
      - sqlserver

  frontend:
    build:
      context: ./src/frontend
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    depends_on:
      - api
```

---

## 🎨 FRONTEND YAPISI

### Sayfalar (Pages)
- `/login` - Giriş sayfası
- `/dashboard` - Ana dashboard
- `/books` - Kitap listesi
- `/books/new` - Yeni kitap ekleme
- `/books/:id` - Kitap detay/düzenleme
- `/books/search` - Gelişmiş arama
- `/transactions` - İşlemler listesi
- `/transactions/new` - Yeni işlem (satış/alış)
- `/reports/revenue` - Gelir raporu
- `/reports/expense` - Gider raporu
- `/reports/sales` - Satış raporu
- `/logs` - Log görüntüleme
- `/logs/price-history` - Fiyat geçmişi

### Bileşenler (Components)
- `BookList`, `BookCard`, `BookForm`
- `TransactionForm`, `TransactionList`
- `ReportChart`, `ReportTable`, `DateRangePicker`
- `LogViewer`, `PriceHistoryChart`
- `SearchBar` (ISBN/Barcode/Title arama)
- `StockAlert` (düşük stok uyarıları)

---

## ✅ GELİŞTİRME AŞAMALARI

### Faz 1: Temel Altyapı (1-2 gün)
- [ ] .NET API projesi oluşturma
- [ ] React frontend projesi oluşturma
- [ ] Docker yapılandırması
- [ ] SQL Server container setup
- [ ] Veritabanı migration'ları
- [ ] Swagger yapılandırması

### Faz 2: Authentication & Authorization (1 gün)
- [ ] JWT authentication implementasyonu
- [ ] User entity ve repository
- [ ] Login/Register endpoint'leri
- [ ] Frontend auth context ve protected routes

### Faz 3: Kitap Yönetimi (2-3 gün)
- [ ] Book entity ve CRUD işlemleri
- [ ] ISBN/Barcode/Title ile arama
- [ ] Fiyat güncelleme ve loglama
- [ ] Stok yönetimi (ekleme/çıkarma)
- [ ] Frontend kitap sayfaları

### Faz 4: İşlemler (Transaction) (1-2 gün)
- [ ] Transaction entity ve işlemleri
- [ ] Satış/Alış/İade işlemleri
- [ ] Stok otomatik güncelleme
- [ ] Frontend transaction formları

### Faz 5: Raporlama (2 gün)
- [ ] Gelir/Gider raporu endpoint'leri
- [ ] Satış raporu ve analizler
- [ ] Frontend rapor sayfaları ve grafikler
- [ ] Tarih aralığı filtreleme

### Faz 6: Loglama & Audit (1 gün)
- [ ] Audit log implementasyonu
- [ ] Fiyat geçmişi endpoint'leri
- [ ] Stok hareket logları
- [ ] Frontend log görüntüleme sayfaları

### Faz 7: Test & Optimizasyon (1-2 gün)
- [ ] Unit testler (backend)
- [ ] Integration testler
- [ ] Frontend testler
- [ ] Performance optimizasyonu
- [ ] Docker production build

---

## 🔧 TEKNİK DETAYLAR

### Backend Best Practices
- Repository Pattern
- Unit of Work Pattern
- Dependency Injection
- FluentValidation
- AutoMapper
- Global Exception Handler
- Response Wrapper (standardize API responses)

### Frontend Best Practices
- Component-based architecture
- Custom hooks (useAuth, useBooks, useReports)
- Error boundaries
- Loading states
- Form validation
- Responsive design

### Database
- Index'ler: ISBN, Barcode, Title, TransactionDate
- Foreign key constraints
- Cascade delete policies
- Database seeding (initial admin user)

---

## 📦 BAĞIMLILIKLAR

### Backend (.NET)
```
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Authentication.JwtBearer
Swashbuckle.AspNetCore
Serilog.AspNetCore
AutoMapper.Extensions.Microsoft.DependencyInjection
FluentValidation.AspNetCore
BCrypt.Net-Next
```

### Frontend (React)
```
react, react-dom, react-router-dom
typescript
axios
@mui/material (veya antd)
recharts (veya chart.js)
react-query (veya swr)
date-fns
```

---

## 🚀 ÇALIŞTIRMA

```bash
# Tüm servisleri başlat
docker-compose up -d

# Migration çalıştır (ilk kurulum)
dotnet ef database update

# API: http://localhost:5000
# Frontend: http://localhost:3000
# Swagger: http://localhost:5000/swagger
# SQL Server: localhost:1433
```

---

## 📝 NOTLAR

- Tüm ID'ler UUID (Guid) kullanılacak
- Tarih/saat işlemleri UTC olarak saklanacak, frontend'de local timezone'a çevrilecek
- Para birimi: TRY (Türk Lirası)
- Stok negatif olamaz (validation)
- Fiyat güncellemelerinde mutlaka neden (reason) girilmeli
- Tüm işlemler audit log'a kaydedilecek

---

## ⚠️ ÖNEMLİ GÜVENLİK NOTLARI

- Production'da güçlü SA password kullanılmalı
- JWT secret key environment variable'dan alınmalı
- CORS sadece frontend domain'ine izin vermeli
- SQL injection koruması (EF Core kullanımı)
- XSS koruması (input sanitization)
- HTTPS kullanımı (production)

---

Bu plan onaylandıktan sonra implementasyona başlayabiliriz. Herhangi bir değişiklik veya ekleme ister misiniz?

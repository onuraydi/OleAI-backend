<div align="center">

# 🍳 OleAI Backend API

**Yapay Zeka Destekli Akıllı Kiler Yönetimi, Reçete Analizi ve Kişiselleştirilmiş Tarif Öneri Platformu**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20%26%20CQRS-2ea44f?style=for-the-badge)](https://learn.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![Redis](https://img.shields.io/badge/Redis-Caching-DC382D?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
[![Ollama](https://img.shields.io/badge/Ollama-Local%20LLM-000000?style=for-the-badge)](https://ollama.com/)
[![Prometheus](https://img.shields.io/badge/Prometheus-Monitoring-E6522C?style=for-the-badge&logo=prometheus&logoColor=white)](https://prometheus.io/)

</div>

---

## 📖 Proje Hakkında

**OleAI Backend**, kullanıcıların mutfak kilerlerindeki malzemeleri (son kullanma tarihleri ve ölçü birimleriyle) takip etmelerini sağlayan, ellerindeki malzemelere, alerjilerine, diyet tercihlerine ve sevmedikleri malzemelere göre **Yapay Zeka (AI) destekli tarif önerileri** sunan kurumsal ölçekli bir RESTful API servisidir.

Sistem; tarif oluşturma aşamasında yerel büyük dil modelleri (**Ollama - `OleAI-Editor`**) ile tarifleri otomatik analiz eder, pişirme (`StartCooking`) esnasında kilerden otomatik stok düşümü yapar ve tükenen malzemeleri olay güdümlü (**Domain Events**) olarak akıllı alışveriş listesine (`SmartRestock`) aktarır.

---

## ✨ Temel Özellikler

* **🧠 AI Tarif Öneri & Kullanıcı Vektör Entegrasyonu:** Kullanıcının kilerindeki malzemeler, alerji kısıtlamaları (`AllergyRestrictions`), diyet tercihleri (`DietaryPreferences`) ve sevmediği malzemeler (`DislikedIngredients`) harmanlanarak Python tabanlı AI öneri motoru ile haberleşilir.
* **🤖 Yerel LLM ile Tarif Analizi (Ollama):** `OleAI-Editor.Modelfile` ile özelleştirilmiş yerel LLM modeli sayesinde tarif metinleri, adımları ve malzeme tutarlılıkları otomatik olarak analiz edilir (`AnalyzeRecipeWithLlm`).
* **🥫 Akıllı Kiler (Pantry) ve Birim Dönüştürme:** Gram, mililitre, adet, su bardağı gibi farklı ölçü birimleri (`MeasurementUnitHelper`) arasında otomatik dönüşüm yapılır. Kilerde olmayan malzemeler için kullanıcılar malzeme talebi (`IngredientRequest`) oluşturabilir.
* **🔄 Event-Driven Stok ve Alışveriş Listesi Senkronizasyonu:**
  * Kullanıcı bir tarifi pişirmeye başladığında (`RecipeConsumedEvent`), kullanılan miktarlar kilerden otomatik düşülür (`InventoryDeductionEventHandler`).
  * Kilerde tükenen ürünler (`IngredientDepletedEvent`) otomatik olarak alışveriş listesine eklenir (`ShoppingListEventHandler`).
  * Alışveriş listesinden satın alınan ürünler (`ItemPurchasedEvent`) tek tıkla kilere geri aktarılır (`PantryAdditionEventHandler`).
* **⏰ Arka Plan İşlemleri ve Bildirimler (Hangfire & Firebase FCM):** `ExpirationNotifierJob` ile son kullanma tarihi yaklaşan kiler ürünleri düzenli taranır ve kullanıcının mobil cihazlarına (`UserDevice`) Firebase Cloud Messaging üzerinden anlık bildirim gönderilir.
* **🛡️ Gelişmiş Kimlik Doğrulama & Güvenlik:** JWT Access/Refresh Token rotasyonu, e-posta OTP doğrulaması (`OtpHelper`), rol bazlı yetkilendirme (`Admin`, `User`), hesap kilitleme/banlama mekanizmaları.
* **📊 Gözlemlenebilirlik (Observability) ve Loglama:** `CorrelationIdMiddleware` ile uçtan uca istek takibi, veritabanı ve dosya tabanlı sistem logları (`AdminLogs`), Prometheus sağlık ve metrik izleyicisi (`HealthCheckMetricsService`).

---

## 🏗️ Mimari ve Teknoloji Yığını

Proje, **Clean Architecture (Temiz Mimari)** ve **CQRS (Command Query Responsibility Segregation)** prensiplerine sıkı sıkıya bağlı kalınarak 4 ana katmanda geliştirilmiştir:

```text
OleAI-backend/
├── src/
│   ├── OleAI.Backend.Core/          # Cross-Cutting Concerns (JWT, Hashing, OTP, Redis, DynamicQuery, Pagination, Exceptions, Mailing)
│   ├── OleAI.Backend.Domain/        # Domain Entities, Enums, Domain Constants ve Domain Services (MeasurementUnitHelper)
│   ├── OleAI.Backend.Application/   # CQRS Features (Commands, Queries, EventHandlers), FluentValidation, AutoMapper, Business Rules
│   └── OleAI.Backend.Presentation/  # ASP.NET Core Web API Controllers, Custom ModelBinders, Middlewares, Filters, Swagger
├── tests/                           # Katman bazlı xUnit Birim Testleri (Core, Domain, Application, Persistence, Presentation)
├── infra/                           # Bulut sunucu kurulum ve provisioning scriptleri (cloud-init, provision.sh)
├── monitoring/                      # Prometheus metrik toplama ve izleme yapılandırmaları
└── nginx/                           # Reverse Proxy yapılandırması (nginx.conf)
```

### Kullanılan Teknolojiler
* **Framework:** .NET 9 / ASP.NET Core Web API
* **Mimari Desenler:** Clean Architecture, CQRS (MediatR), Domain-Driven Events, Repository Pattern, Unit of Work
* **Veritabanı & ORM:** Entity Framework Core (Code-First Migrations, Auto-Seeding)
* **Önbellekleme (Caching):** Redis (`RedisCacheManager`)
* **Doğrulama & Eşleme:** FluentValidation (`ValidationBehavior`), AutoMapper
* **Arka Plan Görevleri:** Hangfire
* **Medya Depolama:** Cloudinary (`CloudinaryImageManager`)
* **Bildirim & İletişim:** Firebase Admin SDK (Push Notifications), SMTP Mail Service (`SmtpMailManager`)
* **Yapay Zeka & LLM:** Ollama (`OllamaLocalLlmManager`), HTTP AI Recommendation Microservice İstemcisi
* **DevOps & Kalite:** Docker, Docker Compose, Nginx, Prometheus, GitHub Actions (CI/CD), Husky.Net (Pre-commit hooks)

---

## 🚀 Kurulum ve Çalıştırma (Getting Started)

Projeyi yerel ortamınızda **Docker ile (Önerilen)** veya **.NET CLI ile** ayağa kaldırabilirsiniz.

### Ön Gereksinimler
* [Git](https://git-scm.com/)
* [.NET SDK](https://dotnet.microsoft.com/download)
* [Docker & Docker Compose](https://www.docker.com/)
* [Ollama](https://ollama.com/) *(Yerel LLM tarif analizi özelliğini kullanmak için)*

### 1. Projeyi Klonlayın

```bash
git clone [https://github.com/](https://github.com/)<kullanici-adin>/OleAI-backend.git
cd OleAI-backend
```

### 2. Ortam Değişkenlerini (`.env`) Yapılandırın

Kök dizinde bulunan `.env.example` dosyasını kopyalayarak `.env` dosyasını oluşturun:

```bash
cp .env.example .env
```

`.env` dosyasını açarak veritabanı bağlantı cümlenizi, JWT anahtarınızı, Redis, Cloudinary ve SMTP ayarlarınızı kendi ortamınıza göre doldurun.

> **⚠️ Firebase Bildirimleri İçin Önemli Not:**
> Push Notification servisinin çalışması için Firebase Console üzerinden aldığınız Service Account JSON dosyasını `src/OleAI.Backend.Presentation/firebase-adminsdk.json` yoluna ekleyin (Bu dosya güvenlik gereği `.gitignore` içerisindedir, asla commit etmeyin).

### 3. Yerel LLM Modelini (Ollama) Hazırlayın

Tariflerin yapay zeka tarafından incelenmesi (`OleAI-Editor`) için proje kök dizinindeki script'i kullanarak özel Ollama modelini oluşturun:

**Windows (PowerShell):**
```powershell
.\deploy-ollama-model.ps1
```

**Linux / macOS:**
```bash
chmod +x deploy-ollama-model.sh
./deploy-ollama-model.sh
```

### 4. Uygulamayı Çalıştırın

#### Seçenek A: Docker Compose ile Çalıştırma (Önerilen)
Tüm altyapıyı (API, Veritabanı, Redis vb.) tek komutla ayağa kaldırın:

```bash
docker compose up -d --build
```

Production ortamı simülasyonu için:
```bash
docker compose -f docker-compose.prod.yml up -d --build
```

Monitoring (Prometheus) servislerini ayağa kaldırmak için:
```bash
docker compose -f monitoring/docker-compose.yml up -d
```

#### Seçenek B: .NET CLI ile Yerel Çalıştırma
```bash
# Bağımlılıkları yükleyin
dotnet restore src/OleAI.Backend.sln

# Veritabanı migration'larını uygulayın (Gerekirse)
dotnet ef database update --project src/OleAI.Backend.Persistence --startup-project src/OleAI.Backend.Presentation

# Projeyi başlatın
dotnet run --project src/OleAI.Backend.Presentation
```
> *Not: Uygulama ilk kez çalıştığında `DbSeeder` devreye girerek varsayılan rolleri, kategorileri, alerji kısıtlamalarını ve örnek verileri otomatik olarak veritabanına ekleyecektir.*

---

## 🧪 Testleri Çalıştırma

Projenin tüm katmanları (`Core`, `Domain`, `Application`, `Persistence`, `Presentation`) kapsamlı birim testlerine sahiptir. Testleri çalıştırmak için:

```bash
dotnet test src/OleAI.Backend.sln --configuration Release
```

---

## 📚 API Dokümantasyonu ve Uç Noktalar (Endpoints)

Uygulama ayağa kalktıktan sonra interaktif **Swagger UI** arayüzüne aşağıdaki adresten ulaşabilirsiniz:
* **Swagger UI:** `http://localhost:<PORT>/swagger`
* **Hangfire Dashboard:** `http://localhost:<PORT>/hangfire` *(Sadece Admin yetkisiyle erişilebilir)*
* **Prometheus Metrics:** `http://localhost:<PORT>/metrics`

### Ana Modüller (Controllers)
| Modül | Açıklama |
| :--- | :--- |
| **`AuthController`** | Kayıt, Giriş, Refresh Token, E-posta OTP Doğrulama, Şifre Sıfırlama, Rol Atama ve Kullanıcı Ban/Unlock işlemleri. |
| **`PantryController`** | Kullanıcının kilerindeki malzemeleri listeleme, çoklu/tekli ekleme, arama, son kullanma tarihi geçmişleri temizleme ve yeni malzeme talebi oluşturma. |
| **`RecipesController`** | Tarif oluşturma (`multipart/form-data` ve özel ModelBinder destekli), LLM ile analiz etme, pişirmeye başlama (`StartCooking`), yorum yapma, beğenme/kaydetme. |
| **`ShoppingListController`** | Alışveriş listesi yönetimi, tükenen malzemeleri senkronize etme, tekli/toplu satın alındı işaretleme. |
| **`UsersController`** | Kullanıcı profili, istatistikler, alerji/diyet/sevilmeyen malzeme tercihlerini güncelleme ve **AI Tarif Önerisi (`GetRecommendation`)** alma. |
| **`Admin*Controllers`** | Sistem istatistikleri (`AdminDashboard`), bekleyen malzeme taleplerini onaylama (`AdminPantry`), sistem/DB loglarını inceleme (`AdminLogs`) ve iletişim mesajları. |

---

## 🪝 Git Kancaları (Husky.Net)

Projede kod kalitesini korumak adına `.husky` üzerinden `pre-commit` kancaları tanımlanmıştır. Klonlama sonrasında Husky'yi aktif etmek için:

```bash
dotnet tool restore
dotnet husky install
```

---

## 🤝 Katkıda Bulunma

Projeye katkıda bulunmak isterseniz lütfen önce [CONTRIBUTING.md](CONTRIBUTING.md) dosyasını inceleyin.

## 📄 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır.

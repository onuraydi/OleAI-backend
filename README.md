<div align="center">

# 🍳 OleAI Backend API

**AI-Powered Smart Pantry Management, Recipe Analysis, and Personalized Recipe Recommendation Platform**

**Yapay Zeka Destekli Akıllı Kiler Yönetimi, Reçete Analizi ve Kişiselleştirilmiş Tarif Öneri Platformu**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20%26%20CQRS-2ea44f?style=for-the-badge)](https://learn.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![Redis](https://img.shields.io/badge/Redis-Caching-DC382D?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
[![Ollama](https://img.shields.io/badge/Ollama-Local%20LLM-000000?style=for-the-badge)](https://ollama.com/)
[![Prometheus](https://img.shields.io/badge/Prometheus-Monitoring-E6522C?style=for-the-badge&logo=prometheus&logoColor=white)](https://prometheus.io/)

**🌐 Language / Dil:** &nbsp; [🇬🇧 English](#en) &nbsp;|&nbsp; [🇹🇷 Türkçe](#tr)

</div>

---

<a id="en"></a>

> 🇹🇷 Türkçe okumak için: **[Türkçe sürüme git ↓](#tr)**

## About the Project

**OleAI Backend** is an enterprise-grade RESTful API service that lets users track the ingredients in their kitchen pantry (with expiration dates and measurement units) and provides **AI-powered recipe recommendations** based on the ingredients they have, their allergies, dietary preferences, and disliked ingredients.

When a recipe is created, the system automatically analyzes it with a local large language model (**Ollama - `OleAI-Editor`**), deducts stock from the pantry automatically while cooking (`StartCooking`), and transfers depleted ingredients to the smart shopping list (`SmartRestock`) in an event-driven manner (**Domain Events**).

---

## Key Features

* **AI Recipe Recommendation & User Vector Integration:** The user's pantry ingredients, allergy restrictions (`AllergyRestrictions`), dietary preferences (`DietaryPreferences`), and disliked ingredients (`DislikedIngredients`) are combined and sent to a Python-based AI recommendation engine.
* **Recipe Analysis with a Local LLM (Ollama):** Thanks to a local LLM customized via `OleAI-Editor.Modelfile`, recipe texts, steps, and ingredient consistency are analyzed automatically (`AnalyzeRecipeWithLlm`).
* **Smart Pantry & Unit Conversion:** Automatic conversion between different measurement units such as grams, milliliters, pieces, and cups (`MeasurementUnitHelper`). For ingredients not available in the pantry, users can submit an ingredient request (`IngredientRequest`).
* **Event-Driven Stock & Shopping List Synchronization:**
  * When a user starts cooking a recipe (`RecipeConsumedEvent`), the used quantities are automatically deducted from the pantry (`InventoryDeductionEventHandler`).
  * Items depleted from the pantry (`IngredientDepletedEvent`) are automatically added to the shopping list (`ShoppingListEventHandler`).
  * Items purchased from the shopping list (`ItemPurchasedEvent`) are moved back into the pantry with a single tap (`PantryAdditionEventHandler`).
* **Background Jobs & Notifications (Hangfire & Firebase FCM):** `ExpirationNotifierJob` regularly scans pantry items nearing their expiration date and sends instant notifications to the user's mobile devices (`UserDevice`) via Firebase Cloud Messaging.
* **Advanced Authentication & Security:** JWT Access/Refresh Token rotation, email OTP verification (`OtpHelper`), role-based authorization (`Admin`, `User`), and account locking/banning mechanisms.
* **Observability & Logging:** End-to-end request tracing with `CorrelationIdMiddleware`, database- and file-based system logs (`AdminLogs`), and a Prometheus health and metrics monitor (`HealthCheckMetricsService`).

---

## Architecture and Tech Stack

The project is built strictly following **Clean Architecture** and **CQRS (Command Query Responsibility Segregation)** principles across 4 main layers:

```text
OleAI-backend/
├── src/
│   ├── OleAI.Backend.Core/          # Cross-Cutting Concerns (JWT, Hashing, OTP, Redis, DynamicQuery, Pagination, Exceptions, Mailing)
│   ├── OleAI.Backend.Domain/        # Domain Entities, Enums, Domain Constants and Domain Services (MeasurementUnitHelper)
│   ├── OleAI.Backend.Application/   # CQRS Features (Commands, Queries, EventHandlers), FluentValidation, AutoMapper, Business Rules
│   └── OleAI.Backend.Presentation/  # ASP.NET Core Web API Controllers, Custom ModelBinders, Middlewares, Filters, Swagger
├── tests/                           # Layer-based xUnit Unit Tests (Core, Domain, Application, Persistence, Presentation)
├── infra/                           # Cloud server setup and provisioning scripts (cloud-init, provision.sh)
├── monitoring/                      # Prometheus metric collection and monitoring configurations
└── nginx/                           # Reverse Proxy configuration (nginx.conf)
```

### Technologies Used
* **Framework:** .NET 9 / ASP.NET Core Web API
* **Architectural Patterns:** Clean Architecture, CQRS (MediatR), Domain-Driven Events, Repository Pattern, Unit of Work
* **Database & ORM:** Entity Framework Core (Code-First Migrations, Auto-Seeding)
* **Caching:** Redis (`RedisCacheManager`)
* **Validation & Mapping:** FluentValidation (`ValidationBehavior`), AutoMapper
* **Background Jobs:** Hangfire
* **Media Storage:** Cloudinary (`CloudinaryImageManager`)
* **Notifications & Communication:** Firebase Admin SDK (Push Notifications), SMTP Mail Service (`SmtpMailManager`)
* **AI & LLM:** Ollama (`OllamaLocalLlmManager`), HTTP AI Recommendation Microservice Client
* **DevOps & Quality:** Docker, Docker Compose, Nginx, Prometheus, GitHub Actions (CI/CD), Husky.Net (Pre-commit hooks)

---

## Getting Started

You can run the project locally either **with Docker (Recommended)** or **with the .NET CLI**.

### Prerequisites
* [Git](https://git-scm.com/)
* [.NET SDK](https://dotnet.microsoft.com/download)
* [Docker & Docker Compose](https://www.docker.com/)
* [Ollama](https://ollama.com/) *(To use the local LLM recipe analysis feature)*

### 1. Clone the Project

```bash
git clone https://github.com/onuraydi/OleAI-backend.git
cd OleAI-backend
```

### 2. Configure Environment Variables (`.env`)

Create the `.env` file by copying the `.env.example` file in the root directory:

```bash
cp .env.example .env
```

Open the `.env` file and fill in your database connection string, JWT key, Redis, Cloudinary, and SMTP settings according to your own environment.

> **Important Note for Firebase Notifications:**
> For the Push Notification service to work, add the Service Account JSON file you obtained from the Firebase Console to the path `src/OleAI.Backend.Presentation/firebase-adminsdk.json` (This file is listed in `.gitignore` for security reasons; never commit it).

### 3. Prepare the Local LLM Model (Ollama)

To enable AI-based recipe review (`OleAI-Editor`), create the custom Ollama model using the script in the project root directory:

**Windows (PowerShell):**
```powershell
.\deploy-ollama-model.ps1
```

**Linux / macOS:**
```bash
chmod +x deploy-ollama-model.sh
./deploy-ollama-model.sh
```

### 4. Run the Application

#### Option A: Run with Docker Compose (Recommended)
Bring up the entire infrastructure (API, Database, Redis, etc.) with a single command:

```bash
docker compose up -d --build
```

To simulate a production environment:
```bash
docker compose -f docker-compose.prod.yml up -d --build
```

To start the monitoring (Prometheus) services:
```bash
docker compose -f monitoring/docker-compose.yml up -d
```

#### Option B: Run Locally with the .NET CLI
```bash
# Install dependencies
dotnet restore src/OleAI.Backend.sln

# Apply database migrations (if needed)
dotnet ef database update --project src/OleAI.Backend.Persistence --startup-project src/OleAI.Backend.Presentation

# Start the project
dotnet run --project src/OleAI.Backend.Presentation
```
> *Note: When the application runs for the first time, `DbSeeder` kicks in and automatically adds the default roles, categories, allergy restrictions, and sample data to the database.*

---

## Running the Tests

All layers of the project (`Core`, `Domain`, `Application`, `Persistence`, `Presentation`) have comprehensive unit tests. To run the tests:

```bash
dotnet test src/OleAI.Backend.sln --configuration Release
```

---

## API Documentation and Endpoints

Once the application is up, you can access the interactive **Swagger UI** at the following addresses:
* **Swagger UI:** `http://localhost:<PORT>/swagger`
* **Hangfire Dashboard:** `http://localhost:<PORT>/hangfire` *(Accessible only with Admin privileges)*
* **Prometheus Metrics:** `http://localhost:<PORT>/metrics`

### Main Modules (Controllers)
| Module | Description |
| :--- | :--- |
| **`AuthController`** | Registration, Login, Refresh Token, Email OTP Verification, Password Reset, Role Assignment, and User Ban/Unlock operations. |
| **`PantryController`** | Listing pantry ingredients, adding single/multiple items, searching, clearing expired items, and creating new ingredient requests. |
| **`RecipesController`** | Recipe creation (`multipart/form-data` with custom ModelBinder support), LLM analysis, starting to cook (`StartCooking`), commenting, liking/saving. |
| **`ShoppingListController`** | Shopping list management, syncing depleted ingredients, marking single/bulk items as purchased. |
| **`UsersController`** | User profile, statistics, updating allergy/diet/disliked-ingredient preferences, and getting an **AI Recipe Recommendation (`GetRecommendation`)**. |
| **`Admin*Controllers`** | System statistics (`AdminDashboard`), approving pending ingredient requests (`AdminPantry`), inspecting system/DB logs (`AdminLogs`), and contact messages. |

---

## Git Hooks (Husky.Net)

To maintain code quality, `pre-commit` hooks are defined via `.husky`. To activate Husky after cloning:

```bash
dotnet tool restore
dotnet husky install
```

---

## Contributing

If you would like to contribute to the project, please read the [CONTRIBUTING.md](CONTRIBUTING.md) file first.

## License

This project is licensed under the MIT License.

<div align="right">

[⬆ Back to top](#) &nbsp;|&nbsp; [🇹🇷 Türkçe sürüme git](#tr)

</div>

---
---

<a id="tr"></a>

> 🇬🇧 To read in English: **[Go to the English version ↑](#en)**

## Proje Hakkında

**OleAI Backend**, kullanıcıların mutfak kilerlerindeki malzemeleri (son kullanma tarihleri ve ölçü birimleriyle) takip etmelerini sağlayan, ellerindeki malzemelere, alerjilerine, diyet tercihlerine ve sevmedikleri malzemelere göre **Yapay Zeka (AI) destekli tarif önerileri** sunan kurumsal ölçekli bir RESTful API servisidir.

Sistem; tarif oluşturma aşamasında yerel büyük dil modelleri (**Ollama - `OleAI-Editor`**) ile tarifleri otomatik analiz eder, pişirme (`StartCooking`) esnasında kilerden otomatik stok düşümü yapar ve tükenen malzemeleri olay güdümlü (**Domain Events**) olarak akıllı alışveriş listesine (`SmartRestock`) aktarır.

---

## Temel Özellikler

* **AI Tarif Öneri & Kullanıcı Vektör Entegrasyonu:** Kullanıcının kilerindeki malzemeler, alerji kısıtlamaları (`AllergyRestrictions`), diyet tercihleri (`DietaryPreferences`) ve sevmediği malzemeler (`DislikedIngredients`) harmanlanarak Python tabanlı AI öneri motoru ile haberleşilir.
* **Yerel LLM ile Tarif Analizi (Ollama):** `OleAI-Editor.Modelfile` ile özelleştirilmiş yerel LLM modeli sayesinde tarif metinleri, adımları ve malzeme tutarlılıkları otomatik olarak analiz edilir (`AnalyzeRecipeWithLlm`).
* **Akıllı Kiler (Pantry) ve Birim Dönüştürme:** Gram, mililitre, adet, su bardağı gibi farklı ölçü birimleri (`MeasurementUnitHelper`) arasında otomatik dönüşüm yapılır. Kilerde olmayan malzemeler için kullanıcılar malzeme talebi (`IngredientRequest`) oluşturabilir.
* **Event-Driven Stok ve Alışveriş Listesi Senkronizasyonu:**
  * Kullanıcı bir tarifi pişirmeye başladığında (`RecipeConsumedEvent`), kullanılan miktarlar kilerden otomatik düşülür (`InventoryDeductionEventHandler`).
  * Kilerde tükenen ürünler (`IngredientDepletedEvent`) otomatik olarak alışveriş listesine eklenir (`ShoppingListEventHandler`).
  * Alışveriş listesinden satın alınan ürünler (`ItemPurchasedEvent`) tek tıkla kilere geri aktarılır (`PantryAdditionEventHandler`).
* **Arka Plan İşlemleri ve Bildirimler (Hangfire & Firebase FCM):** `ExpirationNotifierJob` ile son kullanma tarihi yaklaşan kiler ürünleri düzenli taranır ve kullanıcının mobil cihazlarına (`UserDevice`) Firebase Cloud Messaging üzerinden anlık bildirim gönderilir.
* **Gelişmiş Kimlik Doğrulama & Güvenlik:** JWT Access/Refresh Token rotasyonu, e-posta OTP doğrulaması (`OtpHelper`), rol bazlı yetkilendirme (`Admin`, `User`), hesap kilitleme/banlama mekanizmaları.
* **Gözlemlenebilirlik (Observability) ve Loglama:** `CorrelationIdMiddleware` ile uçtan uca istek takibi, veritabanı ve dosya tabanlı sistem logları (`AdminLogs`), Prometheus sağlık ve metrik izleyicisi (`HealthCheckMetricsService`).

---

## Mimari ve Teknoloji Yığını

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

## Kurulum ve Çalıştırma (Getting Started)

Projeyi yerel ortamınızda **Docker ile (Önerilen)** veya **.NET CLI ile** ayağa kaldırabilirsiniz.

### Ön Gereksinimler
* [Git](https://git-scm.com/)
* [.NET SDK](https://dotnet.microsoft.com/download)
* [Docker & Docker Compose](https://www.docker.com/)
* [Ollama](https://ollama.com/) *(Yerel LLM tarif analizi özelliğini kullanmak için)*

### 1. Projeyi Klonlayın

```bash
git clone https://github.com/onuraydi/OleAI-backend.git
cd OleAI-backend
```

### 2. Ortam Değişkenlerini (`.env`) Yapılandırın

Kök dizinde bulunan `.env.example` dosyasını kopyalayarak `.env` dosyasını oluşturun:

```bash
cp .env.example .env
```

`.env` dosyasını açarak veritabanı bağlantı cümlenizi, JWT anahtarınızı, Redis, Cloudinary ve SMTP ayarlarınızı kendi ortamınıza göre doldurun.

> **Firebase Bildirimleri İçin Önemli Not:**
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

## Testleri Çalıştırma

Projenin tüm katmanları (`Core`, `Domain`, `Application`, `Persistence`, `Presentation`) kapsamlı birim testlerine sahiptir. Testleri çalıştırmak için:

```bash
dotnet test src/OleAI.Backend.sln --configuration Release
```

---

## API Dokümantasyonu ve Uç Noktalar (Endpoints)

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

## Git Kancaları (Husky.Net)

Projede kod kalitesini korumak adına `.husky` üzerinden `pre-commit` kancaları tanımlanmıştır. Klonlama sonrasında Husky'yi aktif etmek için:

```bash
dotnet tool restore
dotnet husky install
```

---

## Katkıda Bulunma

Projeye katkıda bulunmak isterseniz lütfen önce [CONTRIBUTING.md](CONTRIBUTING.md) dosyasını inceleyin.

## Lisans

Bu proje MIT Lisansı altında lisanslanmıştır.

<div align="right">

[⬆ Başa dön](#) &nbsp;|&nbsp; [🇬🇧 İngilizce sürüme git](#en)

</div>

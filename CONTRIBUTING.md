<div align="center">

**🌐 Language / Dil:** &nbsp; [🇬🇧 English](#en) &nbsp;|&nbsp; [🇹🇷 Türkçe](#tr)

</div>

---

<a id="en"></a>

> 🇹🇷 Türkçe okumak için: **[Türkçe sürüme git ↓](#tr)**

# OleAI Backend Contributing Guide

Thank you for your interest in contributing to the **OleAI Backend** project! To maintain the highest standards of code quality, architectural integrity (**Clean Architecture & CQRS**), and testability, we kindly ask you to follow the guidelines outlined below.

---

## 1. Setting Up the Development Environment

1. **Fork** the repository to your own GitHub account and clone it to your local machine:

   ```bash
   git clone https://github.com/onuraydi/OleAI-backend.git
   cd OleAI-backend
   ```

2. Restore the local .NET tools and install Git hooks using Husky:

   ```bash
   dotnet tool restore
   dotnet husky install
   ```

3. Copy the `.env.example` file to `.env` and configure your local development environment.

4. Before starting development, make sure all tests pass:

   ```bash
   dotnet test src/OleAI.Backend.sln
   ```

---

## 2. Branching and Commit Standards

### Branch Naming Conventions

Create a new branch from `main` (or `develop`) for every new feature or bug fix.

- **New Feature:** `feat/<feature-name>` (e.g., `feat/recipe-nutrition-calculator`)
- **Bug Fix:** `fix/<bug-name>` (e.g., `fix/pantry-unit-conversion-bug`)
- **Architecture / Refactoring:** `refactor/<scope>` (e.g., `refactor/redis-cache-keys`)
- **Documentation:** `docs/<scope>` (e.g., `docs/update-readme`)
- **Tests:** `test/<scope>` (e.g., `test/shopping-list-handler-tests`)

### Conventional Commits

The **Conventional Commits** standard is mandatory for this project. All commit messages must follow the format below:

```text
<type>(<scope>): <short description>

[optional detailed description]
```

**Examples:**

- `feat(pantry): add bulk expiration date check endpoint`
- `fix(recipes): handle null images in CreateRecipeCommandModelBinder`
- `test(auth): add unit tests for RefreshTokenCommandHandler`
- `chore(ci): update github actions dotnet version`

> **Note:** When you create a commit, the `.husky/pre-commit` hook runs automatically to check code formatting and build status.

---

## 3. Architecture and Coding Standards

The project includes an `.editorconfig` file. Make sure your IDE respects the rules defined in this file.

When implementing a new feature, you **must** follow the layer-specific rules described below.

### A. Application Layer (CQRS & Feature-Sliced Structure)

Each new feature must be organized under `src/OleAI.Backend.Application/Features/<FeatureName>/`, with the following directory structure where applicable:

1. **`Commands/` or `Queries/`**
   - Each Command or Query must have its own subdirectory.
   - Example: `Commands/CreateIngredient/CreateIngredientCommand.cs` and `CreateIngredientCommandHandler.cs`.

2. **`Rules/` (Business Rules)**
   - Business logic must not be embedded directly in handlers.
   - Place business logic checks in a `<Feature>BusinessRules.cs` class.
   - Throw `BusinessException` or `NotFoundException` when a business rule fails or a requested resource cannot be found.

3. **`Validations/` (FluentValidation)**
   - Every Command received from a user must have a corresponding `AbstractValidator<TCommand>` implementation.
   - The `ValidationBehavior` pipeline will execute these validators automatically.

4. **`Constants/`**
   - Do not use magic strings for error or informational messages.
   - Define these messages in a `<Feature>Messages.cs` class and reference the corresponding constants.

5. **`Profiles/`**
   - Define AutoMapper profiles for Entity-to-DTO and DTO-to-Entity mappings in the relevant feature's `Profiles/` directory.

### B. Presentation Layer (Controllers)

- **Controllers must not contain business logic.**
- Controllers must inherit from `BaseController` and should only dispatch requests through `Mediator.Send(...)` and return the appropriate HTTP status codes, such as `Ok`, `Created`, or `NoContent`.
- Do not use `try-catch` blocks for error handling in controllers. All exceptions are handled by `ExceptionMiddleware` and returned in the standard `ProblemDetails` format.

### C. Security Guidelines

- **Never** hardcode or commit API keys, passwords, connection strings, or sensitive files such as `firebase-adminsdk.json`.
- When introducing a new external service configuration class (`*Settings.cs`), add the corresponding configuration keys to both `.env.example` and `appsettings.json`, leaving sensitive values empty or using safe placeholders.

---

## 4. Unit Testing Requirements

High test coverage is a critical priority for the OleAI Backend project.

- Create corresponding test classes under `tests/OleAI.Backend.Application.Tests/` for every new:
  - `CommandHandler`
  - `QueryHandler`
  - `BusinessRules` class
  - `Validator`

- Use the `MethodName_StateUnderTest_ExpectedBehavior` naming convention or other clear, scenario-based test names.
- Use `Moq` or the mocking library already adopted by the project to isolate dependencies and ensure that tests focus on the behavior being tested.

---

## 5. Pull Request (PR) Process

1. After completing your changes and ensuring that all local tests pass, push your branch to your fork:

   ```bash
   git push origin feat/feature-name
   ```

2. Open a **Pull Request (PR)** on GitHub against the upstream repository.

3. Include the following information in your PR description:
   - A summary of what you changed and why.
   - The relevant Issue number, if applicable (e.g., `Closes #12`).
   - A clear note if you have introduced any new `.env` variables.

4. Ensure that all build and test steps in the GitHub Actions workflow (`backend-ci.yml`) complete successfully.

---

Thank you for helping us improve OleAI Backend! By following these guidelines, you help maintain a consistent, secure, maintainable, and testable codebase.

<div align="right">

[⬆ Back to top](#) &nbsp;|&nbsp; [🇹🇷 Türkçe sürüme git](#tr)

</div>

---
---

<a id="tr"></a>

> 🇬🇧 To read in English: **[Go to the English version ↑](#en)**

# OleAI Backend Katkıda Bulunma Rehberi (Contributing Guide)

**OleAI Backend** projesine katkıda bulunmak istediğiniz için teşekkür ederiz! Projemizin kod kalitesini, mimari bütünlüğünü (**Clean Architecture & CQRS**) ve test edilebilirliğini en üst düzeyde tutmak için aşağıdaki kurallara uymanızı rica ediyoruz.

---

## 1. Geliştirme Ortamının Hazırlanması

1. Projeyi kendi hesabınıza **Fork** edin ve yerel makinenize klonlayın:
   ```bash
   git clone https://github.com/onuraydi/OleAI-backend.git
   cd OleAI-backend
   ```
2. .NET yerel araçlarını (Husky vb.) geri yükleyin:
   ```bash
   dotnet tool restore
   dotnet husky install
   ```
3. `.env.example` dosyasını `.env` olarak kopyalayın ve yerel geliştirme ayarlarınızı yapın.
4. Kod yazmaya başlamadan önce tüm testlerin geçtiğinden emin olun:
   ```bash
   dotnet test src/OleAI.Backend.sln
   ```

---

## 2. Dal (Branch) ve Commit Standartları

### Branch İsimlendirme
Yeni bir özellik veya hata düzeltmesi için `main` (veya `develop`) dalından yeni bir dal oluşturun:
* **Yeni Özellik:** `feat/<ozellik-adi>` (Örn: `feat/recipe-nutrition-calculator`)
* **Hata Düzeltmesi:** `fix/<hata-adi>` (Örn: `fix/pantry-unit-conversion-bug`)
* **Mimari / Refactoring:** `refactor/<kapsam>` (Örn: `refactor/redis-cache-keys`)
* **Dokümantasyon:** `docs/<kapsam>` (Örn: `docs/update-readme`)
* **Test Ekleme:** `test/<kapsam>` (Örn: `test/shopping-list-handler-tests`)

### Conventional Commits
Projemizde **Conventional Commits** standardı zorunludur. Commit mesajlarınız aşağıdaki formatta olmalıdır:

```text
<tip>(<kapsam>): <kısa açıklama>

[opsiyonel detaylı açıklama]
```

**Örnekler:**
* `feat(pantry): add bulk expiration date check endpoint`
* `fix(recipes): handle null images in CreateRecipeCommandModelBinder`
* `test(auth): add unit tests for RefreshTokenCommandHandler`
* `chore(ci): update github actions dotnet version`

> **Not:** Commit attığınızda `.husky/pre-commit` kancası otomatik olarak çalışacak ve kod formatını / derleme durumunu kontrol edecektir.

---

## 🏛️ 3. Mimari Kurallar ve Kodlama Standartları

Projemizde `.editorconfig` dosyası tanımlıdır. IDE'nizin bu dosyayı dikkate aldığından emin olun. Yeni bir özellik (Feature) eklerken aşağıdaki katman kurallarına **kesinlikle** uyulmalıdır:

### A. Application Katmanı (CQRS & Feature-Sliced Yapı)
Her yeni işlev `src/OleAI.Backend.Application/Features/<FeatureName>/` altında ilgili klasörlere ayrılmalıdır:
1. **`Commands/` veya `Queries/`:**
   * Her Command/Query kendi alt klasörüne sahip olmalıdır (Örn: `Commands/CreateIngredient/CreateIngredientCommand.cs` ve `CreateIngredientCommandHandler.cs`).
2. **`Rules/` (İş Kuralları):**
   * İş mantığı kontrolleri doğrudan Handler içine gömülmemeli, `<Feature>BusinessRules.cs` sınıfı içerisine yazılmalı ve hata durumunda `BusinessException` veya `NotFoundException` fırlatılmalıdır.
3. **`Validations/` (FluentValidation):**
   * Kullanıcıdan gelen her Command için mutlaka `AbstractValidator<TCommand>` sınıfı yazılmalıdır. `ValidationBehavior` pipeline'ı bunu otomatik olarak çalıştıracaktır.
4. **`Constants/`:**
   * Döndürülen hata veya bilgi mesajları magic string olarak yazılmamalı, `<Feature>Messages.cs` içerisinden sabit olarak çağrılmalıdır.
5. **`Profiles/`:**
   * Entity <-> DTO dönüşümleri için AutoMapper profilleri ilgili özelliğin `Profiles/` klasöründe tanımlanmalıdır.

### B. Presentation Katmanı (Controllers)
* Controller'lar içerisinde **kesinlikle iş mantığı (business logic) bulunmamalıdır.**
* Controller'lar `BaseController` sınıfından türemeli ve sadece `Mediator.Send(...)` çağrısı yaparak uygun HTTP durum kodunu (`Ok`, `Created`, `NoContent`) dönmelidir.
* Hata yönetimi için `try-catch` blokları **kullanılmamalıdır**; tüm hatalar `ExceptionMiddleware` tarafından yakalanıp standart `ProblemDetails` formatında döndürülmektedir.

### C. Güvenlik Kuralları
* **Asla** API anahtarı, parola, bağlantı cümlesi veya `firebase-adminsdk.json` gibi hassas dosyaları koda gömmeyin ve commit etmeyin.
* Yeni bir dış servis ayar sınıfı eklediğinizde (`*Settings.cs`), örnek anahtarlarını mutlaka `.env.example` ve `appsettings.json` (değeri boş olarak) dosyalarına ekleyin.

---

## 4. Birim Testi (Unit Test) Zorunluluğu

OleAI Backend projesinde yüksek test kapsamı (Code Coverage) kritik öneme sahiptir.
* Yazdığınız her yeni **CommandHandler**, **QueryHandler**, **BusinessRules** ve **Validator** için `tests/OleAI.Backend.Application.Tests/` altında karşılık gelen test sınıfını oluşturmalısınız.
* Test metod isimlendirmelerinde **`MethodName_StateUnderTest_ExpectedBehavior`** veya anlaşılır senaryo isimlendirmeleri kullanın.
* Bağımlılıkları izole etmek için `Moq` (veya projede kullanılan mocking kütüphanesini) kullanın.

---

## 5. Pull Request (PR) Süreci

1. Kodunuzu tamamladıktan ve yerel testlerinizi (`dotnet test`) başarıyla geçirdikten sonra dalınızı (branch) kendi fork'unuza push edin:
   ```bash
   git push origin feat/ozellik-adi
   ```
2. GitHub üzerinden ana repoya bir **Pull Request (PR)** açın.
3. PR açıklamasında:
   * Ne değiştirdiğinizi ve neden değiştirdiğinizi özetleyin.
   * Varsa ilgili Issue numarasını (`Closes #12` gibi) belirtin.
   * Yeni bir `.env` değişkeni eklediyseniz bunu PR açıklamasında açıkça vurgulayın.
4. GitHub Actions (`backend-ci.yml`) üzerindeki tüm derleme ve test adımlarının yeşil (başarılı) olduğundan emin olun.

<div align="right">

[⬆ Başa dön](#) &nbsp;|&nbsp; [🇬🇧 İngilizce sürüme git](#en)

</div>

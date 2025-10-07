# 📅 Terminplaner

[![CI Status](https://github.com/mistalan/Terminplaner/workflows/CI%20-%20Build%20and%20Test/badge.svg)](https://github.com/mistalan/Terminplaner/actions/workflows/ci.yml)
[![CodeQL](https://img.shields.io/badge/CodeQL-enabled-success?logo=github)](https://github.com/mistalan/Terminplaner/security/code-scanning)
[![Code Coverage](https://github.com/mistalan/Terminplaner/workflows/Code%20Coverage/badge.svg)](https://github.com/mistalan/Terminplaner/actions/workflows/coverage.yml)
[![Docker](https://github.com/mistalan/Terminplaner/workflows/Docker%20Build%20and%20Push/badge.svg)](https://github.com/mistalan/Terminplaner/actions/workflows/docker.yml)
[![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/github/license/mistalan/Terminplaner)](LICENSE)

Moderne Terminplaner-App für die Verwaltung von Terminen mit Kategorien und Prioritäten. Entwickelt als Alternative zum Word-Dokument-System - mit allen vertrauten Features, aber in einer modernen Cross-Platform-App die auf PC, Android, iOS und mehr funktioniert!

## 📱 Plattformen

- ✅ **Android** - Native Android App
- ✅ **iOS** - Native iOS App  
- ✅ **Windows** - Native Windows Desktop App
- ✅ **macOS** - Native Mac App (via Mac Catalyst)

## ✨ Features

- ✅ **Termine erstellen** - Füge neue Termine mit Beschreibung hinzu
- 🏷️ **Kategorien** - Organisiere Termine mit farbigen Kategorien (z.B. Arbeit, Privat, Gesundheit, Familie)
- 🎨 **Farbcodierung** - Jeder Kategorie eine eigene Farbe zuweisen
- 🔢 **Prioritäten** - Termine mit Pfeil-Buttons neu sortieren (oben = höchste Priorität)
- ✏️ **Bearbeiten** - Bestehende Termine ändern
- 🗑️ **Löschen** - Erledigte Termine einfach entfernen
- 📱 **Cross-Platform** - Eine Codebasis für alle Plattformen
- 🎯 **Native Performance** - Echte native Apps, keine Web-Wrapper
- 🚀 **Schnell** - Direkte API-Kommunikation mit dem Backend

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core 9.0 Web API (C#)
- **Frontend**: .NET MAUI (Multi-platform App UI) mit XAML
- **Pattern**: MVVM (Model-View-ViewModel) mit CommunityToolkit.Mvvm
- **Datenspeicher**: Repository Pattern mit In-Memory (Standard) oder Azure Cosmos DB (optional)
- **API**: RESTful API mit JSON
- **Deployment**: Android, iOS, Windows, macOS

## 📋 Voraussetzungen

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) oder höher
- Für Android-Entwicklung: Android SDK (kommt mit Visual Studio oder kann separat installiert werden)
- Für iOS-Entwicklung: macOS mit Xcode
- Empfohlen: Visual Studio 2022 oder Visual Studio Code mit C# Extension

## 🚀 Installation & Start

### Backend starten

1. **Repository klonen:**
   ```bash
   git clone https://github.com/mistalan/Terminplaner.git
   cd Terminplaner/TerminplanerApi
   ```

2. **API starten:**
   ```bash
   dotnet run
   ```
   
   Die API läuft nun auf http://localhost:5215

### MAUI App starten

#### Für Android (empfohlen für Entwicklung):

1. **Android Emulator starten** oder Android-Gerät per USB verbinden

2. **App starten:**
   ```bash
   cd ../TerminplanerMaui
   dotnet build -t:Run -f net9.0-android
   ```

#### Für Windows:

```bash
cd ../TerminplanerMaui
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

#### Für iOS (nur auf macOS):

```bash
cd ../TerminplanerMaui
dotnet build -t:Run -f net9.0-ios
```

#### Für macOS (nur auf macOS):

```bash
cd ../TerminplanerMaui
dotnet build -t:Run -f net9.0-maccatalyst
```

### Mit Visual Studio

1. Öffne die Solution in Visual Studio 2022
2. Stelle sicher, dass beide Projekte (TerminplanerApi und TerminplanerMaui) in der Solution sind
3. Setze TerminplanerApi als Startup-Projekt und starte es
4. Setze TerminplanerMaui als Startup-Projekt und wähle die Zielplattform (Android, iOS, Windows)
5. Starte die App

**Wichtig:** Die Backend-API muss laufen, damit die MAUI-App funktioniert!

## 📖 Verwendung

### Termin hinzufügen
1. Gib den Termintext im Feld "Neuer Termin..." ein
2. Optional: Gib eine Kategorie ein (z.B. "Arbeit", "Privat", "Gesundheit")
3. Optional: Gib eine Farbe ein (z.B. "#FF0000" für Rot)
4. Tippe auf "➕ Hinzufügen"

### Termin bearbeiten
1. Tippe auf "✏️" bei einem Termin
2. Ändere Text, Kategorie oder Farbe
3. Tippe auf "💾 Speichern"

### Termin löschen
1. Tippe auf "🗑️" bei einem Termin
2. Bestätige die Löschung

### Priorität ändern
- Nutze die ⬆️ und ⬇️ Buttons um Termine nach oben oder unten zu verschieben
- Die Position bestimmt die Priorität (oben = höchste Priorität)

## 🔌 API Endpoints

Die Anwendung bietet folgende REST API Endpoints:

| Method | Endpoint | Beschreibung |
|--------|----------|--------------|
| GET | `/api/appointments` | Alle Termine abrufen |
| GET | `/api/appointments/{id}` | Einzelnen Termin abrufen |
| POST | `/api/appointments` | Neuen Termin erstellen |
| PUT | `/api/appointments/{id}` | Termin aktualisieren |
| DELETE | `/api/appointments/{id}` | Termin löschen |
| PUT | `/api/appointments/priorities` | Prioritäten aktualisieren |

### Beispiel API Aufruf:

```bash
# Alle Termine abrufen
curl http://localhost:5215/api/appointments

# Neuen Termin erstellen
curl -X POST http://localhost:5215/api/appointments \
  -H "Content-Type: application/json" \
  -d '{"text":"Meeting","category":"Arbeit","color":"#0000FF"}'
```

## 🏗️ Projekt-Struktur

```
Terminplaner/
├── .github/
│   ├── workflows/          # CI/CD Workflows (CI, Coverage, CodeQL, Docker, etc.)
│   ├── dependabot.yml      # Dependency update configuration
│   ├── labels.yml          # GitHub labels configuration
│   ├── release-drafter.yml # Release notes configuration
│   └── copilot-instructions.md
│
├── TerminplanerApi/              # Backend Web API
│   ├── Models/
│   │   └── Appointment.cs        # Datenmodell für Termine
│   ├── Repositories/
│   │   ├── IAppointmentRepository.cs # Repository-Interface
│   │   ├── InMemoryAppointmentRepository.cs # In-Memory Implementierung
│   │   └── CosmosAppointmentRepository.cs # Azure Cosmos DB Implementierung
│   ├── Program.cs                # API-Konfiguration & Endpoints
│   └── TerminplanerApi.csproj    # Backend-Projekt-Datei
│
├── TerminplanerApi.Tests/        # Tests für Backend API
│   ├── AppointmentRepositoryTests.cs      # Unit Tests (23)
│   ├── AppointmentApiIntegrationTests.cs # Integration Tests (19)
│   ├── TEST_CASES.md             # Detaillierte Test-Dokumentation
│   └── TerminplanerApi.Tests.csproj    # Test-Projekt-Datei
│
├── TerminplanerMaui/             # Cross-Platform MAUI App
│   ├── Models/
│   │   └── Appointment.cs        # Client-seitiges Datenmodell
│   ├── Services/
│   │   └── AppointmentApiService.cs # API-Client
│   ├── ViewModels/
│   │   ├── MainViewModel.cs      # Haupt-ViewModel (MVVM)
│   │   └── EditAppointmentViewModel.cs # Edit-ViewModel
│   ├── Pages/
│   │   ├── MainPage.xaml         # Hauptseite UI
│   │   ├── EditAppointmentPage.xaml # Edit-Seite UI
│   │   └── *.xaml.cs             # Code-behind Dateien
│   ├── Platforms/                # Plattform-spezifischer Code
│   │   ├── Android/, iOS/, Windows/, MacCatalyst/
│   ├── Resources/                # App-Ressourcen (Icons, Fonts, Styles)
│   ├── App.xaml                  # App-Definition
│   ├── AppShell.xaml             # Shell/Navigation
│   └── TerminplanerMaui.csproj   # MAUI-Projekt-Datei
│
├── Dockerfile                    # Docker-Konfiguration für API
├── docker-compose.yml            # Docker Compose Setup
└── README.md                     # Diese Datei
```

## 🧪 Tests

Das Projekt enthält umfassende Tests für die Backend-API:

- **42 Tests** (100% bestanden)
  - **23 Unit Tests** für AppointmentService
  - **19 Integration Tests** für API Endpoints

**Tests ausführen:**
```bash
dotnet test
```

**Mit Code Coverage:**
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

**Detaillierte Test-Dokumentation:** Siehe [TerminplanerApi.Tests/TEST_CASES.md](TerminplanerApi.Tests/TEST_CASES.md)

## 💾 Persistence Architecture

Das Projekt verwendet das **Repository Pattern** mit Abstraktionsschicht für flexible Datenspeicherung:

- ✅ **In-Memory Repository** (Standard) - Für Entwicklung und Tests
- ✅ **Azure Cosmos DB Repository** - Für Produktion (optional)
- ✅ **Einfache Konfiguration** - Umschalten via `appsettings.json`
- ✅ **String-basierte IDs** - Kompatibel mit Cosmos DB und anderen NoSQL-Datenbanken

**Dokumentation:**
- [PERSISTENCE_ARCHITECTURE.md](PERSISTENCE_ARCHITECTURE.md) - Repository Pattern und Architektur
- [SECURITY_CONFIGURATION.md](SECURITY_CONFIGURATION.md) - Sichere Konfiguration von Credentials
- [CODESPACES_SETUP.md](CODESPACES_SETUP.md) - GitHub Codespaces Konfiguration
- [AZURE_KEYVAULT_SETUP.md](AZURE_KEYVAULT_SETUP.md) - Azure Key Vault Setup und GitHub Actions Integration
- [GIT_HISTORY_CLEANUP.md](GIT_HISTORY_CLEANUP.md) - Git History bereinigen (bei versehentlich committeten Secrets)
- [PERSISTENCY_EVALUATION.md](PERSISTENCY_EVALUATION.md) - Umfassende Analyse von NoSQL-Datenbanken

## 🔮 Zukünftige Erweiterungen

- 🔔 Benachrichtigungen und Erinnerungen
- 📅 Kalenderintegration
- 👥 Mehrbenutzer-Unterstützung
- 🌐 Cloud-Synchronisation
- 🔍 Such- und Filterfunktionen
- 📊 Statistiken und Berichte
- 🌍 Mehrsprachigkeit
- 🎨 Anpassbare Themes

## 🎯 Warum .NET MAUI?

.NET MAUI ermöglicht es, mit einer einzigen C#/XAML-Codebasis native Apps für alle Plattformen zu erstellen:

- **Echte native Apps** - Keine Web-Wrapper, sondern echte native UI-Controls
- **Beste Performance** - Native Kompilierung für jede Plattform
- **Code-Sharing** - 90%+ Code-Sharing zwischen allen Plattformen
- **Produktivität** - XAML Hot Reload für schnelle Entwicklung
- **Zukunftssicher** - Microsoft's neueste Cross-Platform-Technologie
- **Android-Ready** - Einfaches Deployment auf Android-Geräte

## 🔧 Deployment & CI/CD

Das Projekt verfügt über automatisierte GitHub Actions Workflows für:

### CI/CD Workflows

| Workflow | Trigger | Beschreibung |
|----------|---------|--------------|
| **CI - Build and Test** | Push/PR to main | Baut API, führt alle 42 Tests aus, prüft Code-Formatierung |
| **Code Coverage** | Push/PR to main | Generiert Coverage-Reports (erfordert optional `CODECOV_TOKEN` Secret) |
| **CodeQL Security** | Push/PR to main, Weekly | Sicherheitsanalyse für C# Code (GitHub Default Setup) |
| **Docker Build** | Push to main, Tags | Baut und pusht Docker Image zu GHCR |
| **Release Drafter** | Push to main, PRs | Erstellt automatisch Release-Notizen |
| **Label Sync** | Push to main (labels.yml), Manual | Synchronisiert Repository-Labels |
| **Stale Bot** | Täglich | Schließt inaktive Issues/PRs nach 60/30 Tagen |
| **Deploy API** | Tags `v*.*.*`, Manual | Erstellt Linux/Windows API Packages |
| **Deploy Android** | Tags `android-v*.*.*`, Manual | Baut APK für Android |
| **Deploy Windows** | Tags `windows-v*.*.*`, Manual | Baut Windows Desktop App |

### GitHub Apps & Tools

Das Projekt nutzt folgende integrierte GitHub Features:

- ✅ **Dependabot** - Automatische Dependency-Updates für NuGet und GitHub Actions
- ✅ **CodeQL** - Sicherheitsscan für Schwachstellen und Code-Fehler (GitHub Default Setup)
- ✅ **Release Drafter** - Automatische Generierung von Release Notes
- ✅ **Label Sync** - Automatische Synchronisierung von Repository-Labels
- ✅ **Code Coverage** - Test Coverage Reporting (Cobertura XML)
- ✅ **Docker CI/CD** - Automatische Docker Builds zu GitHub Container Registry
- ✅ **Stale Bot** - Verwaltung inaktiver Issues und Pull Requests

**Empfohlene Marketplace Apps (Optional):**
- **Codecov** - Visuelles Coverage-Reporting (erfordert CODECOV_TOKEN Secret)
- **SonarCloud** - Code-Qualitätsanalyse (erfordert SONAR_TOKEN Secret)

### Schnell-Deployment

**Mit Docker (Empfohlen):**
```bash
# Docker Compose verwenden
docker-compose up
# API läuft auf http://localhost:5215

# Oder mit Docker direkt
docker build -t terminplaner-api .
docker run -p 5215:5215 terminplaner-api
```

**Docker Image von GHCR:**
```bash
docker pull ghcr.io/mistalan/terminplaner/api:latest
docker run -p 5215:5215 ghcr.io/mistalan/terminplaner/api:latest
```

**Android APK erstellen:**
```bash
# Über GitHub Actions (empfohlen):
# 1. Gehe zu Actions → Deploy Android App → Run workflow
# 2. Lade APK-Artifact herunter

# Oder lokal:
cd TerminplanerMaui
dotnet publish -f net9.0-android -c Release
```

**API manuell deployen:**
```bash
cd TerminplanerApi
dotnet publish -c Release -o ./publish
# Publish-Ordner auf Server kopieren
```

### Konfiguration

**Benötigte Secrets (alle optional):**
- `CODECOV_TOKEN` - Für Codecov Coverage-Uploads (nur wenn Codecov App installiert)
- Signing-Secrets für Android/Windows sind für spätere Store-Veröffentlichung vorbereitet (aktuell nicht benötigt)

## 📚 Dokumentation

Das Projekt enthält umfassende Dokumentation in den folgenden Dateien:

- **[PERSISTENCY_EVALUATION.md](PERSISTENCY_EVALUATION.md)** - Umfassende Evaluation von NoSQL-Datenbanken und Cloud-Services für persistente Datenspeicherung
- **[USER_GUIDE.md](USER_GUIDE.md)** - Benutzerhandbuch für die Terminplaner-App
- **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Zusammenfassung der Legacy-Scheduler-Feature-Implementierung
- **[UI_MOCKUP.md](UI_MOCKUP.md)** - UI-Mockups und Design-Spezifikationen
- **[UI_CHANGES.md](UI_CHANGES.md)** - Dokumentation der UI-Änderungen
- **[TerminplanerApi.Tests/TEST_CASES.md](TerminplanerApi.Tests/TEST_CASES.md)** - Detaillierte Test-Dokumentation

## 📝 Lizenz

Dieses Projekt ist für den privaten Gebrauch bestimmt.

## 👤 Autor

Erstellt für meine Schwester - damit sie ihr Word-System endlich in Rente schicken kann! 😊

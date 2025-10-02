# 📅 Terminplaner

Moderne Terminplaner-App für die Verwaltung von Terminen mit Kategorien und Prioritäten.

Entwickelt als Alternative zum Word-Dokument-System - mit allen vertrauten Features, aber in einer modernen Cross-Platform-App die auf PC, Android, iOS und mehr funktioniert!

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
- **Datenspeicher**: In-Memory (Liste) im Backend
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
├── TerminplanerApi/              # Backend Web API
│   ├── Models/
│   │   └── Appointment.cs        # Datenmodell für Termine
│   ├── Services/
│   │   └── AppointmentService.cs # Business-Logik & In-Memory Speicher
│   ├── Program.cs                # API-Konfiguration & Endpoints
│   └── TerminplanerApi.csproj    # Backend-Projekt-Datei
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
│   │   ├── MainPage.xaml.cs      # Code-behind
│   │   ├── EditAppointmentPage.xaml # Edit-Seite UI
│   │   └── EditAppointmentPage.xaml.cs # Code-behind
│   ├── Platforms/
│   │   ├── Android/              # Android-spezifischer Code
│   │   ├── iOS/                  # iOS-spezifischer Code
│   │   ├── Windows/              # Windows-spezifischer Code
│   │   └── MacCatalyst/          # macOS-spezifischer Code
│   ├── Resources/
│   │   ├── Images/               # App-Icons und Bilder
│   │   ├── Fonts/                # Schriftarten
│   │   └── Styles/               # XAML-Styles
│   ├── App.xaml                  # App-Definition
│   ├── AppShell.xaml             # Shell/Navigation
│   ├── MauiProgram.cs            # App-Initialisierung
│   └── TerminplanerMaui.csproj   # MAUI-Projekt-Datei
│
├── README.md                     # Diese Datei
└── QUICKSTART.md                 # Schnellstart-Anleitung
```

## 🔮 Zukünftige Erweiterungen

- 💾 Persistente Datenspeicherung (SQLite/Datenbank)
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

## 🔧 Android Deployment

Um die App auf einem Android-Gerät zu installieren:

1. **Debug-Build erstellen:**
   ```bash
   cd TerminplanerMaui
   dotnet publish -f net9.0-android -c Release
   ```

2. **APK finden:**
   Die APK-Datei befindet sich unter:
   ```
   bin/Release/net9.0-android/publish/
   ```

3. **Auf Gerät installieren:**
   - Kopiere die APK auf dein Android-Gerät
   - Aktiviere "Installation aus unbekannten Quellen" in den Einstellungen
   - Installiere die APK

Für Play Store Deployment ist ein signiertes Release-Build erforderlich.

## 📝 Lizenz

Dieses Projekt ist für den privaten Gebrauch bestimmt.

## 👤 Autor

Erstellt für meine Schwester - damit sie ihr Word-System endlich in Rente schicken kann! 😊

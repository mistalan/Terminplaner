# 🚀 Schnellstart-Anleitung

## Voraussetzungen

1. Stelle sicher, dass .NET 9.0 SDK installiert ist:
   ```bash
   dotnet --version
   ```

2. Klone das Repository:
   ```bash
   git clone https://github.com/mistalan/Terminplaner.git
   cd Terminplaner
   ```

## Schritt 1: Backend API starten

```bash
cd TerminplanerApi
dotnet run
```

Die API startet auf: **http://localhost:5215**

Lasse dieses Terminal-Fenster offen!

## Schritt 2: MAUI App starten

Öffne ein **neues Terminal** und:

### Für Android (Empfohlen):

```bash
cd TerminplanerMaui
dotnet build -t:Run -f net9.0-android
```

**Hinweis:** Du benötigst einen laufenden Android Emulator oder ein angeschlossenes Android-Gerät.

### Für Windows:

```bash
cd TerminplanerMaui
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

### Für iOS (nur macOS):

```bash
cd TerminplanerMaui
dotnet build -t:Run -f net9.0-ios
```

## Erste Schritte

1. Die App öffnet sich automatisch nach dem Start
2. Du siehst bereits 3 Beispiel-Termine (aus der API)
3. Füge einen neuen Termin hinzu:
   - Gib einen Text ein
   - Gib optional eine Kategorie ein
   - Gib optional eine Farbe ein (z.B. #FF0000)
   - Tippe auf "Hinzufügen"

## Hauptfunktionen

| Funktion | Anleitung |
|----------|-----------|
| **Termin hinzufügen** | Text eingeben → Kategorie eingeben → Farbe eingeben → "Hinzufügen" tippen |
| **Termin bearbeiten** | "✏️" Button tippen → Änderungen vornehmen → "Speichern" |
| **Termin löschen** | "🗑️" Button tippen → Bestätigen |
| **Priorität ändern** | "⬆️" oder "⬇️" Buttons verwenden |

## Tipps

- **Kategorien**: Nutze konsistente Namen wie "Arbeit", "Privat", "Gesundheit", "Familie"
- **Farben**: Verwende Hex-Codes wie #FF0000 (Rot), #00FF00 (Grün), #0000FF (Blau)
- **Priorität**: Termine oben = höchste Priorität, unten = niedrigste Priorität
- **Pull to Refresh**: Ziehe die Liste nach unten, um zu aktualisieren

## Architektur

Die App verwendet ein Client-Server-Modell:

```
┌─────────────────────┐
│   MAUI App          │  ← Cross-Platform UI (Android, iOS, Windows, Mac)
│   (Port variiert)   │
└──────────┬──────────┘
           │ HTTP/JSON
           ↓
┌─────────────────────┐
│   ASP.NET Web API   │  ← Backend Server
│   (Port 5215)       │
└─────────────────────┘
```

## Android Spezifisches

Wenn du auf Android entwickelst:
- Der Emulator verwendet `10.0.2.2` statt `localhost` für die API
- Dies ist bereits in der `AppointmentApiService.cs` konfiguriert
- Du kannst den Emulator mit Android Studio oder Visual Studio starten

## Probleme?

### API startet nicht
- Überprüfe, ob der Port 5215 frei ist
- Stelle sicher, dass .NET 9.0 SDK korrekt installiert ist
- Führe `dotnet restore` aus im TerminplanerApi Ordner

### MAUI App startet nicht
- Stelle sicher, dass die API läuft (Schritt 1)
- Für Android: Stelle sicher, dass ein Emulator läuft oder ein Gerät verbunden ist
- Für Windows: Stelle sicher, dass du Windows 10/11 verwendest
- Versuche `dotnet clean` und dann `dotnet build` erneut

### App kann API nicht erreichen
- Stelle sicher, dass die API läuft
- Auf Android Emulator: Die App nutzt automatisch `10.0.2.2` statt `localhost`
- Überprüfe Firewall-Einstellungen

## Visual Studio Nutzer

1. Öffne die Solution in Visual Studio 2022
2. Klicke mit der rechten Maustaste auf die Solution → "Set Startup Projects"
3. Wähle "Multiple startup projects"
4. Setze beide Projekte auf "Start":
   - TerminplanerApi
   - TerminplanerMaui
5. Wähle die Zielplattform für TerminplanerMaui (z.B. Android Emulator)
6. Drücke F5 zum Starten

## API verwenden

Die Backend-API kann auch direkt genutzt werden:

```bash
# Alle Termine abrufen
curl http://localhost:5215/api/appointments

# Neuen Termin erstellen
curl -X POST http://localhost:5215/api/appointments \
  -H "Content-Type: application/json" \
  -d '{"text":"Einkaufen","category":"Privat","color":"#00FF00"}'
```

## Weitere Informationen

- [README.md](README.md) - Vollständige Dokumentation
- **MAUI Dokumentation**: https://learn.microsoft.com/dotnet/maui/
- **ASP.NET Core Dokumentation**: https://learn.microsoft.com/aspnet/core/

## Was ist .NET MAUI?

.NET MAUI (Multi-platform App UI) ist Microsoft's neueste Cross-Platform-Framework:

- Eine Codebasis für Android, iOS, macOS und Windows
- Nutzt native UI-Controls für beste Performance
- XAML für deklarative UIs
- MVVM-Pattern mit Data Binding
- Hot Reload für schnelle Entwicklung

Dies ist der empfohlene Weg für moderne .NET Mobile- und Desktop-Apps!

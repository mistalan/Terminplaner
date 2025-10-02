# 📅 Terminplaner

Moderne Terminplaner-App für die Verwaltung von Terminen mit Kategorien und Prioritäten.

Entwickelt als Alternative zum Word-Dokument-System - mit allen vertrauten Features, aber in einer modernen Web-App die auf PC und Handy funktioniert.

## Screenshots

![Terminplaner Hauptansicht](https://github.com/user-attachments/assets/ca5b80c9-8323-4ff7-9c8d-b5e0d81f264a)

![Neuen Termin hinzufügen](https://github.com/user-attachments/assets/10b8283e-0830-47cd-a5a6-29106636c3e6)

![Termin bearbeiten](https://github.com/user-attachments/assets/2c1f0c82-beed-4493-98d4-8268361db21a)

## ✨ Features

- ✅ **Termine erstellen** - Füge neue Termine mit Beschreibung hinzu
- 🏷️ **Kategorien** - Organisiere Termine mit farbigen Kategorien (z.B. Arbeit, Privat, Gesundheit, Familie)
- 🎨 **Farbcodierung** - Jeder Kategorie eine eigene Farbe zuweisen
- 🔢 **Prioritäten** - Termine per Drag & Drop neu sortieren (oben = höchste Priorität)
- ✏️ **Bearbeiten** - Bestehende Termine ändern
- 🗑️ **Löschen** - Erledigte Termine einfach entfernen
- 📱 **Responsive** - Funktioniert auf Desktop und Mobilgeräten
- 🚀 **Schnell** - Keine Datenbank nötig, läuft direkt im Speicher

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core 9.0 Web API (C#)
- **Frontend**: HTML5, CSS3, JavaScript (Vanilla)
- **Datenspeicher**: In-Memory (Liste)
- **API**: RESTful API mit JSON

## 📋 Voraussetzungen

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) oder höher

## 🚀 Installation & Start

1. **Repository klonen:**
   ```bash
   git clone https://github.com/mistalan/Terminplaner.git
   cd Terminplaner/TerminplanerApi
   ```

2. **Anwendung starten:**
   ```bash
   dotnet run
   ```

3. **Browser öffnen:**
   - Öffne http://localhost:5215 in deinem Browser
   - Die Anwendung läuft automatisch auf Port 5215

## 📖 Verwendung

### Termin hinzufügen
1. Gib den Termintext im Feld "Neuer Termin..." ein
2. Optional: Gib eine Kategorie ein (z.B. "Arbeit", "Privat", "Gesundheit")
3. Optional: Wähle eine Farbe aus
4. Klicke auf "➕ Hinzufügen"

### Termin bearbeiten
1. Klicke auf "✏️ Bearbeiten" bei einem Termin
2. Ändere Text, Kategorie oder Farbe
3. Klicke auf "💾 Speichern"

### Termin löschen
1. Klicke auf "🗑️ Löschen" bei einem Termin
2. Bestätige die Löschung

### Priorität ändern
- Ziehe einen Termin mit der Maus (am ⋮⋮ Symbol) an eine andere Position
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
TerminplanerApi/
├── Models/
│   └── Appointment.cs          # Datenmodell für Termine
├── Services/
│   └── AppointmentService.cs   # Business-Logik & In-Memory Speicher
├── wwwroot/
│   ├── index.html              # Frontend HTML
│   ├── styles.css              # Styling
│   └── app.js                  # Frontend JavaScript
├── Program.cs                  # API-Konfiguration & Endpoints
└── TerminplanerApi.csproj      # Projekt-Datei
```

## 🔮 Zukünftige Erweiterungen

- 💾 Persistente Datenspeicherung (SQLite/JSON-Datei)
- 📱 Native Mobile App (iOS/Android)
- 🔔 Benachrichtigungen und Erinnerungen
- 📅 Kalenderintegration
- 👥 Mehrbenutzer-Unterstützung
- 🌐 Cloud-Synchronisation
- 🔍 Such- und Filterfunktionen
- 📊 Statistiken und Berichte

## 📝 Lizenz

Dieses Projekt ist für den privaten Gebrauch bestimmt.

## 👤 Autor

Erstellt für meine Schwester - damit sie ihr Word-System endlich in Rente schicken kann! 😊

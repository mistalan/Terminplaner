# 🚀 Schnellstart-Anleitung

## Installation

1. Stelle sicher, dass .NET 9.0 SDK installiert ist:
   ```bash
   dotnet --version
   ```

2. Klone das Repository:
   ```bash
   git clone https://github.com/mistalan/Terminplaner.git
   cd Terminplaner
   ```

## Anwendung starten

```bash
cd TerminplanerApi
dotnet run
```

Die Anwendung startet auf: **http://localhost:5215**

## Erste Schritte

1. Öffne http://localhost:5215 im Browser
2. Du siehst bereits 3 Beispiel-Termine
3. Füge einen neuen Termin hinzu:
   - Gib einen Text ein
   - Wähle optional eine Kategorie
   - Wähle optional eine Farbe
   - Klicke auf "Hinzufügen"

## Hauptfunktionen

| Funktion | Anleitung |
|----------|-----------|
| **Termin hinzufügen** | Text eingeben → Kategorie eingeben → Farbe wählen → "Hinzufügen" |
| **Termin bearbeiten** | "Bearbeiten" Button klicken → Änderungen vornehmen → "Speichern" |
| **Termin löschen** | "Löschen" Button klicken → Bestätigen |
| **Priorität ändern** | Termin am ⋮⋮ Symbol packen und verschieben |

## Tipps

- **Kategorien**: Nutze konsistente Namen wie "Arbeit", "Privat", "Gesundheit", "Familie"
- **Farben**: Verwende für jede Kategorie die gleiche Farbe zur besseren Übersicht
- **Priorität**: Termine oben = höchste Priorität, unten = niedrigste Priorität
- **Mobil**: Die App funktioniert auch auf Smartphones und Tablets

## Probleme?

Wenn die Anwendung nicht startet:

1. Überprüfe, ob der Port 5215 frei ist
2. Stelle sicher, dass .NET 9.0 SDK korrekt installiert ist
3. Führe `dotnet restore` aus, bevor du `dotnet run` ausführst

## API verwenden

Du kannst die API auch direkt nutzen:

```bash
# Alle Termine abrufen
curl http://localhost:5215/api/appointments

# Neuen Termin erstellen
curl -X POST http://localhost:5215/api/appointments \
  -H "Content-Type: application/json" \
  -d '{"text":"Einkaufen","category":"Privat","color":"#00FF00"}'
```

## Weitere Informationen

Siehe [README.md](README.md) für die vollständige Dokumentation.

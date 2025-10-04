# Terminplaner UI - Before and After

## BEFORE (Original UI)

```
┌─────────────────────────────────────────────┐
│         📅 Terminplaner                     │
│    Termine einfach verwalten...             │
├─────────────────────────────────────────────┤
│ [Neuer Termin...]                           │
│ [Kategorie...]                              │
│ Farbe: [#808080] ■                          │
│ [➕ Hinzufügen]                              │
├─────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────┐ │
│ │ │ Zahnarzttermin                        │ │
│ │ │ 🏷️ Gesundheit                         │ │
│ │ │ Priorität: 1                          │ │
│ │ │                          [⬆️][⬇️]      │ │
│ │ │                          [✏️][🗑️]      │ │
│ └─────────────────────────────────────────┘ │
│ ┌─────────────────────────────────────────┐ │
│ │ │ Projekt abschließen                   │ │
│ │ │ 🏷️ Arbeit                             │ │
│ │ │ Priorität: 2                          │ │
│ │ │                          [⬆️][⬇️]      │ │
│ │ │                          [✏️][🗑️]      │ │
│ └─────────────────────────────────────────┘ │
└─────────────────────────────────────────────┘
```

## AFTER (New UI - Matching Legacy)

```
┌─────────────────────────────────────────────┐
│ ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓ │
│ ┃  Samstag, 4. Oktober 2025            ┃ │  ← NEW: Current Date
│ ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛ │
├─────────────────────────────────────────────┤
│ [Neuer Termin...]                           │
│ [Kategorie...]                              │
│ Farbe: [#808080] ■                          │
│ [📅 Datum auswählen]              ← NEW     │
│ [Dauer (z.B. 30 min, 1-2 Std)]   ← NEW     │
│ ☐ Außer Haus / Unterwegs          ← NEW     │
│ [➕ Hinzufügen]                              │
├─────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────┐ │
│ │ │ Zahnarzttermin                        │ │
│ │ │ 🏷️ Gesundheit                         │ │
│ │ │ 🕒 10:00 Uhr              ← NEW        │ │
│ │ │ ⏱️ 1 Std                  ← NEW        │ │
│ │ │                          [⬆️][⬇️]      │ │
│ │ │                          [✏️][🗑️]      │ │
│ └─────────────────────────────────────────┘ │
│ ┌─────────────────────────────────────────┐ │
│ │ │ Projekt abschließen                   │ │
│ │ │ 🏷️ Arbeit                             │ │
│ │ │ 🕒 14:00 Uhr              ← NEW        │ │
│ │ │ ⏱️ 2-3 Std                ← NEW        │ │
│ │ │                          [⬆️][⬇️]      │ │
│ │ │                          [✏️][🗑️]      │ │
│ └─────────────────────────────────────────┘ │
│ ╔═════════════════════════════════════════╗ │  ← NEW: Green background
│ ║ │ Lebensmittel einkaufen               ║ │     for out-of-home
│ ║ │ 🏷️ Privat                            ║ │
│ ║ │ 🕒 16:00 Uhr              ← NEW       ║ │
│ ║ │ ⏱️ 30 min                 ← NEW       ║ │
│ ║ │ 📍 Außer Haus            ← NEW       ║ │
│ ║ │                          [⬆️][⬇️]     ║ │
│ ║ │                          [✏️][🗑️]     ║ │
│ ╚═════════════════════════════════════════╝ │
└─────────────────────────────────────────────┘
```

## Key UI Changes Summary

### 1. Date Display (Top of Page)
- **Color:** Blue background (#667eea)
- **Format:** German long date format (e.g., "Samstag, 4. Oktober 2025")
- **Purpose:** Match legacy scheduler's date header

### 2. Add Appointment Form
- **New Field:** DatePicker for scheduled date
- **New Field:** Duration entry (free text)
- **New Field:** Checkbox for "Außer Haus" (out of home)

### 3. Appointment Cards
#### Standard Appointment (White background)
- Shows time if scheduled (🕒 10:00 Uhr)
- Shows duration if set (⏱️ 1 Std)
- No special indicator

#### Out-of-Home Appointment (Green background)
- **Background:** Light green (#C8E6C9)
- **Indicator:** 📍 Außer Haus label in bold green
- Shows time if scheduled (🕒 16:00 Uhr)
- Shows duration if set (⏱️ 30 min)
- Matches legacy green highlighting

### 4. Conditional Rendering
- Time/Duration only shown if values are set
- "Außer Haus" indicator only shown when IsOutOfHome = true
- Green background only applied when IsOutOfHome = true
- All fields are optional for backward compatibility

## Legend
```
┌─┐  Regular border (white background)
╔═╗  Double border (green background for out-of-home)
■    Color indicator box
🕒   Time icon
⏱️   Duration icon
📍   Location/out-of-home icon
🏷️   Category icon
```

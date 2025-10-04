# User Guide - Legacy Scheduler Features

This guide explains how to use the new features that match the legacy scheduler functionality.

## New Features Overview

The Terminplaner app now supports the following features from the legacy scheduler:

1. **Current Date Display** - See today's date at the top
2. **Scheduled Time** - Set when an appointment should occur
3. **Duration** - Track how long an appointment will take
4. **Out-of-Home Activities** - Mark appointments that require leaving home

## Using the New Features

### 1. Current Date Display

The current date is automatically shown at the top of the main page in a blue header.

**Example:** "Samstag, 4. Oktober 2025"

This gives you quick context about what day it is when planning your appointments.

### 2. Adding a New Appointment with Scheduling

When adding a new appointment, you can now:

1. **Enter appointment text** (required)
   - Example: "Zahnarzttermin"

2. **Set category** (optional)
   - Example: "Gesundheit"

3. **Choose color** (optional)
   - Default: #808080 (gray)

4. **Pick a scheduled date** (NEW - optional)
   - Use the date picker to select when this appointment should occur
   - If not set, the appointment is just on your list without a specific date

5. **Enter duration** (NEW - optional)
   - Free text field - enter whatever makes sense
   - Examples: "30 min", "1 Std", "1-2 Std", "45 Minuten"
   - This helps you estimate time needed

6. **Mark as "Out of Home"** (NEW - optional)
   - Check this box if the appointment requires leaving your house
   - Examples: grocery shopping, doctor visits, picking up kids
   - These appointments get a green background for quick identification

### 3. Viewing Appointments

Appointments now display additional information:

#### Standard Appointment (White Background)
```
┌─────────────────────────────────┐
│ │ Projekt abschließen           │
│ │ 🏷️ Arbeit                     │
│ │ 🕒 14:00 Uhr                  │  ← Scheduled time
│ │ ⏱️ 2-3 Std                    │  ← Duration estimate
│ │                    [⬆️][⬇️]    │
│ │                    [✏️][🗑️]    │
└─────────────────────────────────┘
```

#### Out-of-Home Appointment (Green Background)
```
╔═════════════════════════════════╗
║ │ Lebensmittel einkaufen        ║  ← Green background
║ │ 🏷️ Privat                     ║
║ │ 🕒 16:00 Uhr                  ║
║ │ ⏱️ 30 min                     ║
║ │ 📍 Außer Haus                ║  ← Special indicator
║ │                    [⬆️][⬇️]    ║
║ │                    [✏️][🗑️]    ║
╚═════════════════════════════════╝
```

**Note:** If you don't set a scheduled time or duration, those fields simply won't show up.

### 4. Editing Appointments

To edit an existing appointment:

1. Tap the **✏️ Edit** button on the appointment
2. Modify any field:
   - Text
   - Category
   - Color
   - **Scheduled date** (NEW)
   - **Duration** (NEW)
   - **Out of Home checkbox** (NEW)
3. Tap **💾 Speichern** to save changes

### 5. Priority Management

Priority management remains the same:
- Use **⬆️** to move an appointment up (higher priority)
- Use **⬇️** to move an appointment down (lower priority)
- Appointments at the top are most important

## Example Use Cases

### Example 1: Doctor's Appointment
- **Text:** "Zahnarzttermin"
- **Category:** "Gesundheit"
- **Color:** Red (#FF0000)
- **Scheduled:** Today at 10:00
- **Duration:** "1 Std"
- **Out of Home:** No (or yes, if you need to travel)

**Result:** Shows with time "🕒 10:00 Uhr" and duration "⏱️ 1 Std"

### Example 2: Work Task
- **Text:** "Projekt abschließen"
- **Category:** "Arbeit"
- **Color:** Blue (#0000FF)
- **Scheduled:** Today at 14:00
- **Duration:** "2-3 Std"
- **Out of Home:** No

**Result:** Shows with time and duration, white background (at home)

### Example 3: Grocery Shopping
- **Text:** "Lebensmittel einkaufen"
- **Category:** "Privat"
- **Color:** Green (#00FF00)
- **Scheduled:** Today at 16:00
- **Duration:** "30 min"
- **Out of Home:** ✓ **Yes**

**Result:** Shows with **green background** and "📍 Außer Haus" indicator

## Tips

1. **Green = Go Out:** Remember, green background means you need to leave the house
2. **Duration is flexible:** Enter whatever format makes sense to you
3. **All fields are optional:** You can leave scheduled date, duration, or out-of-home unchecked
4. **Quick planning:** Look at all your green (out-of-home) appointments to plan your day efficiently
5. **Time format:** Scheduled times show as "HH:mm Uhr" (24-hour format)

## Backward Compatibility

**Old appointments still work!**
- Appointments without scheduled dates or durations display normally
- Only new appointments can have the enhanced features
- Edit old appointments to add scheduling information

## German Terms

- **Außer Haus** = Out of home / away from home
- **Std** = Stunde(n) = hour(s)
- **min** = Minute(n) = minute(s)
- **Uhr** = o'clock

## Differences from Legacy Scheduler

While the app now supports the main legacy features, there are some improvements:

1. **Better priority management:** Up/down arrows instead of manual reordering
2. **Flexible colors:** Choose any color, not just predefined ones
3. **Dedicated edit page:** Better user experience than inline editing
4. **Modern UI:** Cleaner, more intuitive interface
5. **Cross-platform:** Works on Android, iOS, Windows, and macOS

Enjoy the enhanced scheduling features! 📅

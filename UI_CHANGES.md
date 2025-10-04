# UI Changes to Match Legacy Scheduler

## Overview
This document describes the UI changes made to align the Terminplaner app with the legacy scheduler shown in `assets/Legacy.jpg` and `assets/Legacy_2.jpg`.

## New Model Properties

### Appointment Model (Both API and MAUI)
Added the following properties to support legacy features:

1. **ScheduledDate** (DateTime?)
   - When the appointment is scheduled to occur
   - Optional field (nullable)
   - Example: "2025-10-04T10:00:00+00:00"

2. **Duration** (string?)
   - Estimated time the appointment will take
   - Flexible format for user-friendly input
   - Examples: "30 min", "1 Std", "1-2 Std"

3. **IsOutOfHome** (bool)
   - Flag to indicate if the appointment is an out-of-home activity
   - Default: false
   - When true, the appointment gets a green background (matching legacy)

## UI Changes

### Main Page (MainPage.xaml)

#### 1. Current Date Display
- **New:** Added a prominent date display at the top of the page
- Shows current date in German format: "Sonntag, 4. Oktober 2025"
- Blue background (#667eea) to make it stand out
- Property: `CurrentDateDisplay` in MainViewModel

#### 2. Add Appointment Section
Added new input fields:

- **DatePicker**: Select scheduled date for the appointment
- **Duration Entry**: Free-text field for duration (e.g., "30 min", "1-2 Std")
- **Out-of-Home Checkbox**: Mark appointment as outside activity

#### 3. Appointment List Items
Each appointment now displays:

- **Time** (🕒): Shows scheduled time if set (e.g., "10:00 Uhr")
- **Duration** (⏱️): Shows estimated duration if set (e.g., "1 Std")
- **Out-of-Home Indicator** (📍): Shows "Außer Haus" label for external activities
- **Green Background**: Appointments with `IsOutOfHome = true` get a light green background (#C8E6C9)
- Color bar, category, and action buttons remain the same

### Edit Page (EditAppointmentPage.xaml)

Added the same new fields:
- DatePicker for scheduled date
- Entry for duration
- Checkbox for out-of-home flag

## Sample Data

Updated sample appointments in `AppointmentService`:

1. **Zahnarzttermin** (Dentist appointment)
   - Scheduled: Today at 10:00
   - Duration: "1 Std"
   - Out of Home: No
   - Color: Red (#FF0000)

2. **Projekt abschließen** (Complete project)
   - Scheduled: Today at 14:00
   - Duration: "2-3 Std"
   - Out of Home: No
   - Color: Blue (#0000FF)

3. **Lebensmittel einkaufen** (Grocery shopping)
   - Scheduled: Today at 16:00
   - Duration: "30 min"
   - Out of Home: **Yes** → Gets green background
   - Color: Green (#00FF00)

## Backward Compatibility

All new fields are **optional**:
- `ScheduledDate` is nullable (DateTime?)
- `Duration` is nullable (string?)
- `IsOutOfHome` defaults to false

Existing appointments without these fields will:
- Not show time/duration labels (conditional rendering with IsNotNullConverter)
- Have white background (not green)
- Work exactly as before

## Visual Comparison with Legacy

### Legacy Scheduler Features
✅ Shows current date/day → **Implemented** (top of page)
✅ Shows appointment text → **Already existed**
✅ Shows estimated time → **Implemented** (duration field)
✅ Green highlighting for out-of-home → **Implemented** (IsOutOfHome flag)
✅ Categorization → **Already existed**

### Differences from Legacy (Intentional)
- **Priority management**: New app uses up/down arrows (more intuitive than legacy)
- **Color customization**: New app allows any color (more flexible)
- **Editing**: New app has dedicated edit page (better UX)
- **Modern UI**: Uses Material Design principles with rounded corners and better spacing

## Testing

All changes have been tested:
- ✅ All 42 unit and integration tests pass
- ✅ API correctly serializes/deserializes new fields
- ✅ Backward compatibility maintained (optional fields)
- ✅ Update operations preserve new fields

## Future Enhancements

Potential improvements for even better legacy compatibility:
1. Add time picker for more precise scheduling
2. Add recurring appointment support
3. Add appointment filtering by date
4. Add calendar view

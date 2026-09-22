# ElectricCheck Sync Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a minimal, testable Windows WinForms sync loop that reads new `ElectricCheck` SQLite rows, POSTs the existing server DTO, persists a cursor, and continues from the system tray after the window is closed.

**Architecture:** Keep the existing database browser unchanged. Add focused models and an `ElectricCheckSyncService` that owns SQLite reading, JSON POST, cursor persistence, and one-at-a-time execution. Wire a small sync panel and `NotifyIcon` into `Form1`; use a cancellable periodic loop rather than file watching.

**Tech Stack:** .NET 10 WinForms, `Microsoft.Data.Sqlite` 10.0.11, `System.Net.Http`, `System.Text.Json`, existing JSON configuration style.

**Spec:** `docs/superpowers/specs/2026-09-21-electric-check-sync-design.md`

## Global Constraints

- Keep SQLite read-only and never execute user-provided SQL for synchronization.
- Send no more than 200 records per request.
- Advance the cursor only after a successful HTTP response.
- Convert non-numeric `TemplateItemID` values such as `缺省` to `0`.
- Keep the first version inside WinForms; no Windows Service or message queue.

### Task 1: Add sync models and cursor store

**Files:**
- Create: `Models/ElectricCheckSyncSettings.cs`
- Create: `Models/ElectricCheckRecordPayload.cs`
- Create: `Models/ElectricCheckBatchPayload.cs`
- Create: `Models/ElectricCheckSyncResult.cs`
- Create: `Configuration/ElectricCheckSyncStateStore.cs`
- Test: `Checks/ElectricCheckSyncChecks.cs`

**Interfaces:**
- `ElectricCheckSyncSettings`: settings for enabled, device, SQLite path, table, URL, interval, batch size.
- `ElectricCheckSyncStateStore.LoadAsync(key, token)` and `SaveAsync(key, state, token)` persist `LastRowId` under `%AppData%/c-lan`.
- Payload models serialize property names matching `ElectricCheckBatchRequest` and `ElectricCheckRecordRequest`.

- [ ] **Step 1: Write a failing check** asserting JSON uses `requestId`, `deviceNo`, `records`, `sourceRowId`, and converts no fields implicitly.
- [ ] **Step 2: Run the check and confirm it fails because the payload types do not exist.**
- [ ] **Step 3: Add the models and JSON options with camelCase naming.**
- [ ] **Step 4: Add cursor state load/save using one JSON file and a sanitized key.**
- [ ] **Step 5: Run the check and confirm it passes.**
- [ ] **Step 6: Commit the model/store slice.**

### Task 2: Implement SQLite batch reading and HTTP sync

**Files:**
- Create: `Services/ElectricCheckSyncService.cs`
- Modify: `c#lan.csproj` only if compilation shows an existing package is insufficient.
- Test: `Checks/ElectricCheckSyncChecks.cs`

**Interfaces:**
- `Task<ElectricCheckSyncResult> SyncOnceAsync(ElectricCheckSyncSettings settings, CancellationToken token)` reads from the saved cursor and sends all available batches.
- `Task RunAsync(settings, onResult, token)` repeats `SyncOnceAsync` on the configured interval.

- [ ] **Step 1: Add a failing check for mapping `TemplateItemID = "缺省"` to `0` and blank optional values to null.**
- [ ] **Step 2: Run the check and confirm the expected mapping failure.**
- [ ] **Step 3: Implement fixed-column SQLite reading with `rowid`, parameterized cursor/limit, and read-only connection.**
- [ ] **Step 4: Implement `HttpClient` POST to `/openapi/electric-check/records:batch`; treat only 2xx as success.**
- [ ] **Step 5: Save the cursor only after each successful batch and serialize one batch at a time.**
- [ ] **Step 6: Add a single semaphore so manual and periodic sync cannot overlap.**
- [ ] **Step 7: Run checks and a build.**
- [ ] **Step 8: Commit the sync service slice.**

### Task 3: Wire the minimal WinForms panel and tray lifecycle

**Files:**
- Modify: `Forms/Form1.cs`
- Modify: `Forms/Form1.Designer.cs`
- Modify: `Forms/Form1.resx` only if generated resources require it.
- Modify: `Program.cs`

**Interfaces:**
- `Form1` receives `ElectricCheckSyncService` through the existing composition root.
- Form controls expose enable flag, device number, SQLite path, server URL, interval, manual sync, and status.

- [ ] **Step 1: Add controls and wire the manual sync click; keep the existing database browser behavior unchanged.**
- [ ] **Step 2: Start the cancellable periodic loop on form shown when enabled.**
- [ ] **Step 3: Handle form closing by cancelling the close and hiding to `NotifyIcon`; expose tray show/sync/exit actions.**
- [ ] **Step 4: Dispose the tray icon, timer cancellation, and service-owned HTTP client on real exit.**
- [ ] **Step 5: Build and run the checks.**
- [ ] **Step 6: Commit the WinForms slice.**

### Task 4: Verify against the supplied sample and document server prerequisite

**Files:**
- Modify: `docs/superpowers/specs/2026-09-21-electric-check-sync-design.md` only if implementation discovers a necessary correction.

- [ ] **Step 1: Create a temporary SQLite database from the supplied `ElectricCheck.sql` sample.**
- [ ] **Step 2: Run one sync against a local HTTP test endpoint and verify request shape and cursor advancement.**
- [ ] **Step 3: Repeat the sync and verify no duplicate upload occurs from the client cursor.**
- [ ] **Step 4: Verify a failed HTTP response leaves the cursor unchanged.**
- [ ] **Step 5: Run the full build/check command and inspect `git diff --check`.**
- [ ] **Step 6: Report the exact remaining server database prerequisite: unique `(device_no, source_row_id)` index.**


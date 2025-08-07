# MeterReadingUploadAPI

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 9.0.12.

## Build

Run `dotnet build -tl` to build the solution.

## Run

To run the web application:

```bash
cd .\src\Web\
dotnet watch run
```

Navigate to <https://localhost:5001>. The application will automatically reload if you change any of the source files.

## **Kanban Style View:** Grouped by Task Status  

### ✅ Done

- [x] Initial Commit

---

### 🟨 In Progress

- [X] DB Duplicate check in for CreateMeterReadingItemsCommand
- [x] DTO/FluentValidator for DTO
- [ ] Tests CreateMeterReadingItemsCommandTests
- [x] Angular component
- [x] SOLID for CreateMeterReadingItemsCommand

---

### ⬜ To Do


## Test

The solution contains unit, integration, functional, and acceptance tests.

To run the unit, integration, and functional tests (excluding acceptance tests):

```bash
dotnet test --filter "FullyQualifiedName!~AcceptanceTests"
```

To run the acceptance tests, first start the application:

```bash
cd .\src\Web\
dotnet run
```

Then, in a new console, run the tests:

```bash
cd .\src\Web\
dotnet test
```

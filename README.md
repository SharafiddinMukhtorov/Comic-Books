# 📚 ComicBooks - Manga/Manhwa Reading Platform

An **AsuraScan** alternative built with **.NET 9**, **Blazor Server**, **MudBlazor**, **Clean Architecture**, and **CQRS**.

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Blazor](https://img.shields.io/badge/Blazor-Server-blue)
![MudBlazor](https://img.shields.io/badge/MudBlazor-7.x-red)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-green)

## 🏗️ Architecture: Clean Architecture + CQRS

```
src/
├── ComicBooks.Domain/           # Entities, Enums
├── ComicBooks.Application/      # CQRS (MediatR), DTOs, FluentValidation
├── ComicBooks.Infrastructure/   # EF Core, SQLite/SQL Server
└── ComicBooks.Web/              # Blazor Server + MudBlazor UI
```

## ✨ Features
- 🌙 Dark/Light mode toggle
- 📱 Fully responsive (mobile + desktop)
- 📖 Vertical chapter reader with adjustable width
- 🔍 Search + Filter by status, type, genre
- ⚙️ Full Admin panel (Comics CRUD, Chapters, Genres, Tags)
- 🗄️ SQLite (dev) / SQL Server (prod)
- 🌱 Auto database seed with sample data

## 🚀 Quick Start

```bash
cd src/ComicBooks.Web
dotnet run
```

Open `http://localhost:5000` — database is seeded automatically.

## 🗺️ Routes
| Route | Description |
|-------|-------------|
| `/` | Home |
| `/comics` | Comics list + filters |
| `/comic/{slug}` | Comic detail |
| `/chapter/{id}` | Chapter reader |
| `/genres` | Genre browser |
| `/admin` | Admin dashboard |
| `/admin/comics` | Manage comics |
| `/admin/chapters` | Manage chapters |
| `/admin/genres` | Genres & Tags |

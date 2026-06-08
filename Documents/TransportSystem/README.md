# 🚌 School Transport Management System — Backend

A full-featured ASP.NET Core Web API for managing school transport operations including routes, drivers, trips, students, payments, live tracking, and emergency alerts.

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core (.NET 10)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Authentication:** JWT Bearer Tokens
- **SMS:** Africa's Talking API
- **Email:** Gmail SMTP via MailKit
- **Reports:** PDF & CSV exports

---

## ✅ Features

1. **Analytics Dashboard** — trip stats, attendance rates, payment summaries
2. **SOS Emergency Alerts** — drivers trigger alerts; admins receive SMS + email instantly
3. **Late Trip Detection** — automatic alerts for overdue scheduled trips
4. **Driver Performance Tracking** — on-time rate, trips completed, status monitoring
5. **PDF & CSV Report Exports** — downloadable reports for trips, payments, attendance
6. **SMS Notifications** — via Africa's Talking sandbox API
7. **Email Notifications** — via Gmail SMTP
8. **Multi-school Support** — manage multiple schools with their own students and vehicles
9. **Live Trip Tracking** — GPS coordinates posted and retrieved per trip
10. **Role-Based Access** — Admin, Driver, and Parent roles with separate dashboards

---

## 🗄️ Database Setup

1. Install PostgreSQL and create a database:

```sql
CREATE DATABASE transport_system;
```

2. Update `appsettings.json` (use `appsettings.Example.json` as a template):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=transport_system;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Email": {
    "Username": "your.email@gmail.com",
    "Password": "your-gmail-app-password"
  }
}
```

3. Run migrations:

```bash
dotnet ef database update
```

---

## 🚀 Running the Backend

```bash
cd TransportSystem
dotnet restore
dotnet run
```

API runs on: `http://localhost:5044`

---

## 📡 Key API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/login | Login and get JWT token |
| GET | /api/schools | List all schools |
| GET | /api/drivers | List all drivers |
| GET | /api/trips | List all trips |
| POST | /api/notifications/sos | Trigger SOS alert |
| GET | /api/reports/trips | Download trip report |
| GET | /api/analytics/summary | Get analytics data |

---

## 👩‍💻 Developer

**Maryivy Kibali Wasike**
Bachelor of Business and Information Technology
Africa Nazarene University — Class of 2027
GitHub: [@mysteriouKay](https://github.com/mysteriouKay)
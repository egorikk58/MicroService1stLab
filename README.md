# FlightTickets API (MongoDB + ASP.NET Core)



## 🛠 Требования

1. **.NET 8.0 SDK** (или новее).
2. **MongoDB**: запущенная локально или в Docker.(докер как я понял в других лабах можно только)
Но я тестил и так и так
Если установлен докер то команда выглядит так 
```docker
docker run -d -p 27017:27017 --name mongo-db mongo
```
но тогда надо будет учитывать что при перезапуске БД данные не сохранятся, используйте VOLUME (-v или --volume)
## ⚙️ Настройка перед запуском

Проект настроен на работу с локальной базой данных. Если ваш порт отличается от стандартного, измените строку подключения в файле `appsettings.json`:

```json
"FlightTicketsDB": {
    "ConnectionString": "mongodb://localhost:27017", 
    "DatabaseName": "FlightTickets",
    "CollectionName": "FlightTickets"
}

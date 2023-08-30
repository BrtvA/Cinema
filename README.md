# Cinema

Данное ASP.NET Core приложение представляет собой сервис для продажи билетов в кинотеатр.

Имеется несколько типов пользователей:

- неавторизованный пользователь;
- администратор;
- покупатель.

По умолчанию при первом запуске приложения (при создании базы данных) создаётся пользователь типа администратор со следующими данными:

- email: ```cinema@yandex.ru```
- пароль: ```1234```

Также для осуществления покупок по умолчанию задаётся клиентский банковский счёт:

- номер карты: ```1111222233334444```
- срок действия: ```01 / 30```
- CVV/CVC: ```123```

Перед запуском необходимо настроить строку подключения к базе данных PostgreSQL в файле [appsettings.json](src/Cinema.PL/appsettings.json) в разделе ```ConnectionStrings : CinemaDB```.

Перед запуском модульных тестов также необходимо настроить строку подключения в файле [settings.json](test/Cinema.Test/settings.json)

# Пример запуска в Docker

1. Создаем docker image:
```
docker build -f src/Cinema.PL/Dockerfile -t cinema:1.0.0.0 .
```

2. Содаем docker network:
```
docker network create --driver bridge --subnet 172.28.0.0/16 --gateway 172.28.5.254 chetnet
```

3. Запускаем docker контейнеры в созданной сети с использованием environment variables:

- для СУБД PostgreSQL:
```
docker run --network chetnet --name postgres -e POSTGRES_PASSWORD=pgpassword -d -p 5432:5432 postgres
```

- для ASP.NET приложения:
```
docker run --network chetnet --name cinema -e ConnectionStrings:CinemaDB="Server=172.28.5.254;Database=cinemadb;User Id=postgres;Password=pgpassword;" -p 5000:80 cinema:1.0.0.0
```
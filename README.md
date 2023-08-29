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
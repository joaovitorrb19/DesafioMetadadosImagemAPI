# ChallengeAPI

- API desenvolvida para o desafio da InventSoftware, para CRUD completo do modelo MetadadosDeImagem.
- Utiliza a arquitetura Service/Repository
- Os testes cobrem todos os cenarios do Controller,Service e do Repository(utilizado o Mongo2Go para simular o MongoDB)

## Tecnologias

**API**
- .NET 8.0
- MongoDB.Driver 3.5.0
- EntityFrameworkCore InMemory 9.0.9
- Swashbuckle (Swagger)
- SixLabors
- Docker

**Testes (Challenge.UnitTests)**
- .NET 8.0
- Mongo2Go 4.1.0
- Moq 4.20.72
- xUnit 2.9.3

---

## Como rodar o projeto
git clone https://github.com/joaovitorrb19/DesafioInventSoftwareAPI.git

### Maneira prática com o Docker (utilizar na pasta base onde tem o arquivo docker-compose.yml):
      docker compose up --build -d 

---

### Sem utilizar o Docker:
      dotnet restore
      dotnet run
- Comandos dentro da pasta ChallengeAPI: 
- MongoDB precisa estar disponivel na porta padrao 27017 (modificar no appsettings.json)

### Testar o projeto Challenge.UnitTests:
      dotnet test
- Comando de da pasta Challenge.UnitTests

http://localhost:5050/swagger/index.html > Swagger para testar o CRUD pelo navegador

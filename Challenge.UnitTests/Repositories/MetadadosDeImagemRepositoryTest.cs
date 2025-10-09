using ChallengeAPI.Models;
using ChallengeAPI.Repositories;
using Mongo2Go;
using MongoDB.Driver;
using System.Threading.Tasks;
using Xunit;


namespace Challenge.UnitTests.Repositories;
public class MetadadosDeImagemRepositoryTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly IMongoDatabase _database;
    private readonly MetadadosDeImagemRepository _repository;

    public MetadadosDeImagemRepositoryTests()
    {
        
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        _database = client.GetDatabase("TesteDb");
        _repository = new MetadadosDeImagemRepository(_database);
    }

    [Fact]
    public async Task AdicionarEGetById_DeveFuncionar()
    {
        var metadado = new MetadadosDeImagem { Id = "1", NomeDoArquivo = "teste1.png" };

        await _repository.Adicionar(metadado);
        var resultado = await _repository.GetById("1");

        Assert.NotNull(resultado);
        Assert.Equal("teste1.png", resultado.NomeDoArquivo);
    }

    [Fact]
    public async Task Get_DeveRetornarTodos()
    {
        await _repository.Adicionar(new MetadadosDeImagem { Id = "2", NomeDoArquivo = "teste2.png" });
        await _repository.Adicionar(new MetadadosDeImagem { Id = "3", NomeDoArquivo = "teste3.png" });

        var lista = await _repository.Get();

        Assert.Equal(2, lista.Count);
        Assert.Contains(lista, m => m.NomeDoArquivo == "teste2.png");
        Assert.Contains(lista, m => m.NomeDoArquivo == "teste3.png");
    }

    [Fact]
    public async Task Atualizar_DeveAlterarDados()
    {
        var metadado = new MetadadosDeImagem { Id = "4", NomeDoArquivo = "teste4.png" };
        await _repository.Adicionar(metadado);

        metadado.NomeDoArquivo = "alterado.png";
        await _repository.Atualizar(metadado);

        var resultado = await _repository.GetById("4");
        Assert.Equal("alterado.png", resultado.NomeDoArquivo);
    }

    [Fact]
    public async Task Deletar_DeveRemoverItem()
    {
        var metadado = new MetadadosDeImagem { Id = "5", NomeDoArquivo = "teste5.png" };
        await _repository.Adicionar(metadado);

        await _repository.Deletar("5");
        var resultado = await _repository.GetById("5");

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ExisteNaBase_DeveRetornarCorreto()
    {
        var metadado = new MetadadosDeImagem { Id = "6", NomeDoArquivo = "teste6.png" };
        await _repository.Adicionar(metadado);

        var existe = await _repository.ExisteNaBase("teste6.png");
        var naoExiste = await _repository.ExisteNaBase("inexistente.png");

        Assert.True(existe);
        Assert.False(naoExiste);
    }

    public void Dispose()
    {
        _runner.Dispose(); 
    }
}

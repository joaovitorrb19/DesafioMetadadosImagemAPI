namespace Challenge.UnitTests.Services
{
    using Xunit;
    using Moq;
    using ChallengeAPI.Interfaces;
    using ChallengeAPI.Services;
    using ChallengeAPI.Models;
    using Microsoft.AspNetCore.Http;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Formats.Png;
    using System.IO;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MetadadosDeImagemServiceTests
    {
        private readonly Mock<IMetadadosDeImagemRepository> _metadadosDeImagemRepositoryMock;
        private readonly MetadadosDeImagemService _metadadosDeImagemService;

        public MetadadosDeImagemServiceTests()
        {
            _metadadosDeImagemRepositoryMock = new Mock<IMetadadosDeImagemRepository>();
            _metadadosDeImagemService = new MetadadosDeImagemService(_metadadosDeImagemRepositoryMock.Object);
        }


        private IFormFile CriarMockImageFile(string fileName, string contentType)
        {
            using var image = new Image<Rgba32>(10, 10);
            var stream = new MemoryStream();
            image.Save(stream, new PngEncoder());
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, "Data", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
       
        [Fact]
        public async Task Get_DeveRetornarListaPreenchida_QuandoExistiremDados()
        {
            
            var listaMock = new List<MetadadosDeImagem>
            {
                new MetadadosDeImagem { Id = "1", NomeDoArquivo = "imagem1.jpg" },
                new MetadadosDeImagem { Id = "2", NomeDoArquivo = "imagem2.png" }
            };
            _metadadosDeImagemRepositoryMock.Setup(repo => repo.Get()).ReturnsAsync(listaMock);

            
            var result = await _metadadosDeImagemService.Get();

            
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task Get_DeveRetornarListaVazia_QuandoNaoExistiremDados()
        {
            
            _metadadosDeImagemRepositoryMock.Setup(repo => repo.Get()).ReturnsAsync(new List<MetadadosDeImagem>());

            
            var result = await _metadadosDeImagemService.Get();

            
            Assert.NotNull(result);
            Assert.Empty(result);
        }
       
        [Fact]
        public async Task GetById_DeveRetornarMetadados_QuandoIdExistir()
        {
            
            var metadadosMock = new MetadadosDeImagem { Id = "1", NomeDoArquivo = "teste.png" };
            _metadadosDeImagemRepositoryMock.Setup(repo => repo.GetById("1")).ReturnsAsync(metadadosMock);

            
            var result = await _metadadosDeImagemService.GetById("1");

            
            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        [Fact]
        public async Task GetById_DeveRetornarNull_QuandoIdNaoExistir()
        {
            
            _metadadosDeImagemRepositoryMock.Setup(repo => repo.GetById(It.IsAny<string>())).ReturnsAsync((MetadadosDeImagem?)null);

            
            var result = await _metadadosDeImagemService.GetById("999");

            
            Assert.Null(result);
        }
       

        [Fact]
        public async Task CriarMetadadosDaImagem_DeveLancarExcecao_QuandoArquivoJaExiste()
        {
            
            var mockFile = CriarMockImageFile("teste.png", "image/png");
            _metadadosDeImagemRepositoryMock.Setup(r => r.ExisteNaBase(mockFile.FileName)).ReturnsAsync(true);

            
            var exception = await Assert.ThrowsAsync<Exception>(() => _metadadosDeImagemService.CriarMetadadosDaImagem(mockFile));
            Assert.Equal("Já existe um arquivo com esse nome na base de dados.", exception.Message);
        }

        [Fact]
        public async Task CriarMetadadosDaImagem_DeveLancarExcecao_QuandoExtensaoForInvalida()
        {
            
            var mockFile = CriarMockImageFile("documento.pdf", "application/pdf");

            
            var exception = await Assert.ThrowsAsync<Exception>(() => _metadadosDeImagemService.CriarMetadadosDaImagem(mockFile));
            Assert.StartsWith("Extensão de arquivo inválida.", exception.Message);
        }

        [Fact]
        public async Task CriarMetadadosDaImagem_DeveRetornarMetadados_QuandoCriadoComSucesso()
        {
            
            var mockFile = CriarMockImageFile("novoArquivo.png", "image/png");
            _metadadosDeImagemRepositoryMock.Setup(r => r.ExisteNaBase(mockFile.FileName)).ReturnsAsync(false);
            _metadadosDeImagemRepositoryMock.Setup(r => r.Adicionar(It.IsAny<MetadadosDeImagem>()))
                .ReturnsAsync((MetadadosDeImagem m) => m);

           
            var result = await _metadadosDeImagemService.CriarMetadadosDaImagem(mockFile);

            
            Assert.NotNull(result);
            _metadadosDeImagemRepositoryMock.Verify(r => r.Adicionar(It.IsAny<MetadadosDeImagem>()), Times.Once);
        }
       
       
        [Fact]
        public async Task UpdateMetadadosDeImagem_DeveLancarExcecao_QuandoIdNaoExiste()
        {
            
            var mockFile = CriarMockImageFile("arquivo.png", "image/png");
            _metadadosDeImagemRepositoryMock.Setup(r => r.GetById("999")).ReturnsAsync((MetadadosDeImagem?)null);

            
            var exception = await Assert.ThrowsAsync<Exception>(() => _metadadosDeImagemService.UpdateMetadadosDeImagem("999", mockFile));
            
            Assert.Equal("Metadados não encontrados.", exception.Message);
        }

        [Fact]
        public async Task UpdateMetadadosDeImagem_DeveLancarExcecao_QuandoNovoNomeJaExisteEmOutroRegistro()
        {
            
            var mockFile = CriarMockImageFile("nome-existente.png", "image/png");
            var registroExistente = new MetadadosDeImagem { Id = "1", NomeDoArquivo = "antigo.png" };

            _metadadosDeImagemRepositoryMock.Setup(r => r.GetById("1")).ReturnsAsync(registroExistente);
            _metadadosDeImagemRepositoryMock.Setup(r => r.ExisteNaBase("nome-existente.png")).ReturnsAsync(true);

            
            var exception = await Assert.ThrowsAsync<Exception>(() => _metadadosDeImagemService.UpdateMetadadosDeImagem("1", mockFile));
            Assert.Equal("Já existe outro arquivo com esse nome na base de dados.", exception.Message);
        }

        [Fact]
        public async Task UpdateMetadadosDeImagem_DeveChamarAtualizar_QuandoDadosValidos()
        {
            
            var mockFile = CriarMockImageFile("novo-nome.png", "image/png");
            var registroExistente = new MetadadosDeImagem { Id = "1", NomeDoArquivo = "antigo.png", DataDeCriacao = DateTime.UtcNow.AddDays(-1) };

            _metadadosDeImagemRepositoryMock.Setup(r => r.GetById("1")).ReturnsAsync(registroExistente);
            _metadadosDeImagemRepositoryMock.Setup(r => r.ExisteNaBase(mockFile.FileName)).ReturnsAsync(false);

            
            await _metadadosDeImagemService.UpdateMetadadosDeImagem("1", mockFile);

            _metadadosDeImagemRepositoryMock.Verify(r => r.Atualizar(It.Is<MetadadosDeImagem>(m =>
                m.Id == "1" &&
                m.NomeDoArquivo == "novo-nome.png" &&
                m.DataDeCriacao == registroExistente.DataDeCriacao
            )), Times.Once);
        }
       

        
        [Fact]
        public async Task DeletarMetadadosDeImagem_DeveLancarExcecao_QuandoIdNaoExiste()
        {
            
            _metadadosDeImagemRepositoryMock.Setup(r => r.GetById("999")).ReturnsAsync((MetadadosDeImagem?)null);

            
            var exception = await Assert.ThrowsAsync<Exception>(() => _metadadosDeImagemService.DeletarMetadadosDeImagem("999"));
           
            Assert.Equal("Metadados não encontrados para deletar.", exception.Message);
            _metadadosDeImagemRepositoryMock.Verify(r => r.Deletar(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeletarMetadadosDeImagem_DeveChamarDeletar_QuandoIdExiste()
        {
            
            var metadadosMock = new MetadadosDeImagem { Id = "1", NomeDoArquivo = "teste.png" };
            _metadadosDeImagemRepositoryMock.Setup(r => r.GetById("1")).ReturnsAsync(metadadosMock);

            
            await _metadadosDeImagemService.DeletarMetadadosDeImagem("1");

            
            _metadadosDeImagemRepositoryMock.Verify(r => r.Deletar("1"), Times.Once);
        }
       
    }
}
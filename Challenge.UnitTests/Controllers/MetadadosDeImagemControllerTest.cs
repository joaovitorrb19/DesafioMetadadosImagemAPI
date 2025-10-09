using ChallengeAPI.Controllers;
using ChallengeAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge.UnitTests.Controller
{
    public class MetadadosDeImagemControllerTest
    {
        private readonly MetadadosDeImagemController _controller;
        private readonly Mock<IMetadadosDeImagemService> _serviceMock;

        public MetadadosDeImagemControllerTest()
        {
            _serviceMock = new Mock<IMetadadosDeImagemService>();
            _controller = new MetadadosDeImagemController(_serviceMock.Object);
        }

        [Fact]
        public async Task Get_RetornaListaSucesso()
        {
            
            var mockData = new List<ChallengeAPI.Models.MetadadosDeImagem>
            {
                new ChallengeAPI.Models.MetadadosDeImagem { Id = "1", NomeDoArquivo = "image1.jpg" },
                new ChallengeAPI.Models.MetadadosDeImagem { Id = "2", NomeDoArquivo = "image2.png" }
            };
            _serviceMock.Setup(service => service.Get()).ReturnsAsync(mockData);
            
            var result = await _controller.Get();
            
            var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ChallengeAPI.Models.MetadadosDeImagem>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async Task GetById_RetornaNotFound()
        {
            _serviceMock.Setup(service => service.GetById("1")).ReturnsAsync((ChallengeAPI.Models.MetadadosDeImagem?)null);

            var result = await _controller.Get("1");

            var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);

        }
        [Fact]
        public async Task GetById_RetornaSucesso()
        {
            _serviceMock.Setup(service => service.GetById("1")).ReturnsAsync(new ChallengeAPI.Models.MetadadosDeImagem { Id = "1", NomeDoArquivo = "image1.jpg" });
            var result = await _controller.Get("1");
            var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ChallengeAPI.Models.MetadadosDeImagem>(okResult.Value);
            Assert.Equal("1", returnValue.Id);
        }
        [Fact]
        public async Task Post_RetornaBadRequest_QuandoImagemNula()
        {
            var result = await _controller.Post(null!);
            var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        [Fact]
        public async Task Post_RetornaExcpetionJaExisteNaBase()
        {
            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(f => f.FileName).Returns("image1.jpg");
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            _serviceMock.Setup(service => service.CriarMetadadosDaImagem(It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("Já existe um arquivo com esse nome na base de dados."));

            var result = await _controller.Post(fileMock.Object);

            var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        [Fact]
        public async Task Post_RetornaSucesso()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("image1.jpg");
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            var metadadosMock = new ChallengeAPI.Models.MetadadosDeImagem
            {
                Id = "1",
                NomeDoArquivo = "image1.jpg",
                TipoDoArquivo = "image/jpeg",
                Altura = 800,
                Comprimento = 600,
                Proporcao = "4:3",
                DataDeCriacao = DateTime.UtcNow
            };
            _serviceMock.Setup(service => service.CriarMetadadosDaImagem(It.IsAny<IFormFile>()))
                .ReturnsAsync(metadadosMock);
            var result = await _controller.Post(fileMock.Object);
            var createdAtActionResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<ChallengeAPI.Models.MetadadosDeImagem>(createdAtActionResult.Value);
            Assert.Equal("1", returnValue.Id);
        }
        [Fact]
        public async Task Put_RetornaBadRequest()
        {
            _serviceMock.Setup(service => service.UpdateMetadadosDeImagem("1", null!))
                .ThrowsAsync(new Exception("Nenhuma imagem foi enviada."));

            var result = await _controller.Put("1", null!);

            var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        [Fact]
        public async Task Put_RetornaIdNaoExiste()
        {
            _serviceMock.Setup(service => service.UpdateMetadadosDeImagem("1", It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("Metadados não encontrados."));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("image1.jpg");
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            var result = await _controller.Put("1", fileMock.Object);

            var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result);
            Assert.Equal(404,badRequestResult.StatusCode);
        }
        [Fact]
        public async Task Put_RetornaJaExisteEsseNomeNaBase()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("image1.jpg");
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            _serviceMock.Setup(service => service.UpdateMetadadosDeImagem("1", It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("Já existe outro arquivo com esse nome na base de dados."));

            var result = await _controller.Put("1", fileMock.Object);
            var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        [Fact]
        public async Task Put_RetornaSucesso()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("image1.jpg");
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            _serviceMock.Setup(service => service.UpdateMetadadosDeImagem("1", It.IsAny<IFormFile>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.Put("1", fileMock.Object);

            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }
        [Fact]
        public async Task Delete_RetornaIdNaoExiste()
        {
            _serviceMock.Setup(service => service.DeletarMetadadosDeImagem("1"))
                .ThrowsAsync(new Exception("Metadados não encontrados para deletar."));

            var result = await _controller.Delete("1");

            var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result);
            Assert.Equal(404, badRequestResult.StatusCode);
        }
        [Fact]
        public async Task Delete_RetornaErroInterno()
        {
            _serviceMock.Setup(service => service.DeletarMetadadosDeImagem("1"))
                .ThrowsAsync(new Exception("Erro ao deletar os metadados da imagem."));

            var result = await _controller.Delete("1");
            var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);

            Assert.Equal(400, badRequestResult.StatusCode);
        }
        [Fact]
        public async Task Delete_RetornaSucesso()
        {
            _serviceMock.Setup(service => service.DeletarMetadadosDeImagem("1"))
                .Returns(Task.CompletedTask);
            var result = await _controller.Delete("1");
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

    }
}

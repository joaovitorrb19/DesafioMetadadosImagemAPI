using ChallengeAPI.Interfaces;
using ChallengeAPI.Models;
using SixLabors.ImageSharp;

namespace ChallengeAPI.Services
{
    public class MetadadosDeImagemService : IMetadadosDeImagemService
    {
        private readonly IMetadadosDeImagemRepository _repository;

        public MetadadosDeImagemService(IMetadadosDeImagemRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MetadadosDeImagem>> Get()
        {
            return await _repository.Get();
        }

        
        public async Task<MetadadosDeImagem?> GetById(string id)
        {
            return await _repository.GetById(id);
        }

        public async Task<MetadadosDeImagem> CriarMetadadosDaImagem(IFormFile file)
        {
            ValidarExtensaoDoArquivo(file.ContentType);

            if (await _repository.ExisteNaBase(file.FileName))
                throw new Exception("Já existe um arquivo com esse nome na base de dados.");

            var metadados = InstanciarMetadadosDeImagem(file);
            return await _repository.Adicionar(metadados);
        }

        public async Task UpdateMetadadosDeImagem(string id, IFormFile file)
        {
            ValidarExtensaoDoArquivo(file.ContentType);

            var existente = await _repository.GetById(id);
            if (existente == null)
                throw new Exception("Metadados não encontrados.");

            if (existente.NomeDoArquivo != file.FileName && await _repository.ExisteNaBase(file.FileName))
                throw new Exception("Já existe outro arquivo com esse nome na base de dados.");

            var metadadosAtualizado = InstanciarMetadadosDeImagem(file);

            existente.NomeDoArquivo = metadadosAtualizado.NomeDoArquivo;
            existente.TipoDoArquivo = metadadosAtualizado.TipoDoArquivo;
            existente.Altura = metadadosAtualizado.Altura;
            existente.Comprimento = metadadosAtualizado.Comprimento;
            existente.Proporcao = metadadosAtualizado.Proporcao;

            await _repository.Atualizar(existente);
        }

        public async Task DeletarMetadadosDeImagem(string id)
        {
            var existente = await _repository.GetById(id);
            if (existente == null)
                throw new Exception("Metadados não encontrados para deletar.");

            await _repository.Deletar(id);
        }

        
        private void ValidarExtensaoDoArquivo(string extensao)
        {
            var extensoes = new List<string> { "image/png", "image/jpeg", "image/jpg", "image/gif", "image/bmp" };
            if (!extensoes.Contains(extensao.ToLower()))
                throw new Exception("Extensão de arquivo inválida. Extensões permitidas: PNG, JPEG, JPG, GIF, BMP.");
        }

        private MetadadosDeImagem InstanciarMetadadosDeImagem(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var image = Image.Load(stream);

            return new MetadadosDeImagem
            {
               
                Id = Guid.NewGuid().ToString(),
                NomeDoArquivo = file.FileName,
                TipoDoArquivo = file.ContentType,
                Altura = image.Height,
                Comprimento = image.Width,
                Proporcao = CalcularProporcao(image.Height, image.Width),
                DataDeCriacao = DateTime.Now
            };
        }

        public string CalcularProporcao(int altura, int largura)
        {
            List<int> divisores = new List<int>();

            int numero01 = altura;
            int numero02 = largura;

            int menorNumero = Math.Min(numero01, numero02);

            for (int i = 1; i <= menorNumero; i++)
            {
                if (numero01 % i == 0 && numero02 % i == 0)
                {
                    divisores.Add(i);
                    continue;
                }

            }

            int maiorDivisor = divisores.Max();


            if (maiorDivisor == 1)
            {
                return $"{numero01}:{numero02} ≈ {Math.Round((double)numero01 / numero02, 2)}";
            }

            int proporcaoNumero01 = numero01 / maiorDivisor;
            int proporcaoNumero02 = numero02 / maiorDivisor;

            return $"{numero01}:{numero02} = {proporcaoNumero01}:{proporcaoNumero02}";
        }
    }
}
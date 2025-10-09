// Interfaces/IMetadadosDeImagemRepository.cs

using ChallengeAPI.Models;

namespace ChallengeAPI.Interfaces
{
    public interface IMetadadosDeImagemRepository
    {
        Task<List<MetadadosDeImagem>> Get();
        Task<MetadadosDeImagem?> GetById(string id);
        Task<MetadadosDeImagem> Adicionar(MetadadosDeImagem metadados);
        Task<MetadadosDeImagem> Atualizar(MetadadosDeImagem metadados);
        Task Deletar(string id);
        Task<bool> ExisteNaBase(string nomeDoArquivo);
    }
}
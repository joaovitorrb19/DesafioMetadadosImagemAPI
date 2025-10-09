// Interfaces/IMetadadosDeImagemService.cs

using ChallengeAPI.Models;

namespace ChallengeAPI.Interfaces
{
    public interface IMetadadosDeImagemService
    {
        // ANOTAÇÃO: Removido o '?' pois o método sempre retornará uma lista (pode ser vazia).
        Task<List<MetadadosDeImagem>> Get();

        // ANOTAÇÃO: Adicionado o método GetById para ser usado pelo controller.
        Task<MetadadosDeImagem?> GetById(string id);

        Task<MetadadosDeImagem> CriarMetadadosDaImagem(IFormFile file);

        // ANOTAÇÃO: Assinatura corrigida para usar 'string id'.
        Task UpdateMetadadosDeImagem(string id, IFormFile file);

        // ANOTAÇÃO: Assinatura corrigida para usar 'string id'.
        Task DeletarMetadadosDeImagem(string id);

        // ANOTAÇÃO: Métodos auxiliares (ValidarExtensao, Instanciar, CalcularProporcao, GCD)
        // foram removidos da interface pois são detalhes de implementação.
    }
}
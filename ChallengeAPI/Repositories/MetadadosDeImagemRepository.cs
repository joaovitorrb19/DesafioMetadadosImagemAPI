
using ChallengeAPI.Interfaces;
using ChallengeAPI.Models;
using MongoDB.Driver;

namespace ChallengeAPI.Repositories
{
    public class MetadadosDeImagemRepository : IMetadadosDeImagemRepository
    {
        private readonly IMongoCollection<MetadadosDeImagem> _collection;

        public MetadadosDeImagemRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<MetadadosDeImagem>("MetadadosDeImagens");
        }

        public async Task<List<MetadadosDeImagem>> Get()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<MetadadosDeImagem?> GetById(string id)
        {
            return await _collection.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<MetadadosDeImagem> Adicionar(MetadadosDeImagem metadados)
        {
            await _collection.InsertOneAsync(metadados);
            return metadados;
        }

        public async Task<MetadadosDeImagem> Atualizar(MetadadosDeImagem metadados)
        {
            await _collection.ReplaceOneAsync(m => m.Id == metadados.Id, metadados);
            return metadados;
        }

        public async Task Deletar(string id)
        {
            await _collection.DeleteOneAsync(m => m.Id == id);
        }

        public async Task<bool> ExisteNaBase(string nomeDoArquivo)
        {
            return await _collection.Find(m => m.NomeDoArquivo == nomeDoArquivo).AnyAsync();
        }
    }
}
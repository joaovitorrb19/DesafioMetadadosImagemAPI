// Models/MetadadosDeImagem.cs

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChallengeAPI.Models
{
    public class MetadadosDeImagem
    {
        [BsonId] // Marca esta propriedade como a chave primária do documento.
        [BsonRepresentation(BsonType.String)] // Garante que o ID seja armazenado como string.
        public string Id { get; set; } = string.Empty;

        public string NomeDoArquivo { get; set; } = string.Empty;

        public string TipoDoArquivo { get; set; } = string.Empty;

        public int Altura { get; set; }

        public int Comprimento { get; set; }

        public string Proporcao { get; set; } = string.Empty;

        public DateTime DataDeCriacao { get; set; }

    }
}
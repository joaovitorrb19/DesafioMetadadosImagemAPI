using ChallengeAPI.Interfaces;
using ChallengeAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeAPI.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class MetadadosDeImagemController : ControllerBase
    {
        private readonly IMetadadosDeImagemService _metadadosDeImagemService;

        public MetadadosDeImagemController(IMetadadosDeImagemService metadadosDeImagemService)
        {
            _metadadosDeImagemService = metadadosDeImagemService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MetadadosDeImagem>>> Get()
        {
            
            var metadados = await _metadadosDeImagemService.Get();
            return Ok(metadados);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MetadadosDeImagem>> Get(string id)
        {
            
            var metadados = await _metadadosDeImagemService.GetById(id);

            if (metadados == null)
            {
                return NotFound(new { mensagem = "Metadados não encontrados." });
            }

            return Ok(metadados);
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile Imagem) 
        {
            if (Imagem == null || Imagem.Length == 0)
            {
                return BadRequest(new { erro = "Nenhuma imagem foi enviada." });
            }

            try
            {
                var novoMetadados = await _metadadosDeImagemService.CriarMetadadosDaImagem(Imagem);
                
                return CreatedAtAction(nameof(Get), new { id = novoMetadados.Id }, novoMetadados);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, IFormFile Imagem) 
        {
            if (Imagem == null || Imagem.Length == 0)
            {
                return BadRequest(new { erro = "Nenhuma imagem foi enviada para atualização." });
            }

            try
            {
                await _metadadosDeImagemService.UpdateMetadadosDeImagem(id, Imagem);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                
                if (ex.Message.Contains("não encontrados"))
                {
                    return NotFound(new { erro = ex.Message });
                }
                return BadRequest(new { erro = ex.Message });
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _metadadosDeImagemService.DeletarMetadadosDeImagem(id);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrados"))
                {
                    return NotFound(new { erro = ex.Message });
                }
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}
﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Senai.WebAPI.Domains;
using Senai.WebAPI.Interfaces;
using Senai.WebAPI.Repositorios;

namespace Senai.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class InstituicoesController : ControllerBase
    {
        public readonly IInstituicoesRepository Instituicao;

        public InstituicoesController()
        {
            Instituicao = new InstituicoesRepository();
        }
        
        [HttpGet("listar")]
        public IActionResult ListarInstituicoes() {
            try { 
                return Ok(Instituicao.Listar());
            } catch (Exception exc) {
                return BadRequest(exc.Message);
            }
        }

        [HttpGet("listar/{ID}")]
        public IActionResult BuscarPorID(int ID) {
            try{
                return Ok(Instituicao.Listar(ID));
            } catch (Exception exc) {
                return BadRequest(exc.Message);
            }
        }
        
        [HttpPost("cadastrar")]
        public IActionResult CadastrarInstituicao(InstituicoesDomain instituicao) {
            try {
                Instituicao.Cadastrar(instituicao);
                return Ok("Instituição cadastrada com sucesso");
            } catch (Exception exc){
                return BadRequest(exc.Message);
            }

        }
        [HttpPut("alterar")]
        public IActionResult AtualizarInstituicao(InstituicoesDomain instituicao) {
            try {
                if(instituicao.ID == 0) {
                    throw new Exception("Você precisa especificar qual instituição quer alterar");
                }
                Instituicao.Atualizar(instituicao);
                return Ok("Instituição atualizada com sucesso");
            } catch (Exception exc) {
                return BadRequest(exc.Message);
            }
            
        }

    }
}
﻿using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Senai.WebAPI.Domains;
using Senai.WebAPI.Interfaces;
using Senai.WebAPI.Repositorios;
using Senai.WebAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace Senai.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosRepository repositorio;

        public UsuariosController() {
            repositorio = new UsuariosRepository();
        }

        [HttpPost]
        public IActionResult CadastrarUsuario(UsuariosDomain usuario) {
            try {
                repositorio.Cadastrar(usuario);
                return Ok($"Usuario {usuario.Nome} cadastrado com sucesso");
            } catch (SqlException exc) {
                return BadRequest("Ocorreu um erro no banco de dados\n" + exc.Message);
            } catch (Exception exc) {
                return BadRequest(exc.Message);
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult LogarUsuario(LoginViewModel login) {

            UsuariosDomain usuario = repositorio.Logar(login.Email, login.Senha);

            try {
                if (usuario == null) {
                    return NotFound("Email ou senha incorretos");
                }

                //cria claims que são
                var claims = new[] {
                    // claim : conjunto de chave e valor 
                    new Claim(JwtRegisteredClaimNames.Email,usuario.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,usuario.ID.ToString()),
                    new Claim(ClaimTypes.Role,usuario.TipoUsuario.ToString())
                };

                // Chave de acesso do token
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("svigufo-chave-autenticacao"));

                //Credenciais do Token - Header
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                //Gera o token
                var token = new JwtSecurityToken(
                    issuer: "SviGufo.WebApi",
                    audience: "SviGufo.WebApi",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                return Ok(new {
                    Token = new JwtSecurityTokenHandler().WriteToken(token)//cria o token
                });
            } catch (Exception exc) {
                return BadRequest(exc.Message);
            }
        }

        [HttpGet]
        public IActionResult Listar() {
            try {
                return Ok(repositorio.Listar());
            } catch (SqlException exc) {
                return BadRequest("Ocorreu um erro no banco de dados\n" + exc.Message);
            } catch (Exception exc) {
                return BadRequest(exc.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Listar(int id) {
            try {
                return Ok(repositorio.Listar(id));
            } catch (SqlException exc) {
                return BadRequest("Ocorreu um erro no banco de dados\n" + exc.Message);
            } catch (Exception exc) {
                return BadRequest(exc.Message);
            }
        }
    }
}
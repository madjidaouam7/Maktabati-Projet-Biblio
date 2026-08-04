using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Biblio.Domain.Entities;
using Biblio.Infrastructure.Identity;
using Biblio.Infrastructure.Data;
using Biblio.Api.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Biblio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<Utilisateur> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            var utilisateurExistant = await _userManager.FindByEmailAsync(registerDto.Email);
            if (utilisateurExistant != null)
            {
                return BadRequest("Un utilisateur avec cet email existe déjà.");
            }

            var utilisateur = new Utilisateur
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Nom = registerDto.Nom,
                Prenom = registerDto.Prenom
            };

            var resultat = await _userManager.CreateAsync(utilisateur, registerDto.MotDePasse);

            if (!resultat.Succeeded)
            {
                var erreurs = resultat.Errors.Select(e => e.Description);
                return BadRequest(erreurs);
            }

            if (!await _roleManager.RoleExistsAsync("Lecteur"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Lecteur"));
            }

            await _userManager.AddToRoleAsync(utilisateur, "Lecteur");

            var reponse = await GenererReponseAuth(utilisateur);
            return Ok(reponse);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var utilisateur = await _userManager.FindByEmailAsync(loginDto.Email);
            if (utilisateur == null)
            {
                return Unauthorized("Email ou mot de passe incorrect.");
            }

            var motDePasseValide = await _userManager.CheckPasswordAsync(utilisateur, loginDto.MotDePasse);
            if (!motDePasseValide)
            {
                return Unauthorized("Email ou mot de passe incorrect.");
            }

            var reponse = await GenererReponseAuth(utilisateur);
            return Ok(reponse);
        }




        // POST: api/auth/creer-bibliothecaire
        [HttpPost("creer-bibliothecaire")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreerBibliothecaire(CreerBibliothecaireDto dto)
        {
            var utilisateurExistant = await _userManager.FindByEmailAsync(dto.Email);
            if (utilisateurExistant != null)
            {
                return BadRequest("Un utilisateur avec cet email existe déjà.");
            }

            var utilisateur = new Utilisateur
            {
                UserName = dto.Email,
                Email = dto.Email,
                Nom = dto.Nom,
                Prenom = dto.Prenom
            };

            var resultat = await _userManager.CreateAsync(utilisateur, dto.MotDePasse);

            if (!resultat.Succeeded)
            {
                var erreurs = resultat.Errors.Select(e => e.Description);
                return BadRequest(erreurs);
            }

            await _userManager.AddToRoleAsync(utilisateur, "Bibliothecaire");

            return Ok($"Compte Bibliothécaire créé avec succès pour {dto.Email}.");
        }




        // Méthode privée : génère le token JWT et construit la réponse
        private async Task<AuthResponseDto> GenererReponseAuth(Utilisateur utilisateur)
        {
            var roles = await _userManager.GetRolesAsync(utilisateur);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, utilisateur.Id),
                new Claim(ClaimTypes.Email, utilisateur.Email!),
                new Claim(ClaimTypes.GivenName, utilisateur.Prenom)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var cle = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Cle"]!));
            var credentials = new SigningCredentials(cle, SecurityAlgorithms.HmacSha256);

            var dureeMinutes = int.Parse(_configuration["Jwt:DureeExpirationMinutes"]!);
            var expiration = DateTime.Now.AddMinutes(dureeMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                Email = utilisateur.Email!,
                Nom = utilisateur.Nom,
                Prenom = utilisateur.Prenom,
                Roles = roles.ToList()
            };
        }
    }
}
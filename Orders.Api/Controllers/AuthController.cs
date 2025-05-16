using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Orders.Domain.Models.Dtos;
using Orders.Domain.Services;

namespace Orders.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// POST /api/auth/register
        /// {
        ///     "email": "usuario@exemplo.com",
        ///     "password": "Senha@123",
        ///     "confirmPassword": "Senha@123"
        /// }
        /// </remarks>
        /// <param name="model">Dados de registro do usuário</param>
        /// <returns>Confirmação de registro</returns>
        /// <response code="200">Usuário registrado com sucesso</response>
        /// <response code="400">Se os dados forem inválidos ou o email já existir</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return Ok(new { Message = "Usuário registrado com sucesso" });
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Autentica um usuário e retorna um token JWT
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// POST /api/auth/login
        /// {
        ///     "email": "usuario@exemplo.com",
        ///     "password": "Senha@123"
        /// }
        /// </remarks>
        /// <param name="model">Credenciais de login</param>
        /// <returns>Token JWT para autenticação</returns>
        /// <response code="200">Retorna o token de autenticação</response>
        /// <response code="401">Se as credenciais forem inválidas</response>

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.GenerateToken(user, roles);

                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Email ou senha inválidos" });
        }
    }
}
using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Models;
using Mannys_Cloud_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Mannys_Cloud_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;

        public AuthController(ApplicationDbContext context, IJwtService jwtService, IPasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            try
            {
                // Validate dto
                if (string.IsNullOrEmpty(request.FullName))
                    return BadRequest("Full name is required.");
                if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    return BadRequest("Invalid email format");
                if (string.IsNullOrEmpty(request.Password))
                    return BadRequest("Password is required.");


                // Check if email has been used by existing user
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                    return Forbid("Email already registered.");

                var newUser = new User { FullName = request.FullName, Email = request.Email, PasswordHash = _passwordService.HashPassword(request.Password) };
                _context.Users.Add(newUser);

                // Create root folder for user
                var newFolder = new Folder { UserId = newUser.UserId, FolderName = "root" };
                _context.Folders.Add(newFolder);

                await _context.SaveChangesAsync();

                return Ok("Registration completed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            try {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null) return BadRequest("User is not registered with this email.");

                if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                    return BadRequest("Invalid password");

                var token = _jwtService.GenerateToken(user);
                return Ok(token);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    } }

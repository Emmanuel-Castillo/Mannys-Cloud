using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Models;
using Mannys_Cloud_Backend.Services;
using Mannys_Cloud_Backend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        private readonly ConvertDto _convertDto;

        public AuthController(ApplicationDbContext context, IJwtService jwtService, IPasswordService passwordService, ConvertDto convertDto)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _convertDto = convertDto;
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
                await _context.SaveChangesAsync();

                // Create root folder for user
                var newFolder = new Folder { UserId = newUser.UserId, FolderName = "root", IsRootFolder = true };
                _context.Folders.Add(newFolder);
                await _context.SaveChangesAsync();

                // Return User data and token
                var token = _jwtService.GenerateToken(newUser);
                var userDto = _convertDto.ConvertToUserDto(newUser);
                return Ok(new { success = true, message = "User successfully registered", userData = userDto, token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null) return BadRequest("User is not registered with this email.");

                if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                    return BadRequest("Invalid password");

                var token = _jwtService.GenerateToken(user);
                var userDto = _convertDto.ConvertToUserDto(user);
                return Ok(new { success = true, message = "User successfully logged in", userData = userDto, token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("check")]
        [Authorize]
        public async Task<IActionResult> CheckAuth()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return NotFound();

                var user = await _context.Users.FindAsync(int.Parse(userId));
                if (user == null) return NotFound();

                var userDto = _convertDto.ConvertToUserDto(user);
                return Ok(new { success = true, userData = userDto });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

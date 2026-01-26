using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Models;
using Mannys_Cloud_Backend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Mannys_Cloud_Backend.Controllers
{
    public record AuthResponse
    {
        public readonly bool success;
        public readonly string message;
        public readonly UserDto? userData;
        public readonly string? token;

        public AuthResponse(bool _success, string _message, UserDto? _userData, string? _token)
        {
            success = _success;
            message = _message;
            userData = _userData;
            token = _token;
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            try
            {
                var result = await _authService.Register(request);
                var response = new AuthResponse(true, "User successfully registered", result.userData, result.token);
                return Ok(response);
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
                var result = await _authService.Login(request);
                var response = new AuthResponse(true, "User successfully logged in", result.userData, result.token);
                return Ok(response);
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
                var userId = CheckUser.GrabParsedUserId(User);
                var userDto = await _authService.CheckAuth(userId);
                var response = new AuthResponse(true, "User retrieved.", userDto, null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

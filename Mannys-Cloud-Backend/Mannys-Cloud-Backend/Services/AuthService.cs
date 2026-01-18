using Azure.Core;
using Mannys_Cloud_Backend.Data;
using Mannys_Cloud_Backend.DTO;
using Mannys_Cloud_Backend.DTO.Requests;
using Mannys_Cloud_Backend.Interfaces;
using Mannys_Cloud_Backend.Models;
using Mannys_Cloud_Backend.Util;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Mannys_Cloud_Backend.Services
{
    public record AuthenticatedUserData
    {
        public UserDto userData;
        public string token;

        public AuthenticatedUserData(UserDto userData, string token)
        {
            this.userData = userData;
            this.token = token;
        }
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;

        public AuthService(ApplicationDbContext context, IJwtService jwtService, IPasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        public async Task<UserDto> CheckAuth(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new Exception("User not found!");

                var userDto = ConvertToDto.ConvertToUserDto(user);
                return userDto;
            }
            catch
            {
                throw;
            }
        }

        public async Task<AuthenticatedUserData> Login(LoginUserRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null) throw new Exception("User is not registered with this email.");

                if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                    throw new Exception("Invalid password");

                var token = _jwtService.GenerateToken(user);
                var userDto = ConvertToDto.ConvertToUserDto(user);

                return new AuthenticatedUserData(userDto, token);
            }
            catch
            {
                throw;
            }
        }

        public async Task<AuthenticatedUserData> Register(RegisterUserRequest request)
        { try {
                // Validate dto
                if (string.IsNullOrEmpty(request.FullName))
                    throw new Exception ("Full name is required.");
                if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new Exception("Invalid email format");
                if (string.IsNullOrEmpty(request.Password))
                    throw new Exception("Password is required.");


                // Check if email has been used by existing user
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                    throw new Exception("Email already registered.");

                var newUser = new User { FullName = request.FullName, Email = request.Email, PasswordHash = _passwordService.HashPassword(request.Password) };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Create root folder for user
                var newFolder = new Folder { UserId = newUser.UserId, FolderName = "root", IsRootFolder = true };
                _context.Folders.Add(newFolder);
                await _context.SaveChangesAsync();

                // Return User data and token
                var token = _jwtService.GenerateToken(newUser);
                var userDto = ConvertToDto.ConvertToUserDto(newUser);

                return new AuthenticatedUserData(userDto, token);
            }
            catch{
                throw;
            }
        }
    }
}

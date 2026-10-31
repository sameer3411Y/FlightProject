using Backend.Models;
using Backend.Repositories;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;
        public AuthController(IUserRepository userRepository, EmailService emailService, IConfiguration config)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _config = config;
        }


        public class RegisterRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string? Gender { get; set; }
            public int? Age { get; set; }
            public string? MobileNumber { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var existing = await _userRepository.GetByEmailAsync(req.Email);
            if (existing != null) return BadRequest("Email already exists");
            var user = new User
            {
                Email = req.Email,
                FirstName = req.FirstName,
                Gender = req.Gender,
                Age = req.Age,
                MobileNumber = req.MobileNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            var welcomeHtmlMessage = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Welcome to SkyWings</title>
    <style>
    @keyframes bounce {{ 0%, 20%, 53%, 80%, 100% {{ transform: translate3d(0,0,0); }} 40%, 43% {{ transform: translate3d(0,-30px,0); }} 70% {{ transform: translate3d(0,-15px,0); }} 90% {{ transform: translate3d(0,-4px,0); }} }}
    @keyframes pulse {{ 0% {{ transform: scale(1); }} 50% {{ transform: scale(1.1); }} 100% {{ transform: scale(1); }} }}
    @keyframes fadeInUp {{ from {{ opacity: 0; transform: translateY(30px); }} to {{ opacity: 1; transform: translateY(0); }} }}
    @keyframes slideInRight {{ from {{ opacity: 0; transform: translateX(50px); }} to {{ opacity: 1; transform: translateX(0); }} }}
    @keyframes glow {{ 0% {{ box-shadow: 0 0 5px #10b981; }} 50% {{ box-shadow: 0 0 20px #10b981, 0 0 30px #10b981; }} 100% {{ box-shadow: 0 0 5px #10b981; }} }}
    @keyframes shake {{ 0%, 100% {{ transform: translateX(0); }} 10%, 30%, 50%, 70%, 90% {{ transform: translateX(-5px); }} 20%, 40%, 60%, 80% {{ transform: translateX(5px); }} }}
    .bounce {{ animation: bounce 2s infinite; }}
    .pulse {{ animation: pulse 2s infinite; }}
    .fadeInUp {{ animation: fadeInUp 1s ease-out; }}
    .slideInRight {{ animation: slideInRight 1s ease-out; }}
    .glow {{ animation: glow 2s infinite; }}
    .shake {{ animation: shake 0.5s ease-in-out; }}
    .hover-scale {{ transition: transform 0.3s ease; }}
    .hover-scale:hover {{ transform: scale(1.02); }}
    </style>
</head>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);'>
    <div style='max-width: 650px; margin: 0 auto; background: white; border-radius: 20px; overflow: hidden; box-shadow: 0 20px 40px rgba(0,0,0,0.15);'>
        
        <!-- Animated Header -->
        <div style='background: linear-gradient(135deg, #1e3a8a 0%, #3b82f6 50%, #06b6d4 100%); padding: 40px 30px; text-align: center; position: relative; overflow: hidden;'>
            <div style='position: absolute; top: -30px; right: -30px; width: 80px; height: 80px; background: rgba(255,255,255,0.1); border-radius: 50%;'></div>
            <div style='position: absolute; bottom: -20px; left: -20px; width: 50px; height: 50px; background: rgba(255,255,255,0.1); border-radius: 50%;'></div>
            
            <div class='bounce' style='font-size: 50px; margin-bottom: 10px;'>✈️</div>
            <h1 class='slideInRight' style='color: white; margin: 0; font-size: 36px; font-weight: 800;'>SkyWings</h1>
            <div class='fadeInUp' style='background: rgba(255,255,255,0.2); color: white; padding: 8px 20px; border-radius: 20px; display: inline-block; margin-top: 15px; font-weight: 600;'>🎉 Welcome Aboard!</div>
        </div>
        
        <!-- Content -->
        <div style='padding: 40px 35px;'>
            
            <!-- Success Section -->
            <div class='fadeInUp' style='text-align: center; margin-bottom: 35px;'>
                <div class='pulse glow' style='background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; width: 90px; height: 90px; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center; font-size: 40px; margin-bottom: 20px; box-shadow: 0 10px 30px rgba(16, 185, 129, 0.3);'>👋</div>
                <h2 style='color: #1f2937; margin: 0 0 10px 0; font-size: 28px; font-weight: 700;'>Welcome to SkyWings!</h2>
                <p style='color: #6b7280; font-size: 16px; margin: 0;'>Your account has been successfully created</p>
            </div>
            
            <!-- Account Details Card -->
            <div class='slideInRight hover-scale' style='background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%); border-radius: 16px; padding: 25px; margin-bottom: 30px; border: 1px solid #e5e7eb; position: relative; overflow: hidden;'>
                <div style='position: absolute; top: -15px; right: -15px; width: 60px; height: 60px; background: linear-gradient(135deg, #3b82f6, #1d4ed8); border-radius: 50%; opacity: 0.1;'></div>
                <h3 class='bounce' style='color: #1f2937; margin: 0 0 20px 0; font-size: 20px; font-weight: 600;'>👤 Account Details</h3>
                <table style='width: 100%;'>
                    <tr><td style='padding: 8px 0; color: #4a5568; font-weight: bold;'>Name:</td><td class='pulse' style='padding: 8px 0; color: #1f2937; font-weight: 700;'>{user.FirstName}</td></tr>
                    <tr><td style='padding: 8px 0; color: #4a5568; font-weight: bold;'>Email:</td><td style='padding: 8px 0; color: #1f2937; font-weight: 600;'>{user.Email}</td></tr>
                    <tr><td style='padding: 8px 0; color: #4a5568; font-weight: bold;'>Account Type:</td><td style='padding: 8px 0; color: #1f2937; font-weight: 600;'>{user.Role}</td></tr>
                    <tr><td style='padding: 8px 0; color: #4a5568; font-weight: bold;'>Registration Date:</td><td style='padding: 8px 0; color: #1f2937; font-weight: 600;'>{DateTime.Now:dd MMM yyyy}</td></tr>
                </table>
            </div>
            
            <!-- Getting Started -->
            <div class='fadeInUp' style='margin-bottom: 30px;'>
                <h3 class='bounce' style='color: #1f2937; margin: 0 0 25px 0; font-size: 22px; font-weight: 700;'>🚀 Getting Started</h3>
                <div class='hover-scale fadeInUp' style='background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%); border-radius: 16px; padding: 25px; margin-bottom: 20px; border-left: 5px solid #0ea5e9; box-shadow: 0 4px 15px rgba(0,0,0,0.1);'>
                    <ul style='color: #0f172a; margin: 0; padding-left: 20px; line-height: 1.8;'>
                        <li style='margin-bottom: 10px;'>🔍 <strong>Search Flights:</strong> Find the perfect flight for your journey</li>
                        <li style='margin-bottom: 10px;'>💺 <strong>Choose Seats:</strong> Select your preferred seats</li>
                        <li style='margin-bottom: 10px;'>💳 <strong>Secure Payment:</strong> Complete booking with our secure payment system</li>
                        <li>📧 <strong>Get Confirmation:</strong> Receive instant booking confirmations</li>
                    </ul>
                </div>
            </div>
            
            <!-- Important Notes -->
            <div class='slideInRight hover-scale' style='background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%); border: 2px solid #f59e0b; border-radius: 16px; padding: 25px; margin-bottom: 30px;'>
                <h3 class='shake' style='color: #92400e; margin: 0 0 15px 0; font-size: 18px; font-weight: 700;'>💡 Quick Tips</h3>
                <ul style='color: #78350f; margin: 0; padding-left: 20px; line-height: 1.6;'>
                    <li style='margin-bottom: 8px;'>🔐 Keep your login credentials secure</li>
                    <li style='margin-bottom: 8px;'>📱 Download our mobile app for easy booking</li>
                    <li style='margin-bottom: 8px;'>🎯 Set up flight alerts for your favorite routes</li>
                    <li>💰 Check for special offers and discounts</li>
                </ul>
            </div>
            
            <!-- Footer -->
            <div class='fadeInUp' style='background: linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%); padding: 25px; border-radius: 16px; text-align: center; border: 1px solid #cbd5e1;'>
                <div class='bounce' style='font-size: 24px; margin-bottom: 10px;'>✨</div>
                <h4 style='color: #1f2937; margin: 0 0 8px 0; font-size: 18px; font-weight: 700;'>Ready to Fly?</h4>
                <p style='color: #64748b; margin: 0; font-size: 15px;'>Start exploring amazing destinations! ✈️🌍</p>
            </div>
            
        </div>
    </div>
    
    <!-- Footer Note -->
    <div style='text-align: center; margin-top: 20px; color: rgba(255,255,255,0.8); font-size: 14px;'>
        Welcome email sent to {user.Email} | SkyWings Flight System
    </div>
    
</body>
</html>";
            
            await _emailService.SendEmailAsync(user.Email, "Welcome to SkyWings - Your Account is Ready!", welcomeHtmlMessage);
            return Ok("Registration successful");
        }


        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
        
        public class ForgotPasswordRequest
        {
            public string Email { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userRepository.GetByEmailAsync(req.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");
            var token = GenerateJwtToken(user);
            return Ok(new { 
                token, 
                user = new { 
                    id = user.Id, 
                    email = user.Email, 
                    firstName = user.FirstName, 
                    lastName = user.LastName,
                    gender = user.Gender,
                    age = user.Age,
                    mobileNumber = user.MobileNumber,
                    role = user.Role,
                    isActive = user.IsActive,
                    createdAt = user.CreatedAt
                } 
            });
        }

        // In-memory OTP storage (use Redis or database in production)
        private static readonly Dictionary<string, (string Otp, DateTime Expiry)> _otpStorage = new();
        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null) return Ok("If the email exists, an OTP has been sent");
            
            // Generate 6-digit OTP using cryptographically secure random number generator
            var otp = GenerateSecureOtp();
            var expiry = DateTime.UtcNow.AddMinutes(10); // OTP expires in 10 minutes
            
            // Store OTP
            _otpStorage[request.Email] = (otp, expiry);
            
            var otpHtmlMessage = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Your SkyWings Password Reset OTP</title>
    <style>
    @keyframes bounce {{ 0%, 20%, 53%, 80%, 100% {{ transform: translate3d(0,0,0); }} 40%, 43% {{ transform: translate3d(0,-30px,0); }} 70% {{ transform: translate3d(0,-15px,0); }} 90% {{ transform: translate3d(0,-4px,0); }} }}
    @keyframes pulse {{ 0% {{ transform: scale(1); }} 50% {{ transform: scale(1.1); }} 100% {{ transform: scale(1); }} }}
    @keyframes fadeInUp {{ from {{ opacity: 0; transform: translateY(30px); }} to {{ opacity: 1; transform: translateY(0); }} }}
    @keyframes slideInRight {{ from {{ opacity: 0; transform: translateX(50px); }} to {{ opacity: 1; transform: translateX(0); }} }}
    @keyframes glow {{ 0% {{ box-shadow: 0 0 5px #ef4444; }} 50% {{ box-shadow: 0 0 20px #ef4444, 0 0 30px #ef4444; }} 100% {{ box-shadow: 0 0 5px #ef4444; }} }}
    .bounce {{ animation: bounce 2s infinite; }}
    .pulse {{ animation: pulse 2s infinite; }}
    .fadeInUp {{ animation: fadeInUp 1s ease-out; }}
    .slideInRight {{ animation: slideInRight 1s ease-out; }}
    .glow {{ animation: glow 2s infinite; }}
    .hover-scale {{ transition: transform 0.3s ease; }}
    </style>
</head>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);'>
    <div style='max-width: 650px; margin: 0 auto; background: white; border-radius: 20px; overflow: hidden; box-shadow: 0 20px 40px rgba(0,0,0,0.15);'>
        
        <!-- Animated Header -->
        <div style='background: linear-gradient(135deg, #dc2626 0%, #ef4444 50%, #f97316 100%); padding: 40px 30px; text-align: center; position: relative; overflow: hidden;'>
            <div style='position: absolute; top: -30px; right: -30px; width: 80px; height: 80px; background: rgba(255,255,255,0.1); border-radius: 50%;'></div>
            <div style='position: absolute; bottom: -20px; left: -20px; width: 50px; height: 50px; background: rgba(255,255,255,0.1); border-radius: 50%;'></div>
            
            <div class='bounce' style='font-size: 50px; margin-bottom: 10px;'>🔐</div>
            <h1 class='slideInRight' style='color: white; margin: 0; font-size: 36px; font-weight: 800;'>SkyWings</h1>
            <div class='fadeInUp' style='background: rgba(255,255,255,0.2); color: white; padding: 8px 20px; border-radius: 20px; display: inline-block; margin-top: 15px; font-weight: 600;'>🔑 Password Reset OTP</div>
        </div>
        
        <!-- Content -->
        <div style='padding: 40px 35px;'>
            
            <!-- OTP Section -->
            <div class='fadeInUp' style='text-align: center; margin-bottom: 35px;'>
                <div class='pulse glow' style='background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%); color: white; width: 90px; height: 90px; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center; font-size: 40px; margin-bottom: 20px; box-shadow: 0 10px 30px rgba(239, 68, 68, 0.3);'>🔢</div>
                <h2 style='color: #1f2937; margin: 0 0 10px 0; font-size: 28px; font-weight: 700;'>Your Reset OTP</h2>
                <p style='color: #6b7280; font-size: 16px; margin: 0;'>Use this code to reset your password</p>
            </div>
            
            <!-- OTP Code -->
            <div class='slideInRight hover-scale' style='background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%); border-radius: 16px; padding: 25px; margin-bottom: 30px; border: 1px solid #e5e7eb; text-align: center;'>
                <h3 class='bounce' style='color: #1f2937; margin: 0 0 20px 0; font-size: 20px; font-weight: 600;'>Your OTP Code</h3>
                <div class='pulse glow' style='background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%); color: white; padding: 20px; border-radius: 15px; font-size: 36px; font-weight: bold; letter-spacing: 8px; margin: 20px 0; display: inline-block; box-shadow: 0 8px 20px rgba(239, 68, 68, 0.3);'>{otp}</div>
                <p style='color: #6b7280; font-size: 14px; margin: 10px 0 0 0;'>This code expires in <strong>10 minutes</strong></p>
            </div>
            
            <!-- Instructions -->
            <div class='fadeInUp' style='margin-bottom: 30px;'>
                <h3 class='bounce' style='color: #1f2937; margin: 0 0 15px 0; font-size: 18px; font-weight: 700;'>📝 How to use:</h3>
                <ol style='color: #4b5563; margin: 0; padding-left: 20px; line-height: 1.8;'>
                    <li style='margin-bottom: 8px;'>Go to the password reset page</li>
                    <li style='margin-bottom: 8px;'>Enter this 6-digit OTP code</li>
                    <li style='margin-bottom: 8px;'>Create your new password</li>
                    <li>Login with your new password</li>
                </ol>
            </div>
            
            <!-- Security Notice -->
            <div class='slideInRight hover-scale' style='background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%); border: 2px solid #f59e0b; border-radius: 16px; padding: 25px; margin-bottom: 30px;'>
                <h3 style='color: #92400e; margin: 0 0 15px 0; font-size: 18px; font-weight: 700;'>🔒 Security Notice</h3>
                <ul style='color: #78350f; margin: 0; padding-left: 20px; line-height: 1.6;'>
                    <li style='margin-bottom: 8px;'>This OTP expires in 10 minutes</li>
                    <li style='margin-bottom: 8px;'>Never share this code with anyone</li>
                    <li style='margin-bottom: 8px;'>If you didn't request this, ignore this email</li>
                    <li>Contact support if you need help</li>
                </ul>
            </div>
            
            <!-- Footer -->
            <div class='fadeInUp' style='background: linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%); padding: 25px; border-radius: 16px; text-align: center; border: 1px solid #cbd5e1;'>
                <div class='bounce' style='font-size: 24px; margin-bottom: 10px;'>🔐</div>
                <h4 style='color: #1f2937; margin: 0 0 8px 0; font-size: 18px; font-weight: 700;'>Need Help?</h4>
                <p style='color: #64748b; margin: 0; font-size: 15px;'>Contact our support team anytime! 📞💬</p>
            </div>
        </div>
    </div>
    
    <!-- Footer Note -->
    <div style='text-align: center; margin-top: 20px; color: rgba(255,255,255,0.8); font-size: 14px;'>
        OTP sent to {user.Email} | SkyWings Flight System
    </div>
</body>
</html>";
            
            await _emailService.SendEmailAsync(user.Email, "Your SkyWings Password Reset OTP", otpHtmlMessage);
            return Ok("If the email exists, an OTP has been sent");
        }
        
        public class VerifyOtpRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Otp { get; set; } = string.Empty;
        }
        
        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (!_otpStorage.ContainsKey(request.Email))
                return BadRequest("Invalid or expired OTP");
                
            var (storedOtp, expiry) = _otpStorage[request.Email];
            
            if (DateTime.UtcNow > expiry)
            {
                _otpStorage.Remove(request.Email);
                return BadRequest("OTP has expired");
            }
            
            if (storedOtp != request.Otp)
                return BadRequest("Invalid OTP");
                
            // OTP is valid, remove it and allow password reset
            _otpStorage.Remove(request.Email);
            return Ok("OTP verified successfully");
        }
        
        public class ResetPasswordRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Otp { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }
        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // Verify OTP first
            if (!_otpStorage.ContainsKey(request.Email))
                return BadRequest("Invalid or expired OTP");
                
            var (storedOtp, expiry) = _otpStorage[request.Email];
            
            if (DateTime.UtcNow > expiry || storedOtp != request.Otp)
            {
                _otpStorage.Remove(request.Email);
                return BadRequest("Invalid or expired OTP");
            }
            
            // Get user and update password
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return BadRequest("User not found");
                
            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);
            
            // Remove OTP after successful reset
            _otpStorage.Remove(request.Email);
            
            return Ok("Password reset successfully");
        }

        private static string GenerateSecureOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomNumber = BitConverter.ToUInt32(bytes, 0);
            return (randomNumber % 900000 + 100000).ToString();
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _config["Jwt:Key"] ?? "ThisIsASecretKeyForJwtToken";
            var jwtIssuer = _config["Jwt:Issuer"] ?? "FlightApp";
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

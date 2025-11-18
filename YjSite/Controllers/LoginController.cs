using Dm.filter;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using YjSite.ViewModel;
using SqlSugar;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Site.Domain.Entities;
using NetTaste;
using YjSite.DTOs;

namespace YjSite.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ISqlSugarClient _sql;
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config, ISqlSugarClient context)
        {
            _sql = context;
            _config = config;
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult LoginAsync(LoginDto loginDto)
        {
            LoginView auther = new();
            #region 校验用户信息，
            if (loginDto.PrivateKey != "")
            {
                var pkUser = _sql.Queryable<User>().Where(it => it.Pk == loginDto.PrivateKey).First();
                if(pkUser == null) return Ok(JsonView(false, "私钥错误"));
                auther = SetCookie(pkUser);
                return Ok(JsonView(auther, "登录成功"));
            }
            var user = _sql.Queryable<User>().Where(it => it.Account == loginDto.UserName).First();
            //var user = sql.Queryable().Where(it => it.UserName == loginDto.UserName).First();

            if(user == null)
            {
                return Ok(JsonView(false, "用户不存在"));
            }
            var pwdSalt = user.Password.Split("~");
            var pwd = pwdSalt[0];
            var salt = pwdSalt[1];
            var pwdHash = HashPassword(loginDto.Password, salt);
            if (pwdHash != pwd)
            {
                return Ok(JsonView(false, "密码错误"));
            }
            #endregion
            auther = SetCookie(user);
            return Ok(JsonView(auther, "登录成功"));
        }

        [Authorize]
        [HttpGet("verify-token")]
        public IActionResult VerifyAsync()
        {
            return Ok(JsonView(true, "验证成功"));
        }

        private LoginView SetCookie(User user)
        {
            LoginView auther = new();
            var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                };
            var token = GenerateJwtToken(claims);
            Response.Cookies.Append("x-access-token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(1)
            });
            auther.Avatar = user.Avatar;
            auther.Name = user.UserName;
            auther.Id = user.Id;
            auther.Expires = DateTime.Now.AddDays(1);
            auther.Token = token;
            return auther;
        }

        private static string HashPassword(string value, string salt)
        {
            byte[] valueByte = Encoding.Default.GetBytes(value);
            byte[] saltByte = Encoding.Default.GetBytes(salt);
            var valueBytes = new Rfc2898DeriveBytes(valueByte, saltByte, 100, HashAlgorithmName.SHA1);//最后输出的秘钥长度
            return Convert.ToBase64String(valueBytes.GetBytes(128));
        }

        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var view = new LoginView
            {
                Expires = DateTime.Now.AddDays(1)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "yjcabin.com",
                audience: "yjcabin.com",
                claims: claims,
                expires: view.Expires,
                signingCredentials: creds);
            return view.Token = new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

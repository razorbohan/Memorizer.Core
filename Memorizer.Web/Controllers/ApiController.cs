using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Memorizer.Logic;
using Memorizer.Web.Models;
using Memorizer.Web.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Memo = Memorizer.Data.Models.Memo;

namespace Memorizer.Web.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private IMemorizerLogic MemorizerLogic { get; }
        private SignInManager<IdentityUser> SignInManager { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private IOptions<JwtAuthentication> JwtAuthentication { get; }

        public ApiController(IMemorizerLogic memorizerLogic,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<JwtAuthentication> jwtAuthentication)
        {
            MemorizerLogic = memorizerLogic;
            SignInManager = signInManager;
            UserManager = userManager;
            JwtAuthentication = jwtAuthentication;
        }

        // POST: api/Authenticate
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<ApiResponse<AuthBody>> Authenticate([FromBody]LoginInputModel input)
        {
            try
            {
                if (!ModelState.IsValid) throw new Exception("Wrong login data!");

                var result = await SignInManager.PasswordSignInAsync(input.Email, input.Password, input.RememberMe, false);
                if (!result.Succeeded) throw new Exception("Failed to authenticate!");

                var user = await UserManager.FindByEmailAsync(input.Email);

                var tokenString = GenerateToken(user);
                return new ApiResponse<AuthBody>(success: true, body: new AuthBody { Username = user.UserName, Token = tokenString });
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthBody>(success: false, error: ex.Message);
            }
        }

        // POST: api/Register
        [HttpPost("Register")]
        public async Task<ApiResponse> Register([FromBody]RegisterInputModel input)
        {
            try
            {
                if (!ModelState.IsValid) throw new Exception("Wrong register data!");

                var user = new IdentityUser { UserName = input.Email, Email = input.Email };
                var result = await UserManager.CreateAsync(user, input.Password);
                if (!result.Succeeded) throw new Exception("Failed to register!");

                var tokenString = GenerateToken(user);
                return new ApiResponse<string>(success: true, body: tokenString);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(success: false, error: ex.Message);
            }
        }

        // GET: api/CheckEmail/email@mail.com
        [AllowAnonymous]
        [HttpGet("CheckEmail/{email}")]
        public async Task<ApiResponse> Authenticate(string email)
        {
            try
            {
                var user = await UserManager.FindByEmailAsync(email);
                return user != null ? new ApiResponse(success: true) : new ApiResponse(success: false);
            }
            catch (Exception ex)
            {
                return new ApiResponse(success: false, error: ex.Message);
            }
        }

        // GET: api/GetMemos
        [HttpGet("GetMemos")]
        public async Task<ApiResponse<IEnumerable<Memo>>> GetMemos()
        {
            try
            {
                var memos = await MemorizerLogic.GetMemos().ToListAsync();
                return new ApiResponse<IEnumerable<Memo>>(success: true, body: memos);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<Memo>>(success: false, error: ex.Message);
            }
        }

        // GET: api/GetMemo/5
        [HttpGet("GetMemo/{id}")]
        public async Task<ApiResponse<Memo>> GetMemo(int id)
        {
            try
            {
                var memo = await Task.Run(() => MemorizerLogic.GetMemo(id));
                return memo == null
                    ? new ApiResponse<Memo>(success: false, error: "Memo not found")
                    : new ApiResponse<Memo>(success: true, body: memo);
            }
            catch (Exception ex)
            {
                return new ApiResponse<Memo>(success: false, error: ex.Message);
            }
        }

        // POST: api/UpdateMemo
        [HttpPost("UpdateMemo")]
        public async Task<ApiResponse> UpdateMemo([FromBody] Memo memo)
        {
            try
            {
                await Task.Run(() => MemorizerLogic.UpdateMemo(memo));
            }
            catch (Exception ex)
            {
                return !MemoExists(memo.Id)
                    ? new ApiResponse(success: false, error: "Memo not found")
                    : new ApiResponse(success: false, error: ex.Message);
            }

            return new ApiResponse(success: true);
        }

        // POST: api/AddMemo
        [HttpPost("AddMemo")]
        public async Task<ApiResponse> AddMemo([FromBody] Memo memo)
        {
            try
            {
                await Task.Run(() => MemorizerLogic.AddMemo(memo));
            }
            catch (Exception ex)
            {
                return new ApiResponse(success: false, error: ex.Message);
            }

            return new ApiResponse(success: true);
        }

        // POST: api/DeleteMemo
        [HttpPost("DeleteMemo")]
        public async Task<ApiResponse> DeleteMemo([FromBody] int id)
        {
            try
            {
                await Task.Run(() => MemorizerLogic.DeleteMemo(id));
            }
            catch (Exception ex)
            {
                return !MemoExists(id)
                    ? new ApiResponse(success: false, error: "Memo not found")
                    : new ApiResponse(success: false, error: ex.Message);
            }

            return new ApiResponse(success: true);
        }

        //POST: api/SubmitAnswer
        [HttpPost("SubmitAnswer")]
        public async Task<ApiResponse> SubmitAnswer([FromBody] MemoAnswer memoAnswer)
        {
            try
            {
                await Task.Run(() => MemorizerLogic.SubmitAnswer(memoAnswer.Id, memoAnswer.AnswerType));
            }
            catch (Exception ex)
            {
                return new ApiResponse(success: false, error: ex.Message);
            }

            return new ApiResponse(success: true);
        }

        // GET: api/FindMemo/green
        [HttpGet("FindMemo/{key?}/{value?}")]
        public async Task<ApiResponse<IEnumerable<Memo>>> FindMemo(string key, string value)
        {
            var memos = await Task.Run(() => MemorizerLogic.FindMemo(key, value));
            //if (!memos.Any())
            //{
            //    return NotFound();
            //}

            return new ApiResponse<IEnumerable<Memo>>(success: true, body: memos);
        }

        private bool MemoExists(int id)
        {
            return MemorizerLogic.GetMemos().Any(e => e.Id == id);
        }

        private string GenerateToken(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = JwtAuthentication.Value.ValidIssuer,
                Audience = JwtAuthentication.Value.ValidAudience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = JwtAuthentication.Value.SigningCredentials
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
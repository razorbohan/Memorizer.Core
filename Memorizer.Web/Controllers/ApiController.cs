using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Memorizer.Logic;
using Memorizer.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Memo = Memorizer.Data.Models.Memo;

namespace Memorizer.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private IMemorizerLogic MemorizerLogic { get; }
        private SignInManager<IdentityUser> SignInManager { get; }
        private UserManager<IdentityUser> UserManager { get; }

        public ApiController(IMemorizerLogic memorizerLogic, 
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            MemorizerLogic = memorizerLogic;
            SignInManager = signInManager;
            UserManager = userManager;
        }

        // POST: api/Authenticate
        [HttpPost("Authenticate")]
        public async Task<ApiResponse<string>> Authenticate([FromBody]LoginInputModel input)
        {
            try
            {
                var result = await SignInManager.PasswordSignInAsync(input.Email, input.Password, input.RememberMe, false);
                if (!result.Succeeded) throw new Exception("Failed to authenticate!");

                var user = UserManager.FindByEmailAsync(input.Email);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("secret_Key_123123123sdfsdfsdfsdf");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new ApiResponse<string>(success: true, body: tokenString);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(success: false, error: ex.Message);
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
    }
}
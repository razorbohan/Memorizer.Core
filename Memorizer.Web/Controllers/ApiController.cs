using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Memorizer.Logic;
using Memorizer.Web.Models;
using Memo = Memorizer.Data.Models.Memo;

namespace Memorizer.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private IMemorizerLogic MemorizerLogic { get; }

        public ApiController(IMemorizerLogic memorizerLogic)
        {
            MemorizerLogic = memorizerLogic;
        }

        // GET: api/GetMemos
        [HttpGet("GetMemos")]
        public async Task<IEnumerable<Memo>> GetMemos()
        {
            return await MemorizerLogic.GetMemos().ToListAsync();
        }

        // GET: api/GetMemo/5
        [HttpGet("GetMemo/{id}")]
        public async Task<IActionResult> GetMemo(int id)
        {
            var memo = await Task.Run(() => MemorizerLogic.GetMemo(id));
            if (memo == null)
            {
                return NotFound();
            }

            return Ok(memo);
        }

        // POST: api/UpdateMemo
        [HttpPost("UpdateMemo")]
        public async Task<IActionResult> UpdateMemo([FromBody] Memo memo)
        {
            try
            {
                await Task.Run(() => MemorizerLogic.UpdateMemo(memo));
            }
            catch (Exception)
            {
                if (!MemoExists(memo.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/AddMemo
        [HttpPost("AddMemo")]
        public async Task<IActionResult> AddMemo([FromBody] Memo memo)
        {
            await Task.Run(() => MemorizerLogic.AddMemo(memo));

            return CreatedAtAction("GetMemo", new { id = memo.Id }, memo);
        }

        // POST: api/DeleteMemo
        [HttpPost("DeleteMemo")]
        public async Task<IActionResult> DeleteMemo([FromBody] int id)
        {
            try
            {
                await Task.Run(() => MemorizerLogic.DeleteMemo(id));
            }
            catch (Exception)
            {
                if (!MemoExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        //POST: api/SubmitAnswer
        [HttpPost("SubmitAnswer")]
        public async Task<IActionResult> SubmitAnswer([FromBody] MemoAnswer memoAnswer)
        {
            await Task.Run(() => MemorizerLogic.SubmitAnswer(memoAnswer.Id, memoAnswer.AnswerType));

            return NoContent();
        }

        // GET: api/FindMemo/green
        [HttpGet("FindMemo/{key?}/{value?}")]
        public async Task<IActionResult> FindMemo(string key, string value)
        {
            var memos = await Task.Run(() => MemorizerLogic.FindMemo(key, value));
            if (!memos.Any())
            {
                return NotFound();
            }

            return Ok(memos);
        }

        private bool MemoExists(int id)
        {
            return MemorizerLogic.GetMemos().Any(e => e.Id == id);
        }
    }
}
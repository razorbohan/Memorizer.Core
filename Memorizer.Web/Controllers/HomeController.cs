using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Memorizer.Logic;
using Memorizer.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Memorizer.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IMemorizerLogic MemorizerLogic { get; }

        public HomeController(IMemorizerLogic memorizerLogic)
        {
            MemorizerLogic = memorizerLogic;
        }

        public IActionResult Index()
        {
            Queue<int> memos;

            if (HttpContext.Session.GetString("Memos") == null)
            {
                var isRepeat = HttpContext.Session.GetString("Mode") != "Learn";

                memos = new Queue<int>(MemorizerLogic.GetMemoIds(isRepeat));
                HttpContext.Session.SetString("Memos", JsonConvert.SerializeObject(memos));
                HttpContext.Session.SetString("Mode", isRepeat ? "Learn" : "Repeat");
            }
            else memos = JsonConvert.DeserializeObject<Queue<int>>(HttpContext.Session.GetString("Memos"));

            ViewBag.Mode = HttpContext.Session.GetString("Mode");
            ViewBag.Overall = memos.Count;

            if (memos.Count == 0)
            {
                return View();
            }

            var currentMemo = Mapper.Map<Memo>(MemorizerLogic.GetMemo(memos.Peek()));

            return View(currentMemo);
        }

        [HttpPost]
        public IActionResult Index(MemoAnswerType answer)
        {
            //var zero = 0;
            //var tt = 6 / zero;
            var memoQueue = JsonConvert.DeserializeObject<Queue<int>>(HttpContext.Session.GetString("Memos"));
            MemorizerLogic.SubmitAnswer(memoQueue.Dequeue(), answer);
            HttpContext.Session.SetString("Memos", JsonConvert.SerializeObject(memoQueue));

            return Index();
        }

        [HttpGet("Home/{mode}")]
        public IActionResult SwitchMode(string mode)
        {
            HttpContext.Session.Remove("Memos");
            HttpContext.Session.SetString("Mode", mode);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public string Update([FromForm] Memo memo)
        {
            try
            {
                MemorizerLogic.UpdateMemo(Mapper.Map<Data.Models.Memo>(memo));

                return "Updated!";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [HttpPost]
        public string Add([FromForm] Memo memo)
        {
            try
            {
                MemorizerLogic.AddMemo(Mapper.Map<Data.Models.Memo>(memo));

                return "Added!";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string Delete(int id)
        {
            try
            {
                MemorizerLogic.DeleteMemo(id);

                var memoQueue = GetMemoQueue();
                memoQueue.Dequeue();
                SetMemoQueue(memoQueue);

                return "Deleted!";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [HttpGet("Home/Find/{key?}/{value?}")]
        public IEnumerable<Data.Models.Memo> Find(string key, string value)
        {
            var foundMemos = MemorizerLogic.FindMemo(key, value).ToList();

            return foundMemos;
        }

        private Queue<int> GetMemoQueue()
        {
            return JsonConvert.DeserializeObject<Queue<int>>(HttpContext.Session.GetString("Memos"));
        }

        private void SetMemoQueue(Queue<int> memoQueue)
        {
            HttpContext.Session.SetString("Memos", JsonConvert.SerializeObject(memoQueue));
        }
    }
}

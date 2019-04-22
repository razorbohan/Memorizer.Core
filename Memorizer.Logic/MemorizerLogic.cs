using System;
using System.Collections.Generic;
using System.Linq;
using Memorizer.Data;
using Memorizer.Data.Models;

namespace Memorizer.Logic
{
    public enum MemoAnswerType
    {
        Bad = 1,
        Tomorrow,
        Later, 
        Cool
    }

    public enum Postpone
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 6,
        Four = 15,
        Five = 40,
        Six = 100,
        Seven = 250,
        Eight = 625
    }

    public interface IMemorizerLogic
    {
        IQueryable<Memo> GetMemos();
        IEnumerable<int> GetMemoIds(bool isRepeat);
        Memo GetMemo(int id);
        void SubmitAnswer(int id, MemoAnswerType memoAnswerType);
        void UpdateMemo(Memo memo);
        void AddMemo(Memo memo);
        IEnumerable<Memo> FindMemo(string key, string value);
        void DeleteMemo(int id);
    }

    public class MemorizerLogic : IMemorizerLogic
    {
        private IRepository<Memo> MemoRepository { get; }

        public MemorizerLogic(IRepository<Memo> memoRepository)
        {
            MemoRepository = memoRepository;
        }

        public IQueryable<Memo> GetMemos()
        {
            var memos = MemoRepository.GetMemos();

            return memos;
        }

        public IEnumerable<int> GetMemoIds(bool isRepeat)
        {
            var memos = GetMemos()
                .Where(memo => memo.RepeatDate < DateTime.Now)
                .Where(memo => isRepeat ? memo.PostponeLevel != 0 : memo.PostponeLevel == 0)
                .OrderByDescending(memo => memo.RepeatDate)
                .ThenByDescending(memo => memo.PostponeLevel)
                .Select(memo => memo.Id);

            return memos;
        }

        public Memo GetMemo(int id)
        {
            return MemoRepository.GetMemo(id);
        }

        public void UpdateMemo(Memo memo)
        {
            MemoRepository.Update(memo);
        }

        public void AddMemo(Memo memo)
        {
            memo.Question = memo.Question.Trimize();
            memo.Answer = memo.Answer.Trimize();
            memo.RepeatDate = Extensions.GetTomorrow(0);

            MemoRepository.Add(memo);
        }

        public IEnumerable<Memo> FindMemo(string key, string value)
        {
            var memos = MemoRepository.Find(value, key);

            return memos;
        }

        public void DeleteMemo(int id)
        {
            var memo = MemoRepository.GetMemo(id);

            MemoRepository.Delete(memo);
        }

        public virtual void SubmitAnswer(int id, MemoAnswerType memoAnswerType)
        {
            var currentMemo = MemoRepository.GetMemo(id);

            switch (memoAnswerType)
            {
                case MemoAnswerType.Bad:
                    currentMemo.RepeatDate = Extensions.GetTomorrow();
                    currentMemo.PostponeLevel = 0;
                    currentMemo.Scores++;
                    break;
                case MemoAnswerType.Tomorrow:
                    currentMemo.RepeatDate = Extensions.GetTomorrow();
                    currentMemo.Scores++;
                    break;
                case MemoAnswerType.Later:
                    //currentMemo.RepeatDate = DateTime.Now.AddMinutes(1);
                    break;
                case MemoAnswerType.Cool:
                    var nextPostponeLevel = ((Postpone)currentMemo.PostponeLevel).NextLevel();
                    currentMemo.RepeatDate = Extensions.GetTomorrow((int)nextPostponeLevel);
                    currentMemo.PostponeLevel = (int)nextPostponeLevel;
                    currentMemo.Scores++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            MemoRepository.Update(currentMemo);
        }
    }
}

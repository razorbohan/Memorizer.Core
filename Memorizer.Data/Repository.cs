using System.Linq;
using Memorizer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Memorizer.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetMemos();
        T GetMemo(int id);
        IQueryable<T> Find(string value, string key = null);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }

    public class Repository : IRepository<Memo>
    {
        private MemoContext Context { get; }
        private DbSet<Memo> Entities { get; }

        public Repository(MemoContext context)
        {
            Context = context;
            Entities = context.Set<Memo>();
        }

        public IQueryable<Memo> GetMemos()
        {
            return Entities;
        }

        public Memo GetMemo(int id)
        {
            return Entities.FirstOrDefault(memo => memo.Id == id);
        }

        public IQueryable<Memo> Find(string value, string key = null)
        {
            if (string.IsNullOrEmpty(value))
                return from memo in Entities select memo;

            if (string.IsNullOrEmpty(key))
            {
                return from memo in Entities
                       where memo.Question.Contains(value) || memo.Answer.Contains(value)
                       select memo;

            }

            return from memo in Entities
                   where GetPropertyValue(memo, key).Equals(value)
                   select memo;
        }

        public void Add(Memo entity)
        {
            Entities.Add(entity);

            Context.SaveChanges();
        }

        public void Update(Memo entity)
        {
            Context.Memos.Update(entity);
            Context.SaveChanges();
        }

        public void Delete(Memo entity)
        {
            Entities.Remove(entity);
            Context.SaveChanges();
        }

        private string GetPropertyValue(Memo memo, string key)
        {
            switch (key.ToLower())
            {
                case "id" : return memo.Id.ToString();
                case "question": return memo.Question;
                case "answer": return memo.Answer;
                case "postponelevel": return memo.PostponeLevel.ToString();
                case "repeatdate": return memo.RepeatDate.Date.ToString("dd.MM.yyyy");
                case "scores": return memo.Scores.ToString();
            }

            return null;
        }
    }
}

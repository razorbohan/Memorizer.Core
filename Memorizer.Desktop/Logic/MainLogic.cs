using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Memorizer.Model;
using Memorizer.Utility;
using File = System.IO.File;

namespace Memorizer.Logic
{
    public class MainLogic : INotifyPropertyChanged
    {
        private ApiController ApiController { get; }
        private Queue<Memo> Memos { get; set; }
        public Memo CurrentMemo { get; private set; }
        public bool IsRepeat { get; private set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;
        private int _memoLeftCount;
        public int MemoLeftCount
        {
            get => _memoLeftCount;
            set
            {
                _memoLeftCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MemoLeftCount)));
            }
        }

        public MainLogic()
        {
            ApiController = new ApiController();
        }

        public async Task<bool> InitializeMemos(bool isRepeat)
        {
            try
            {
                Memos = new Queue<Memo>(await ApiController.GetMemos(isRepeat));
                MemoLeftCount = Memos.Count();

                if (MemoLeftCount > 0)
                {
                    GetNewMemo();
                    IsRepeat = isRepeat;

                    return true;
                }

                Logger.Log(Extensions.GetLocalizedValue("MessageNotFound"));
                
                return false;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
                return false;
            }
        }

        private void GetNewMemo()
        {
            try
            {
                CurrentMemo = Memos.Dequeue();
                MemoLeftCount--;
            }
            catch (InvalidOperationException)
            {
                CurrentMemo = null;
                Logger.Log($"{(IsRepeat ? Extensions.GetLocalizedValue("LoggerFinishedRepeating") : Extensions.GetLocalizedValue("LoggerFinishedLearning"))}!");
            }
        }

        public async Task SubmitAnswer(MemoAnswerType memoAnswerType)
        {
            try
            {
                if (CurrentMemo == null)
                    return;

                await ApiController.SubmitAnswer(CurrentMemo.Id, memoAnswerType);

                GetNewMemo();
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
            }
        }

        public async Task Update(string question, string answer)
        {
            try
            {
                if (CurrentMemo == null)
                    return;

                if (string.IsNullOrEmpty(question) && string.IsNullOrEmpty(answer))
                    MessageBox.Show(
                        Extensions.GetLocalizedValue("MessageDefineWhatToUpdate"),
                        Extensions.GetLocalizedValue("MessageWarning"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                {
                    CurrentMemo.Question = !string.IsNullOrEmpty(question) ? question : CurrentMemo.Question;
                    CurrentMemo.Answer = !string.IsNullOrEmpty(answer) ? answer : CurrentMemo.Answer;

                    await ApiController.Update(CurrentMemo);

                    Logger.Log($"{Extensions.GetLocalizedValue("LoggerUpdated")}: {CurrentMemo.Id}");
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
            }
        }

        public async Task Update(Memo memo)
        {
            try
            {
                await ApiController.Update(memo);

                Logger.Log($"{Extensions.GetLocalizedValue("LoggerUpdated")}: {memo.Id}");
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
            }
        }

        public async Task<bool> Delete(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    if (CurrentMemo == null)
                        return false;

                    await ApiController.Delete(CurrentMemo.Id);
                    GetNewMemo();
                }
                else
                {
                    await ApiController.Delete(id);
                }

                Logger.Log($"{Extensions.GetLocalizedValue("LoggerDeleted")}{(id != 0 ? $": {id}" : "")}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
                return false;
            }
        }

        public async Task Add(string question, string answer)
        {
            try
            {
                var newMemo = new Memo
                {
                    Question = question,
                    Answer = answer
                };
                await ApiController.Add(newMemo);

                Logger.Log($"{Extensions.GetLocalizedValue("LoggerAdded")}!");
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
            }
        }

        public async Task<IEnumerable<Memo>> Find(string searchWord)
        {
            try
            {
                return await ApiController.Find(searchWord);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
                return null;
            }
        }

        public async Task ImportFromFile(string pathToFile, string separator)
        {
            try
            {
                var importListForRegex = File.ReadAllText(pathToFile, Encoding.UTF8);
                var matchesList = new Regex($"(?<question>.+){separator}(?<answer>[^\\r\\n]+)").Matches(importListForRegex);

                if (matchesList.Count == 0)
                    MessageBox.Show(
                        Extensions.GetLocalizedValue("MessageNotFound"),
                        Extensions.GetLocalizedValue("MessageWarning"),
                        MessageBoxButton.OK, MessageBoxImage.Information);

                var badList = new List<string>();

                foreach (Match match in matchesList)
                {
                    if (new Regex(separator).Matches(match.ToString()).Count == 1)
                    {
                        //TODO add bulk?
                        var newMemo = new Memo
                        {
                            Question = match.Groups["question"].Value,
                            Answer = match.Groups["answer"].Value
                        };

                        await ApiController.Add(newMemo);
                    }
                    else
                    {
                        badList.Add(match.Value);
                    }
                }

                File.WriteAllLines("bad.txt", badList);
                Logger.Log($"{Extensions.GetLocalizedValue("LoggerImported")}: {matchesList.Count}, " +
                           $"{Extensions.GetLocalizedValue("LoggerBads")}: {badList.Count}");
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
            }
        }
    }
}
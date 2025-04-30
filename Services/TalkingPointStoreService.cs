using SoulWriter.Models;
using System.Collections.Concurrent;

namespace SoulWriter.Services
{
    public class TalkingPointsStoreService
    {
        private readonly ConcurrentBag<TalkingPoint> _points = new();

        public Task AddAsync(string text)
        {
            _points.Add(new TalkingPoint { Text = text });
            return Task.CompletedTask;
        }

        public Task<List<TalkingPoint>> ListAsync()
        {
            return Task.FromResult(_points.OrderBy(p => p.CreatedAt).ToList());
        }

        public Task ClearAsync()
        {
            while (!_points.IsEmpty)
            {
                _points.TryTake(out _);
            }
            return Task.CompletedTask;
        }

        public Task<bool> DeleteByIndexAsync(int index)
        {
            var list = _points.OrderBy(p => p.CreatedAt).ToList();
            if (index < 0 || index >= list.Count)
                return Task.FromResult(false);

            var itemToRemove = list[index];
            var items = _points.ToList();
            _points.Clear();
            foreach (var point in items)
            {
                if (point.Id != itemToRemove.Id)
                    _points.Add(point);
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteLastAsync()
        {
            var last = _points.OrderBy(p => p.CreatedAt).LastOrDefault();
            if (last is null)
                return Task.FromResult(false);

            var items = _points.ToList();
            _points.Clear();
            foreach (var point in items)
            {
                if (point.Id != last.Id)
                    _points.Add(point);
            }

            return Task.FromResult(true);
        }

        public Task<string> GetAsMarkdownAsync()
        {
            var md = string.Join("\n\n", _points
                .OrderBy(p => p.CreatedAt)
                .Select((p, i) => $"**{i + 1}.** {p.Text}"));

            return Task.FromResult(md);
        }

        public Task<string> GetAsPlainTextAsync()
        {
            var plain = string.Join("\n\n", _points
                .OrderBy(p => p.CreatedAt)
                .Select((p, i) => $"{i + 1}. {p.Text}"));

            return Task.FromResult(plain);
        }

        public bool HasAny => !_points.IsEmpty;
    }
}

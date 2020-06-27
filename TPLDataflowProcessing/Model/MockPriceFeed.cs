using System;
using System.Threading.Tasks;

namespace TPLDataflowProcessing.Model
{
    public class MockPriceFeed
    {
        public class PriceEventArgs : EventArgs
        {
            public PriceEventArgs(string name, decimal price)
            {
                TimeStampUtc = DateTime.UtcNow;
                Price = price;
                Name = string.Intern(name);
            }
            public DateTime TimeStampUtc { get; }
            public decimal Price { get; }
            public string Name { get; }
        }

        public delegate void PriceEvent(MockPriceFeed sender, PriceEventArgs e);
        public event PriceEvent OnNextPricePublished;
        
        
        private decimal _lastPrice;
        private Random _rand = new Random();
        private object _feedLock = new object();
        private bool _isRunning = false;

        public MockPriceFeed(string feedName)
        {
            FeedName = feedName;
            _lastPrice = _rand.Next(0, 100);
        }

        public string FeedName { get; }

        public async void StartAsync()
        {
            lock (_feedLock)
            {
                if (_isRunning) return;
                _isRunning = true;
            }

            while (_isRunning)
            {
                _lastPrice = Math.Abs(_lastPrice + (_rand.Next(-5, 5) / 10m));
                var priceObj = new PriceEventArgs(FeedName, _lastPrice);
                OnNextPricePublished(this, priceObj);
                await Task.Delay(100);
            }
        }

        public void Stop()
        {
            lock (_feedLock)
            {
                if (!_isRunning) return;
                _isRunning = false;
            }
        }
    }
}

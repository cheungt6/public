using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TPLDataflowProcessing.Model;
using Common.Wpf.Command;
using Common.Wpf.ViewModel;
using static TPLDataflowProcessing.Model.MockPriceFeed;
using System.Threading.Tasks.Dataflow;

namespace TPLDataflowProcessing.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IList<string> _stocks = new[] { "FB", "AAPL", "AMZN", "NFLX", "GOOG" };
        //private IList<string> stocks = new[] { "FB" };
        private const int _maxLogSize = 20;
        private IDictionary<string, MockPriceFeed> _priceFeeds = new Dictionary<string, MockPriceFeed>();
        private bool _feedsStarted = false;

        public MainWindowViewModel()
        {
            _updateLog = new ObservableCollection<string>();
            _latestPrices = new ObservableCollection<PriceEventViewModel>();
            _latestPriceDict = new Dictionary<string, PriceEventViewModel>();
            _averagePrices = new ObservableCollection<PriceEventViewModel>();
            _averagePriceDict = new Dictionary<string, PriceEventViewModel>();

            StartFeedsCommand = new GenericCommand(() => StartStopFeeds(true), _ => !_feedsStarted);
            StopFeedsCommand = new GenericCommand(() => StartStopFeeds(false), _ => _feedsStarted);

            Init();
        }

        private void Init()
        {
            _broadcastBlock = new BroadcastBlock<PriceEventArgs>(e => e);
            foreach (var stock in _stocks)
            {
                var feed = new MockPriceFeed(stock);
                _priceFeeds.Add(stock, feed);
                feed.OnNextPricePublished += Feed_OnNextPricePublished;
            }
            InitUILog();
            InitUIAverage();
            InitUILatestPriceTimeInterval();

        }

        private void InitUIAverage()
        {
            foreach (var feed in _priceFeeds)
            {
                var batchBlock = new BatchBlock<PriceEventArgs>(10);
                var actionBlock = new ActionBlock<PriceEventArgs[]>(e =>
                {
                    var average = e.Average(v => v.Price);
                    var low = e.Min(v => v.Price);
                    var high = e.Max(v => v.Price);
                    var update = new PriceEventViewModel(feed.Key, DateTime.UtcNow, average, low, high);
                    UpdatePrice(update, _averagePriceDict, AveragePrices);
                });

                _broadcastBlock.LinkTo(batchBlock, p => p.Name == feed.Key);
                batchBlock.LinkTo(actionBlock);
            }
        }

        private void InitUILatestPriceTimeInterval()
        {
            foreach (var feed in _priceFeeds)
            {
                var batchBlock = new BatchBlock<PriceEventArgs>(10000);
                var timeOut = TimeSpan.FromSeconds(1);
                var timeOutTimer = new System.Timers.Timer(timeOut.TotalMilliseconds);
                timeOutTimer.Elapsed += (s, e) => batchBlock.TriggerBatch();

                var updateUILatestPrice = new ActionBlock<PriceEventArgs[]>(e =>
                {
                    var latest = e.OrderByDescending(o => o.TimeStampUtc)
                        .FirstOrDefault();
                    var low = e.Min(v => v.Price);
                    var high = e.Max(v => v.Price);
                    var update = new PriceEventViewModel(feed.Key, DateTime.UtcNow, latest.Price, low, high);

                    UpdatePrice(update, _latestPriceDict, LatestPrices);
                });

                _broadcastBlock.LinkTo(batchBlock, p => p.Name == feed.Key);
                batchBlock.LinkTo(updateUILatestPrice);
                timeOutTimer.Start();
            }
        }

        private void UpdatePrice(PriceEventViewModel v, Dictionary<string, PriceEventViewModel> dictToUpdate,
            ObservableCollection<PriceEventViewModel> collectionToUpdate)
        {
            if (App.Current != null)
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (dictToUpdate.TryGetValue(v.Name, out var lastPrice))
                        lastPrice.Update(v);
                    else
                    {
                        dictToUpdate.Add(v.Name, v);
                        collectionToUpdate.Add(v);
                    }
                });
        }

        private void InitUILog()
        {
            var transformToText = new TransformBlock<PriceEventArgs, string>(e =>
            {
                return $"{e.TimeStampUtc} {e.Name}: {e.Price}";
            });

            var updateUILog = new ActionBlock<string>(e =>
            {
                if (App.Current != null)
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (UpdateLog.Count() == _maxLogSize)
                            UpdateLog.RemoveAt(_maxLogSize - 1);
                        UpdateLog.Insert(0, e);
                    });
            });

            _broadcastBlock.LinkTo(transformToText);
            transformToText.LinkTo(updateUILog);
        }

        private void StartStopFeeds(bool start)
        {
            _feedsStarted = start;
            foreach (var feed in _priceFeeds)
                if (start)
                    feed.Value.StartAsync();
                else
                    feed.Value.Stop();
        }

        public GenericCommand StartFeedsCommand { get; set; }
        public GenericCommand StopFeedsCommand { get; set; }
        
        private BroadcastBlock<PriceEventArgs> _broadcastBlock;

        private ObservableCollection<string> _updateLog;
        public ObservableCollection<string> UpdateLog 
        {
            get => _updateLog; 
            set 
            {
                _updateLog = value;
                NotifyPropertyChanged();
            }
        }

        Dictionary<string, PriceEventViewModel> _averagePriceDict;
        private ObservableCollection<PriceEventViewModel> _averagePrices;
        public ObservableCollection<PriceEventViewModel> AveragePrices
        {
            get => _averagePrices;
            set
            {
                _averagePrices = value;
                NotifyPropertyChanged();
            }
        }

        Dictionary<string, PriceEventViewModel> _latestPriceDict;
        private ObservableCollection<PriceEventViewModel> _latestPrices;
        public ObservableCollection<PriceEventViewModel> LatestPrices
        {
            get => _latestPrices;
            set
            {
                _latestPrices = value;
                NotifyPropertyChanged();
            }
        }

        private void Feed_OnNextPricePublished(MockPriceFeed sender, PriceEventArgs e)
        {
            _broadcastBlock.Post(e);
        }

        ~MainWindowViewModel()
        {
            StartStopFeeds(false);
        }
    }
}
//
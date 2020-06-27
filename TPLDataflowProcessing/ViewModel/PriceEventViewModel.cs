using Common.Wpf.ViewModel;
using System;

namespace TPLDataflowProcessing.ViewModel
{
    public class PriceEventViewModel : ViewModelBase
    {
        private DateTime _timeStampUtc;
        private decimal _price;
        private decimal _high;
        private decimal _low;

        public PriceEventViewModel(string name, DateTime timeStampUtc, decimal price, decimal low, decimal high)
        {
            Name = name;
            Price = price;
            High = high;
            Low = low;
            TimeStampUtc = timeStampUtc;
        }

        public void Update(PriceEventViewModel v)
        {
            Price = v.Price;
            High = v.High;
            Low = v.Low;
            TimeStampUtc = v.TimeStampUtc;

        }

        public string Name { get; }

        public DateTime TimeStampUtc
        {
            get
            {
                return _timeStampUtc;
            }
            set
            {
                _timeStampUtc = value;
                NotifyPropertyChanged();
            }
        }

        public decimal Low
        {
            get
            {
                return _low;
            }
            set
            {
                _low = value;
                NotifyPropertyChanged();
            }
        }

        public decimal High
        {
            get
            {
                return _high;
            }
            set
            {
                _high = value;
                NotifyPropertyChanged();
            }
        }

        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                NotifyPropertyChanged();
            }
        }

    }
}

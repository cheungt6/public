using ReflectionEmitClassGeneration.Command;
using ReflectionEmitClassGeneration.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReflectionEmitClassGeneration.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IList items;
        private Dictionary<string, Type> _properties;
        private string _newPropertyName;
        private TypeGeneratorGeneric<ItemViewModelBase> _typeGenerator;

        private static object _feedLock = new object();
        private bool _feedRunning;

        public MainWindowViewModel()
        {
            GenerateItemCommand = new GenericCommand(GenerateItems, obj =>
            {
                return !string.IsNullOrEmpty(obj?.ToString()) &&
                !_properties.ContainsKey(obj.ToString());
            });
            _properties = new Dictionary<string, Type>();
            _newPropertyName = "Id";
            GenerateItems();
            StartDataFeed();
        }

        public IList Items
        {
            get => items;
            set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand GenerateItemCommand { get; }

        public string NewPropertyName
        {
            get => _newPropertyName;
            set
            {
                _newPropertyName = value;
                NotifyPropertyChanged();
            }
        }

        public void GenerateItems()
        {
            _properties.Add(NewPropertyName, typeof(int));
            NewPropertyName = null;
            _typeGenerator = new TypeGeneratorGeneric<ItemViewModelBase>(_properties);
            var items = new List<ItemViewModelBase>();
            for (var i = 0; i < 10; i++)
            {
                var newObject = _typeGenerator.CreateInstance(new Dictionary<string, object> { { "Id", i } });
                items.Add(newObject);
            }

            Items = _typeGenerator.CreateList(items.ToArray());
        }

        private async void StartDataFeed()
        {
            _feedRunning = true;
            while (_feedRunning)
            {
                await Task.Delay(500);
                if (_properties.Count() == 1) continue;
                lock (_feedLock)
                {
                    foreach (var item in Items)
                    {
                        var data = GetRandomData();
                        var vm = item as ItemViewModelBase;
                        _typeGenerator.SetValues(vm, data);
                        foreach (var property in _properties)
                            vm.NotifyPropertyChanged(property.Key);


                    }
                }
            }
        }

        private Dictionary<string, object> GetRandomData()
        {
            var random = new Random();
            var valuesDict = new Dictionary<string, object>();
            foreach (var property in _properties)
            {
                if (property.Key == "Id") continue;
                valuesDict.Add(property.Key, random.Next());

            }
            return valuesDict;
        }

        ~MainWindowViewModel()
        {
            lock (_feedLock)
                _feedRunning = false;
        }
    }
}

using ReflectionEmitClassGeneration.Command;
using ReflectionEmitClassGeneration.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReflectionEmitClassGeneration.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IList items;
        private Type _itemType;
        private Dictionary<string, Type> _properties;
        private string _newPropertyName;

        public MainWindowViewModel()
        {
            GenerateItemCommand = new GenericCommand(GenerateItem, obj =>
            {
                return !string.IsNullOrEmpty(obj?.ToString()) &&
                !_properties.ContainsKey(obj.ToString());
            });
            _properties = new Dictionary<string, Type>();
            _newPropertyName = "Id";
            GenerateItem();
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

        public Type ItemType
        {
            get => _itemType;
            set
            {
                _itemType = value;

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

        public void GenerateItem()
        {
            _properties.Add(NewPropertyName, typeof(int));
            NewPropertyName = null;
            var typeGenerator = new TypeGeneratorGeneric<ItemViewModelBase>(_properties);
            ItemType = typeGenerator.GeneratedType;
            var items = new List<ItemViewModelBase>();

            for (var i = 0; i < 10; i++)
            {
                var newObject = typeGenerator.CreateInstance(new Dictionary<string, object> { { "Id", i } });
                items.Add(newObject);
                if (_properties.Count > 1)
                {
                    var data = GetRandomData();
                    typeGenerator.SetValues(newObject, data);
                }
            }
            Items = typeGenerator.CreateList(items.ToArray());


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
    }
}

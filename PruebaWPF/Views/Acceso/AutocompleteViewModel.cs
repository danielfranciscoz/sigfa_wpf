﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MaterialDesignThemes.Wpf;

using MaterialDesignExtensions.Model;
using System.Collections;
using System.ComponentModel;

namespace MaterialDesignExtensionsDemo.ViewModel
{
    public class AutocompleteViewModel 
    {
        private IAutocompleteSource m_autocompleteSource;
        public event PropertyChangedEventHandler PropertyChanged;


        private object m_selectedItem;

        public IAutocompleteSource AutocompleteSource
        {
            get
            {
                return m_autocompleteSource;
            }
        }

     

        public object SelectedItem
        {
            get
            {
                return m_selectedItem;
            }

            set
            {
                m_selectedItem = value;

                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && !string.IsNullOrWhiteSpace(propertyName))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public AutocompleteViewModel()
            : base()
        {
            m_autocompleteSource = new OperatingSystemAutocompleteSource();

            m_selectedItem = null;
        }
    }

    public class OperatingSystemItem
    {
        public PackIconKind Icon { get; set; }

        public string Name { get; set; }

        public OperatingSystemItem() { }
    }

    public class OperatingSystemAutocompleteSource : IAutocompleteSource
    {
        private List<OperatingSystemItem> m_operatingSystemItems;

        public OperatingSystemAutocompleteSource()
        {
            m_operatingSystemItems = new List<OperatingSystemItem>()
            {
                new OperatingSystemItem() { Icon = PackIconKind.Android, Name = "Android Gingerbread" },
                new OperatingSystemItem() { Icon = PackIconKind.Android, Name = "Android Icecream Sandwich" },
                new OperatingSystemItem() { Icon = PackIconKind.Android, Name = "Android Jellybean" },
                new OperatingSystemItem() { Icon = PackIconKind.Android, Name = "Android Lollipop" },
                new OperatingSystemItem() { Icon = PackIconKind.Android, Name = "Android Nougat" },
                new OperatingSystemItem() { Icon = PackIconKind.Linux, Name = "Debian" },
                new OperatingSystemItem() { Icon = PackIconKind.DesktopMac, Name = "Mac OSX" },
                new OperatingSystemItem() { Icon = PackIconKind.DeveloperBoard, Name = "Raspbian" },
                new OperatingSystemItem() { Icon = PackIconKind.Ubuntu, Name = "Ubuntu Wily Werewolf" },
                new OperatingSystemItem() { Icon = PackIconKind.Ubuntu, Name = "Ubuntu Xenial Xerus" },
                new OperatingSystemItem() { Icon = PackIconKind.Ubuntu, Name = "Ubuntu Yakkety Yak" },
                new OperatingSystemItem() { Icon = PackIconKind.Ubuntu, Name = "Ubuntu Zesty Zapus" },
                new OperatingSystemItem() { Icon = PackIconKind.MicrosoftWindows, Name = "Windows 7" },
                new OperatingSystemItem() { Icon = PackIconKind.MicrosoftWindows, Name = "Windows 8" },
                new OperatingSystemItem() { Icon = PackIconKind.MicrosoftWindows, Name = "Windows 8.1" },
                new OperatingSystemItem() { Icon = PackIconKind.MicrosoftWindows, Name = "Windows 10" },
                new OperatingSystemItem() { Icon = PackIconKind.MicrosoftWindows, Name = "Windows Vista" },
                new OperatingSystemItem() { Icon = PackIconKind.MicrosoftWindows, Name = "Windows XP" }
            };
        }

        public IEnumerable Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return m_operatingSystemItems.Where(item => item.Name.ToLower().Contains(searchTerm));
        }
    }
    
}

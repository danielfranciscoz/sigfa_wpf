using MaterialDesignExtensions.Model;
using MaterialDesignExtensionsDemo.ViewModel;
using MaterialDesignThemes.Wpf;
using PruebaWPF.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PruebaWPF.Views.Acceso
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            //IAutocompleteSource auto = new OperatingSystemAutocompleteSource();

            this.DataContext = new AutocompleteViewModel();
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual string DocumentationUrl
        {
            get
            {
                return "https://spiegelp.github.io/MaterialDesignExtensions/#documentation";
            }
        }

        public ViewModel() { }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && !string.IsNullOrWhiteSpace(propertyName))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class AutocompleteViewModel : ViewModel
    {
        private IAutocompleteSource m_autocompleteSource;

        private object m_selectedItem;

        public IAutocompleteSource AutocompleteSource
        {
            get
            {
                return m_autocompleteSource;
            }
        }

        public override string DocumentationUrl
        {
            get
            {
                return "https://spiegelp.github.io/MaterialDesignExtensions/#documentation/autocomplete";
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

        public AutocompleteViewModel()
            : base()
        {
            m_autocompleteSource = new OperatingSystemAutocompleteSource();

            m_selectedItem = null;
        }
    }

    public class OperatingSystemAutocompleteSource : IAutocompleteSource
    {
        public string autocomplete = "";
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

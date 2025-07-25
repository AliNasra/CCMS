using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2.ViewModels.Cement
{
    public class CementMenuViewModel : INotifyPropertyChanged
    {
        private double _frameWidth;
        public double FrameWidth
        {
            get => _frameWidth;
            set
            {
                _frameWidth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonWidth));
                OnPropertyChanged(nameof(ButtonMargin));
            }
        }

        private double _frameHeight;
        public double FrameHeight
        {
            get => _frameHeight;
            set
            {
                _frameHeight = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonHeight));
                OnPropertyChanged(nameof(ButtonMargin));
            }
        }

        public double ButtonWidth => FrameWidth * 0.35;
        public double ButtonHeight => FrameHeight * 0.15;


        public Thickness ButtonMargin => new Thickness(
            FrameWidth * 0.05,      // Left
            FrameHeight * 0.05,     // Top
            FrameWidth * 0.05,      // Right
            FrameHeight * 0.05      // Bottom
        );
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

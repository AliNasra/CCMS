using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models.Items;
using WpfApp2.Services;

namespace WpfApp2.ViewModels.Concrete
{
    public class DisplayConcreteRecordViewModel : INotifyPropertyChanged
    {
        public  PlotModel ConcreteModel { get; private set; }
        private AreaSeries _lineSeriesProducedConcrete;
        private double _frameWidth;
        public double FrameWidth
        {
            get => _frameWidth;
            set
            {
                _frameWidth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonWidth));
                OnPropertyChanged(nameof(ComboBoxWidth));
                OnPropertyChanged(nameof(ReturnButtonMargin));
                OnPropertyChanged(nameof(ItemMargin));

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
                OnPropertyChanged(nameof(ComboboxHeight));
                OnPropertyChanged(nameof(ReturnButtonMargin));
                OnPropertyChanged(nameof(ItemMargin));
            }
        }

        public double ButtonWidth => FrameWidth * 0.15;
        public double ButtonHeight => FrameHeight * 0.15;
        public double ComboBoxWidth => FrameWidth * 0.10;
        public double ComboboxHeight => FrameHeight * 0.07;

        public Thickness ItemMargin => new Thickness(
            FrameWidth * 0.005,      // Left
            FrameHeight * 0.005,     // Top
            FrameWidth * 0.005,      // Right
            FrameHeight * 0.005      // Bottom
        );

        public Thickness ReturnButtonMargin => new Thickness(
            FrameWidth * 0.1,      // Left
            FrameHeight * 0.01,     // Top
            0,       // Right
            FrameHeight * 0.01      // Bottom
        );

        private ObservableCollection<FetchConcreteRecord> _concreteRecords;
        public ObservableCollection<FetchConcreteRecord> concreteRecords
        {
            get => _concreteRecords;
            set
            {
                _concreteRecords = value;
                OnPropertyChanged();
            }
        }

        private DateTime _startDate;
        public DateTime startDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                UpdateLines();
            }
        }

        private DateTime _endDate;
        public DateTime endDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
                UpdateLines();
            }
        }

        private string _mixerName;
        public string mixerName
        {
            get => _mixerName;
            set
            {
                _mixerName = value;
                OnPropertyChanged();
                UpdateLines();
            }
        }

        

        public ObservableCollection<string> mixerNames { get; set; }

        public void initializeModel()
        {
            ConcreteModel = new PlotModel
            {
                Title = "تمام الخرسانة",
                TitleFontSize = 18,
                TextColor = OxyColors.DimGray,
                PlotAreaBorderColor = OxyColors.Transparent,
                IsLegendVisible = true,
            };
            ConcreteModel.Legends.Add(new Legend()
            {
                LegendPosition = LegendPosition.TopRight,
                LegendFontSize = 12,
                LegendSymbolLength = 24,
            });
            // X Axis
            ConcreteModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "التاريخ",
                TitleFontSize = 14,
                StringFormat = "MMM dd, yyyy",
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Minimum = DateTimeAxis.ToDouble(startDate.AddDays(-7)),
                Maximum = DateTimeAxis.ToDouble(endDate.AddDays(7)),
                MajorGridlineColor = OxyColor.FromRgb(220, 220, 220),
                Angle = 45
            });

            // Y Axis
            ConcreteModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "كمية الخرسانة بالمتر المكعب",
                TitleFontSize = 14,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(220, 220, 220)
            });

            _lineSeriesProducedConcrete = new AreaSeries
            {
                Title = "الخرسانة المنتجة",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColors.SteelBlue,
                StrokeThickness = 0,
                Color = OxyColors.SteelBlue
            };
            ConcreteModel.Series.Add(_lineSeriesProducedConcrete);
            UpdateLines();
        }

        public void UpdateLines()
        {
            if (ConcreteModel != null && startDate<=endDate)
            {
                List<DataPoint> result = ConcreteService.retrieveViewPoints(mixerName, startDate, endDate);
                _lineSeriesProducedConcrete.Points.Clear();

                foreach (DataPoint point in result)
                {
                    _lineSeriesProducedConcrete.Points.Add(point);
                }
                var xAxis = ConcreteModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
                xAxis.Minimum = DateTimeAxis.ToDouble(startDate.AddDays(-7));
                xAxis.Maximum = DateTimeAxis.ToDouble(endDate.AddDays(7));
                ConcreteModel.InvalidatePlot(true);
            }
        }

        public DisplayConcreteRecordViewModel()
        {
            ConcreteService.initializeConcreteRecords();
            mixerNames = new ObservableCollection<string>(MixerService.getOperationalMixers().Select(x => x.mixerName));
            if (mixerNames.Count > 0)
            {
                mixerName = mixerNames.First();
            }
            else
            {
                mixerName = "";
            }           
            startDate = DateTime.Today.Date;
            endDate = DateTime.Today.Date;
            initializeModel();

        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

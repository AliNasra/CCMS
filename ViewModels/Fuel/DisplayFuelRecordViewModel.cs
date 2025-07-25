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
using WpfApp2.Services;

namespace WpfApp2.ViewModels.Fuel
{
    public class DisplayFuelRecordViewModel : INotifyPropertyChanged
    {
        public  PlotModel   FuelModel { get; private set; }
        private AreaSeries _lineSeriesImported;
        private AreaSeries _lineSeriesConsumed;
        private AreaSeries _lineSeriesRemaining;
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


        public Thickness ReturnButtonMargin => new Thickness(
            FrameWidth * 0.1,      // Left
            FrameHeight * 0.01,     // Top
            0,       // Right
            FrameHeight * 0.01      // Bottom
        );

        public Thickness ItemMargin => new Thickness(
            FrameWidth * 0.005,      // Left
            FrameHeight * 0.005,     // Top
            FrameWidth * 0.005,      // Right
            FrameHeight * 0.005      // Bottom
        );

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

        private string _depotName;
        public string depotName
        {
            get => _depotName;
            set
            {
                _depotName = value;
                OnPropertyChanged();
                UpdateLines();
            }
        }

        public ObservableCollection<string> depotNames { get; set; }

        public void initializeModel()
        {
            FuelModel = new PlotModel
            {
                Title = "تمام السولار",
                TitleFontSize = 18,
                TextColor = OxyColors.DimGray,
                PlotAreaBorderColor = OxyColors.Transparent,
                IsLegendVisible = true,
            };
            FuelModel.Legends.Add(new Legend()
            {
                LegendPosition = LegendPosition.TopRight,
                LegendFontSize = 12,
                LegendSymbolLength = 24,
            });

            // X Axis
            FuelModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "التاريخ",
                TitleFontSize = 14,
                StringFormat = "MMM dd",
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Minimum = DateTimeAxis.ToDouble(startDate.AddDays(-7)),
                Maximum = DateTimeAxis.ToDouble(endDate.AddDays(7)),
                MajorGridlineColor = OxyColor.FromRgb(220, 220, 220),
                Angle = 45
            });

            // Y Axis
            FuelModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "كمية السولار باللتر",
                TitleFontSize = 14,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(220, 220, 220)
            });

            var line = new LineSeries
            {
                Color = OxyColors.SteelBlue,
                
            };
            _lineSeriesImported = new AreaSeries
            { 
                Title = "السولار المورد",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColors.SteelBlue,
                StrokeThickness = 0,
                Color = OxyColors.SteelBlue,
            };
            _lineSeriesConsumed = new AreaSeries
            {
                Title = "السولار المستهلك",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColors.DarkRed,
                StrokeThickness = 0,
                Color = OxyColors.DarkRed,
            };
            _lineSeriesRemaining = new AreaSeries
            { 
                Title = "السولار المتبقى",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColors.DarkOliveGreen,
                StrokeThickness = 0,
                Color = OxyColors.DarkOliveGreen,
            };
            FuelModel.Series.Add(_lineSeriesImported);
            FuelModel.Series.Add(_lineSeriesConsumed);
            FuelModel.Series.Add(_lineSeriesRemaining);

            UpdateLines();
        }


        public void UpdateLines()
        {
            if (FuelModel != null && startDate <= endDate)
            {
                List<List<DataPoint>> result = FuelService.retrieveViewPoints(depotName, startDate, endDate);
                _lineSeriesImported.Points.Clear();
                _lineSeriesConsumed.Points.Clear();
                _lineSeriesRemaining.Points.Clear();
                foreach (DataPoint point in result.ElementAt(0))
                {
                    _lineSeriesImported.Points.Add(point);
                }
                foreach (DataPoint point in result.ElementAt(1))
                {
                    _lineSeriesConsumed.Points.Add(point);
                }
                foreach (DataPoint point in result.ElementAt(2))
                {
                    _lineSeriesRemaining.Points.Add(point);
                }

                var xAxis = FuelModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
                xAxis.Minimum = DateTimeAxis.ToDouble(startDate.AddDays(-7));
                xAxis.Maximum = DateTimeAxis.ToDouble(endDate.AddDays(7));
                FuelModel.InvalidatePlot(true);
            }        
        }

        public DisplayFuelRecordViewModel()
        {
            FuelService.initializeFuelRecords();
            depotNames = new ObservableCollection<string>(DepotService.fetchDepots().Select(x => x.depotName));
            if (depotNames.Count > 0)
            {
                depotName = depotNames.First();
            }
            else
            {
                depotName = "";
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
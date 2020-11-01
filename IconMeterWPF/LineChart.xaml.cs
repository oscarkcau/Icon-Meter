using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IconMeterWPF
{

    /// <summary>
    /// Interaction logic for LineChart.xaml
    /// </summary>
    public partial class LineChart : UserControl, INotifyPropertyChanged
    {
        public enum DisplayStyleEnum { Normal, Accumulative, Separated };

        // private fields
        private readonly List<Brush> brushes = new List<Brush>();
        private readonly List<Brush> darkBrushes = new List<Brush>();
        private readonly List<Line> lineToRemove = new List<Line>();
        private float[] previousData;
        private float _adjustedMaxValue, _adjustedMaxValue2;

        // public dependency properties
        public static readonly DependencyProperty NextDataDependencyProperty
            = DependencyProperty.Register("NextData", typeof(IEnumerable<float>), typeof(LineChart),
                new PropertyMetadata(null, new PropertyChangedCallback(OnDependencyPropertyChanged)));
        public static readonly DependencyProperty LineColorsDependencyProperty
            = DependencyProperty.Register("LineColors", typeof(IEnumerable<Color>), typeof(LineChart),
                new PropertyMetadata(null, new PropertyChangedCallback(OnDependencyPropertyChanged)));
        public static readonly DependencyProperty MaxValueDependencyProperty
            = DependencyProperty.Register("MaxValue", typeof(float), typeof(LineChart),
                new PropertyMetadata(100f, new PropertyChangedCallback(OnDependencyPropertyChanged)));
        public static readonly DependencyProperty IsAutoAdjustDependencyProperty
            = DependencyProperty.Register("IsAutoAdjust", typeof(bool), typeof(LineChart),
                new PropertyMetadata(false, new PropertyChangedCallback(OnDependencyPropertyChanged)));
        public static readonly DependencyProperty DisplayStyleDependencyProperty
            = DependencyProperty.Register("DisplayStyle", typeof(DisplayStyleEnum), typeof(LineChart),
                new PropertyMetadata(DisplayStyleEnum.Normal, new PropertyChangedCallback(OnDependencyPropertyChanged)));
        public static readonly DependencyProperty DisplayUnitDependencyProperty
            = DependencyProperty.Register("DisplayUnit", typeof(string), typeof(LineChart),
                new PropertyMetadata("", new PropertyChangedCallback(OnDependencyPropertyChanged)));
        public IEnumerable<float> NextData
        {
            set => SetValue(NextDataDependencyProperty, value);
            get => (IEnumerable<float>)GetValue(NextDataDependencyProperty);
        }
        public IEnumerable<Color> LineColors
        {
            set => SetValue(LineColorsDependencyProperty, value);
            get => (IEnumerable<Color>)GetValue(LineColorsDependencyProperty);
        }
        public float MaxValue
        {
            set => SetValue(MaxValueDependencyProperty, value);
            get => (float)GetValue(MaxValueDependencyProperty);
        }
        public bool IsAutoAdjust
        {
            set => SetValue(IsAutoAdjustDependencyProperty, value);
            get => (bool)GetValue(IsAutoAdjustDependencyProperty);
        }
        public DisplayStyleEnum DisplayStyle
       {
            set => SetValue(DisplayStyleDependencyProperty, value);
            get => (DisplayStyleEnum) GetValue(DisplayStyleDependencyProperty);
        }
        public string DisplayUnit {
            set => SetValue(DisplayUnitDependencyProperty, value);
            get => (string)GetValue(DisplayUnitDependencyProperty);
        }

        // public properties
        private float AdjustedMaxValue
		{
            get => _adjustedMaxValue;
            set { 
                SetField(ref _adjustedMaxValue, value);
                LabelTopValue.Text = GetFormattedSize(_adjustedMaxValue); 
            }
        }
        public float AdjustedMaxValue2 {
            get => _adjustedMaxValue2;
            set {
                SetField(ref _adjustedMaxValue2, value);
                LabelBottomValue.Text = GetFormattedSize(_adjustedMaxValue2);
            }
        }

        // constructor
        public LineChart()
        {
            InitializeComponent();

            AdjustedMaxValue = AdjustedMaxValue2 = MaxValue;
            LabelTopValue.Visibility = (IsAutoAdjust) ? Visibility.Visible : Visibility.Hidden;
            LabelBottomValue.Visibility = (IsAutoAdjust) ? Visibility.Visible : Visibility.Hidden;

            brushes.Add(new SolidColorBrush(Color.FromRgb(100, 100, 255)));
            brushes.Add(new SolidColorBrush(Color.FromRgb(255, 100, 100)));
        }

        // event handlers
        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineChart mycontrol = d as LineChart;
            mycontrol.OnDependencyPropertyChanged(e);
        }
        private void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == NextDataDependencyProperty) OnNextDataChanged(e);
            else if (e.Property == LineColorsDependencyProperty) OnLineColorsChanged(e);
            else if (e.Property == MaxValueDependencyProperty)
            {
                AdjustedMaxValue = AdjustedMaxValue2 = MaxValue;
            }
            else if (e.Property == IsAutoAdjustDependencyProperty)
            {
                LabelTopValue.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Hidden;
                LabelBottomValue.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Hidden;
            }                
        }
        private void OnNextDataChanged(DependencyPropertyChangedEventArgs e)
        {
            RemoveOldLines();

            if (e.NewValue is IEnumerable<float> values)
            {
                AddNewLines(values);
            }

            if (IsAutoAdjust)
            {
                if (DisplayStyle == DisplayStyleEnum.Separated) AdjustMaxValue_Separated();
                else AdjustMaxValue();
            }
        }
        private void OnLineColorsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IEnumerable<Color> values)
            {
                this.brushes.Clear();
                this.darkBrushes.Clear();
                foreach (Color c in values)
                {
                    this.brushes.Add(new SolidColorBrush(c));

                    byte r = (byte)(c.R / 2);
                    byte g = (byte)(c.G / 2);
                    byte b = (byte)(c.B / 2);
                    this.darkBrushes.Add(new SolidColorBrush(Color.FromRgb(r, g, b)));
                }
            }
        }

        // private methods
        private void RemoveOldLines()
		{
            lineToRemove.Clear();

            // scan all existing lines
            foreach (var child in MainCanvas.Children)
            {
                if (child is Line line)
                {
                    // shift lines to left by 1 pixel
                    line.X1 -= 1;
                    line.X2 -= 1;

                    // collect lines that are out of the control
                    if (line.X1 <= 0)
                    {
                        lineToRemove.Add(line);
                    }
                }
            }

            // remove all collected lines
            foreach (var line in lineToRemove)
            {
                MainCanvas.Children.Remove(line);
            }
        }
        private void AddNewLines(IEnumerable<float> values)
        {
            switch (DisplayStyle)
			{
                case DisplayStyleEnum.Normal:
                    AddNewLine_Normal(values);
                    break;
                case DisplayStyleEnum.Accumulative:
                    AddNewLine_Accumulative(values);
                    break;
                case DisplayStyleEnum.Separated:
                    AddNewLine_Separated(values);
                    break;
			}
        }
        private void AdjustMaxValue()
		{
            AdjustedMaxValue2 = 0;

            float existingMaxValue = float.MinValue;
            foreach (var child in MainCanvas.Children)
            {
                if (child is Line line)
                {
                    var tuple = (ValueTuple<float, float>)line.Tag;
                    if (tuple.Item1 > existingMaxValue) existingMaxValue = tuple.Item1;
                    if (tuple.Item2 > existingMaxValue) existingMaxValue = tuple.Item2;
                }
            }

            float m = MaxValue;
            if (existingMaxValue > this.MaxValue)
            {
                m = (float)Math.Ceiling(existingMaxValue / MaxValue) * MaxValue;
            }

            if (m != AdjustedMaxValue)
			{
                AdjustedMaxValue = m;

                foreach (var child in MainCanvas.Children)
                {
                    if (child is Line line)
                    {
                        var tuple = (ValueTuple<float, float>)line.Tag;
                        double y1 = tuple.Item1 * MainCanvas.ActualHeight / AdjustedMaxValue;
                        double y2 = tuple.Item2 * MainCanvas.ActualHeight / AdjustedMaxValue;
                        if (y1 > MainCanvas.ActualHeight) y1 = MainCanvas.ActualHeight;
                        if (y2 > MainCanvas.ActualHeight) y2 = MainCanvas.ActualHeight;
                        line.Y1 = MainCanvas.ActualHeight - y1;
                        line.Y2 = MainCanvas.ActualHeight - y2;
                    }
                }
            }
        }

        private void AddNewLine_Normal(IEnumerable<float> values)
		{
            int index = 0;
            float[] cacheY = new float[values.Count()];

            foreach (float value in values)
            {
                if (previousData != null)
                {
                    double y1 = previousData[index] * MainCanvas.ActualHeight / AdjustedMaxValue;
                    double y2 = value * MainCanvas.ActualHeight / AdjustedMaxValue;
                    if (y1 > MainCanvas.ActualHeight) y1 = MainCanvas.ActualHeight;
                    if (y2 > MainCanvas.ActualHeight) y2 = MainCanvas.ActualHeight;
                    Line tip = new Line
                    {
                        X1 = MainCanvas.ActualWidth - 2,
                        Y1 = MainCanvas.ActualHeight - y1,
                        X2 = MainCanvas.ActualWidth - 1,
                        Y2 = MainCanvas.ActualHeight - y2,
                        StrokeThickness = 1,
                        Stroke = this.brushes[index],
                        Tag = (previousData[index], value)
                    };
                    Canvas.SetZIndex(tip, 1);
                    MainCanvas.Children.Add(tip);
                }

                cacheY[index] = value;
                index++;
            }

            previousData = cacheY;
        }
        private void AddNewLine_Accumulative(IEnumerable<float> values)
        {
            int index = 0;
            float accumatedValue = 0;
            float[] cacheY = new float[values.Count()];

            foreach (float value in values)
            {
                float adjustedValue = accumatedValue + value;
                double y1 = accumatedValue * MainCanvas.ActualHeight / AdjustedMaxValue;
                double y2 = adjustedValue * MainCanvas.ActualHeight / AdjustedMaxValue;
                if (y1 > MainCanvas.ActualHeight) y1 = MainCanvas.ActualHeight;
                if (y2 > MainCanvas.ActualHeight) y2 = MainCanvas.ActualHeight;
                Line line = new Line
                {
                    X1 = MainCanvas.ActualWidth - 1,
                    Y1 = MainCanvas.ActualHeight - y1,
                    X2 = MainCanvas.ActualWidth - 1,
                    Y2 = MainCanvas.ActualHeight - y2,
                    StrokeThickness = 1,
                    Stroke = this.darkBrushes[index],
                    Tag = (accumatedValue, adjustedValue)
                };
                Canvas.SetZIndex(line, 1);
                MainCanvas.Children.Add(line);

                if (previousData != null)
                {
                    y1 = previousData[index] * MainCanvas.ActualHeight / AdjustedMaxValue;
                    if (y1 > MainCanvas.ActualHeight) y1 = MainCanvas.ActualHeight;
                    Line tip = new Line
                    {
                        X1 = MainCanvas.ActualWidth - 2,
                        Y1 = MainCanvas.ActualHeight - y1,
                        X2 = MainCanvas.ActualWidth - 1,
                        Y2 = MainCanvas.ActualHeight - y2,
                        StrokeThickness = 1,
                        Stroke = this.brushes[index],
                        Tag = (previousData[index], adjustedValue)
                    };
                    Canvas.SetZIndex(tip, 1);
                    MainCanvas.Children.Add(tip);
                }

                accumatedValue += value;
                cacheY[index] = adjustedValue;
                index++;
            }
            previousData = cacheY;
        }
        private void AddNewLine_Separated(IEnumerable<float> values)
        {
            int index = 0;
            float[] cacheY = new float[values.Count()];
            double h = MainCanvas.ActualHeight * 0.45;

            foreach (float value in values)
            {
                double y1 = 0;
                double y2 = value * h / AdjustedMaxValue;
                if (y2 > h) y2 = h;
                Line line = new Line
                {
                    X1 = MainCanvas.ActualWidth - 1,
                    Y1 = (index % 2 == 0) ? y1 : MainCanvas.ActualHeight - y1,
                    X2 = MainCanvas.ActualWidth - 1,
                    Y2 = (index % 2 == 0) ? y2 : MainCanvas.ActualHeight - y2,
                    StrokeThickness = 1,
                    Stroke = this.darkBrushes[index],
                    Tag = (0f, value, index)
                };
                MainCanvas.Children.Add(line);
                Canvas.SetZIndex(line, 1);

                if (previousData != null)
                {
                    y1 = previousData[index] * h / AdjustedMaxValue;
                    y2 = value * h / AdjustedMaxValue;
                    if (y1 > h) y1 = h;
                    if (y2 > h) y2 = h;
                    Line tip = new Line
                    {
                        X1 = MainCanvas.ActualWidth - 2,
                        Y1 = (index % 2 == 0) ? y1 : MainCanvas.ActualHeight - y1,
                        X2 = MainCanvas.ActualWidth - 1,
                        Y2 = (index % 2 == 0) ? y2 : MainCanvas.ActualHeight - y2,
                        StrokeThickness = 1,
                        Stroke = this.brushes[index],
                        Tag = (previousData[index], value, index)
                    };
                    MainCanvas.Children.Add(tip);
                    Canvas.SetZIndex(tip, 1);
                }

                cacheY[index] = value;
                index++;
            }

            previousData = cacheY;
        }
        private void AdjustMaxValue_Separated()
        {
            float existingMaxValue1 = float.MinValue;
            float existingMaxValue2 = float.MinValue;

            foreach (var child in MainCanvas.Children)
            {
                if (child is Line line)
                {
                    var tuple = (ValueTuple<float, float, int>)line.Tag;
                    if ((tuple.Item3 % 2) == 0)
					{
                        if (tuple.Item1 > existingMaxValue1) existingMaxValue1 = tuple.Item1;
                        if (tuple.Item2 > existingMaxValue1) existingMaxValue1 = tuple.Item2;
                    }
                    else
					{
                        if (tuple.Item1 > existingMaxValue2) existingMaxValue2 = tuple.Item1;
                        if (tuple.Item2 > existingMaxValue2) existingMaxValue2 = tuple.Item2;
                    }
                }
            }

            if (existingMaxValue1 > this.MaxValue)
            {
                float m = (float)Math.Ceiling(existingMaxValue1 / MaxValue) * MaxValue;
                if (m != AdjustedMaxValue)
				{
                    AdjustedMaxValue = m;
                }
            }
            if (existingMaxValue2 > this.MaxValue)
            {
                float m = (float)Math.Ceiling(existingMaxValue2 / MaxValue) * MaxValue;
                if (m != AdjustedMaxValue2)
				{
                    AdjustedMaxValue2 = m;
                }
            }

            {
                double h = MainCanvas.ActualHeight * 0.45;

                foreach (var child in MainCanvas.Children)
                {
                    if (child is Line line)
                    {
                        var tuple = (ValueTuple<float, float, int>)line.Tag;
                        int index = tuple.Item3;
                        double m = (index % 2 == 0) ? AdjustedMaxValue : AdjustedMaxValue2;
                        double y1 = tuple.Item1 * h / m;
                        double y2 = tuple.Item2 * h / m;
                        if (y1 > h) y1 = h;
                        if (y2 > h) y2 = h;
                        line.Y1 = (index % 2 == 0) ? y1 : MainCanvas.ActualHeight - y1;
                        line.Y2 = (index % 2 == 0) ? y2 : MainCanvas.ActualHeight - y2;
                    }
                }
            }
        }
        private string GetFormattedSize(float size)
        {
            float s = size;
            string unit = DisplayUnit;
            if (s < 1024)
                return (s == Math.Floor(s) ? s.ToString() : string.Format("{0:N1}", s)) + unit;

            s /= 1024;
            if (s < 1024) return string.Format("{0:N1} K", s) + unit;
            s /= 1024;
            if (s < 1024) return string.Format("{0:N1} M", s) + unit;
            s /= 1024;
            return string.Format("{0:N1} G", s) + unit;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

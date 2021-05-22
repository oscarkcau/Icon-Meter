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

        private class LineRecord
		{
            public double x1, y1, x2, y2;
            public double originalValue1, originalValue2;
            public int colorIndex;
		}

        // private fields
        private readonly Queue<LineRecord> lineQueue = new Queue<LineRecord>();
        private readonly Queue<LineRecord> fillLineQueue = new Queue<LineRecord>();
        private List<Pen> pens = new List<Pen>();
        private List<Pen> darkPens = new List<Pen>();
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
        public float AdjustedMaxValue
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
                else { AdjustMaxValue(); }
            }

            this.InvalidateVisual();
        }
        private void OnLineColorsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IEnumerable<Color> values)
            {
                pens = values.Select(c => new Pen(new SolidColorBrush(c), 1)).ToList();
                darkPens = values.Select(c => new Pen(
                    new SolidColorBrush(Color.FromRgb((byte)(c.R>>1), (byte)(c.G>>1), (byte)(c.B>>1))), 
                    1)
                ).ToList();
            }
        }
		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);

            dc.DrawRectangle(Brushes.Black, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            foreach (var r in fillLineQueue)
            {
                dc.DrawLine(darkPens[r.colorIndex], new Point(r.x1, r.y1), new Point(r.x2, r.y2));
            }

            foreach (var r in lineQueue)
            {
                dc.DrawLine(pens[r.colorIndex], new Point(r.x1, r.y1), new Point(r.x2, r.y2));
            }
        }

        // private methods
        private void RemoveOldLines()
		{
            // shift lines to left by 1 pixel
            foreach (var r in lineQueue) { r.x1 -= 1; r.x2 -= 1; }
            foreach (var r in fillLineQueue) { r.x1 -= 1; r.x2 -= 1; }

            // remove lines that are out of the control
            while (lineQueue.Count > 0 && lineQueue.Peek().x1 <= 0) lineQueue.Dequeue();
            while (fillLineQueue.Count > 0 && fillLineQueue.Peek().x1 <= 0) fillLineQueue.Dequeue();
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
        private void AddNewLine_Normal(IEnumerable<float> values)
		{
            if (pens.Count < values.Count()) return;

            int index = 0;
            float[] cacheY = new float[values.Count()];

            foreach (float value in values)
            {
                if (previousData != null)
                {
                    double y1 = previousData[index] * ActualHeight / AdjustedMaxValue;
                    double y2 = value * ActualHeight / AdjustedMaxValue;
                    if (y1 > ActualHeight) y1 = ActualHeight;
                    if (y2 > ActualHeight) y2 = ActualHeight;

                    LineRecord r = new LineRecord()
                    {
                        x1 = ActualWidth - 2,
                        y1 = ActualHeight - y1,
                        x2 = ActualWidth - 1,
                        y2 = ActualHeight - y2,
                        colorIndex = index,
                        originalValue1 = previousData[index],
                        originalValue2 = value
                    };
                    this.lineQueue.Enqueue(r);
                }

                cacheY[index] = value;
                index++;
            }

            previousData = cacheY;
        }
        private void AddNewLine_Accumulative(IEnumerable<float> values)
        {
            if (pens.Count < values.Count()) return;
            if (darkPens.Count < values.Count()) return;

            int index = 0;
            float accumatedValue = 0;
            float[] cacheY = new float[values.Count()];

            foreach (float value in values)
            {
                float adjustedValue = accumatedValue + value;
                double y1 = accumatedValue * ActualHeight / AdjustedMaxValue;
                double y2 = adjustedValue * ActualHeight / AdjustedMaxValue;
                if (y1 > ActualHeight) y1 = ActualHeight;
                if (y2 > ActualHeight) y2 = ActualHeight;

                LineRecord r = new LineRecord()
                {
                    x1 = ActualWidth - 1,
                    y1 = ActualHeight - y1,
                    x2 = ActualWidth - 1,
                    y2 = ActualHeight - y2,
                    colorIndex = index,
                    originalValue1 = accumatedValue,
                    originalValue2 = adjustedValue
                };
                this.fillLineQueue.Enqueue(r);

                if (previousData != null)
                {
                    y1 = previousData[index] * ActualHeight / AdjustedMaxValue;
                    if (y1 > ActualHeight) y1 = ActualHeight;

                    LineRecord r2 = new LineRecord()
                    {
                        x1 = ActualWidth - 2,
                        y1 = ActualHeight - y1,
                        x2 = ActualWidth - 1,
                        y2 = ActualHeight - y2,
                        colorIndex = index,
                        originalValue1 = previousData[index],
                        originalValue2 = adjustedValue
                    };
                    this.lineQueue.Enqueue(r2);
                }

                accumatedValue += value;
                cacheY[index] = adjustedValue;
                index++;
            }
            previousData = cacheY;
        }
        private void AddNewLine_Separated(IEnumerable<float> values)
        {
            if (pens.Count < values.Count()) return;
            if (darkPens.Count < values.Count()) return;

            int index = 0;
            float[] cacheY = new float[values.Count()];
            double h = ActualHeight * 0.45;

            foreach (float value in values)
            {
                double y1 = 0;
                double y2 = value * h / AdjustedMaxValue;
                if (y2 > h) y2 = h;

                LineRecord r = new LineRecord()
                {
                    x1 = ActualWidth - 1,
                    y1 = (index % 2 == 0) ? y1 : ActualHeight - y1,
                    x2 = ActualWidth - 1,
                    y2 = (index % 2 == 0) ? y2 : ActualHeight - y2,
                    colorIndex = index,
                    originalValue1 = 0f,
                    originalValue2 = value
                };
                this.fillLineQueue.Enqueue(r);

                if (previousData != null)
                {
                    LineRecord r2 = new LineRecord()
                    {
                        x1 = ActualWidth - 2,
                        y1 = (index % 2 == 0) ? y1 : ActualHeight - y1,
                        x2 = ActualWidth - 1,
                        y2 = (index % 2 == 0) ? y2 : ActualHeight - y2,
                        colorIndex = index,
                        originalValue1 = previousData[index],
                        originalValue2 = value
                    };
                    this.lineQueue.Enqueue(r2);
                }

                cacheY[index] = value;
                index++;
            }

            previousData = cacheY;
        }
        private void AdjustMaxValue()
        {
            AdjustedMaxValue2 = 0;
            double currentMaxValue = double.MinValue;
            foreach (var r in lineQueue)
            {
                if (r.originalValue1 > currentMaxValue) currentMaxValue = r.originalValue1;
                if (r.originalValue2 > currentMaxValue) currentMaxValue = r.originalValue2;
            }

            float m = MaxValue;
            if (currentMaxValue > this.MaxValue)
            {
                m = (float)Math.Ceiling(currentMaxValue / MaxValue) * MaxValue;
            }

            if (m != AdjustedMaxValue)
            {
                AdjustedMaxValue = m;

                foreach (var r in lineQueue.Concat(fillLineQueue))
                {
                    double y1 = r.originalValue1 * ActualHeight / AdjustedMaxValue;
                    double y2 = r.originalValue2 * ActualHeight / AdjustedMaxValue;
                    if (y1 > ActualHeight) y1 = ActualHeight;
                    if (y2 > ActualHeight) y2 = ActualHeight;
                    r.y1 = ActualHeight - y1;
                    r.y2 = ActualHeight - y2;
                }
            }
        }
        private void AdjustMaxValue_Separated()
        {
            double existingMaxValue1 = double.MinValue;
            double existingMaxValue2 = double.MinValue;

            foreach (var r in lineQueue)
            {
                if (r.colorIndex % 2 == 0)
				{
                    if (r.originalValue1 > existingMaxValue1) existingMaxValue1 = r.originalValue1;
                    if (r.originalValue2 > existingMaxValue1) existingMaxValue1 = r.originalValue2;
                }
                else
				{
                    if (r.originalValue1 > existingMaxValue2) existingMaxValue2 = r.originalValue1;
                    if (r.originalValue2 > existingMaxValue2) existingMaxValue2 = r.originalValue2;
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

            double h = ActualHeight * 0.45;

            foreach (var r in lineQueue.Concat(fillLineQueue))
            {
                double m = (r.colorIndex % 2 == 0) ? AdjustedMaxValue : AdjustedMaxValue2;
                double y1 = r.originalValue1 * h / m;
                double y2 = r.originalValue2 * h / m;
                if (y1 > h) y1 = h;
                if (y2 > h) y2 = h;
                r.y1 = (r.colorIndex % 2 == 0) ? y1 : ActualHeight - y1;
                r.y2 = (r.colorIndex % 2 == 0) ? y2 : ActualHeight - y2;
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

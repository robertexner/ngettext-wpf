﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NGettext.Wpf.Example
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private int _memoryLeakTestProgress;
        private DateTime _currentTime;
        private readonly string _someDeferredLocalization = Translation.Noop("Deferred localization");
        private int _counter;

        public MainWindow()
        {
            InitializeComponent();

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.1) };
            timer.Tick += (sender, args) => { CurrentTime = DateTime.Now; };
            timer.Tick += (sender, args) => { Counter = (Counter + 1) % 1000; };
            timer.Start();
        }

        public decimal SomeNumber => 1234567.89m;

        public DateTime CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
            }
        }

        private async void OpenMemoryLeakTestWindow(object sender, RoutedEventArgs e)
        {
            var leakTestWindowReference = GetWeakReferenceToLeakTestWindow();
            for (var i = 0; i < 20; ++i)
            {
                if (!leakTestWindowReference.TryGetTarget(out _)) return;
                await Task.Delay(TimeSpan.FromSeconds(1));
                GC.Collect();
            }
            Debug.Assert(!leakTestWindowReference.TryGetTarget(out _), "memory leak detected");
        }

        private WeakReference<MemoryLeakTestWindow> GetWeakReferenceToLeakTestWindow()
        {
            var window = new MemoryLeakTestWindow();
            window.Closed += async (o, args) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                ++MemoryLeakTestProgress;
                foreach (var locale in new[]
                    {"da-DK", "de-DE", "en-US", TrackCurrentCultureBehavior.CultureTracker?.CurrentCulture?.Name})
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (TrackCurrentCultureBehavior.CultureTracker != null)
                    {
                        TrackCurrentCultureBehavior.CultureTracker.CurrentCulture = CultureInfo.GetCultureInfo(locale);
                    }

                    ++MemoryLeakTestProgress;
                }
            };
            window.Show();
            MemoryLeakTestProgress = 0;

            window.Close();

            return new WeakReference<MemoryLeakTestWindow>(window);
        }

        public int MemoryLeakTestProgress
        {
            get => _memoryLeakTestProgress;
            set
            {
                _memoryLeakTestProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MemoryLeakTestProgress)));
            }
        }

        public ICollection<ExampleEnum> EnumValues { get; } = Enum.GetValues(typeof(ExampleEnum)).Cast<ExampleEnum>().ToList();

        public event PropertyChangedEventHandler PropertyChanged;

        public string SomeDeferredLocalization => Translation.GetText(_someDeferredLocalization);

        public string Header => Translation.GetText("NGettext.WPF Example");

        public string PluralGettext => Translation.PluralGettext(1, "Singular", "Plural") +
                                       "---" + Translation.PluralGettext(2, "Singular", "Plural");

        public string PluralGettextParams => Translation.PluralGettext(1, "Singular {0:n3}", "Plural {0:n3}", 1m / 3m) +
                                             "---" + Translation.PluralGettext(2, "Singular {0:n3}", "Plural {0:n3}", 1m / 3m);

        public int Counter
        {
            get => _counter;
            set
            {
                _counter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Counter)));
            }
        }
    }
}
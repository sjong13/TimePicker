using Quorum.FieldVisor.Mobile.Extensions;
using Quorum.FieldVisor.Mobile.Models;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Quorum.FieldVisor.Mobile.Pages.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimePickerPopup : PopupPage
    {
        public FloatLabeledEntry FLE;
        public int CurrentFormat;
        public int CurrentYear;
        public int CurrentMonth;
        public int CurrentDay;
        public int CurrentHour;
        public int CurrentMinute;
        public bool Animate;
        public double ScrollPositionHours;
        public double ScrollPositionMinutes;
        public double ScrollPositionFormat;
        public Timer HourScrollTimer;
        public Timer MinuteScrollTimer;
        public Timer FormatScrollTimer;

        public TimePickerPopup(FloatLabeledEntry fle)
        {
            FLE = fle;
            FLE.UnFocusEntry();

            HourScrollTimer = new Timer
            {
                AutoReset = true,
                Interval = Device.RuntimePlatform == Device.UWP ? 300 : 150
            };

            HourScrollTimer.Elapsed += CheckHourScrollPosition;

            MinuteScrollTimer = new Timer
            {
                AutoReset = true,
                Interval = Device.RuntimePlatform == Device.UWP ? 300 : 150
            };

            MinuteScrollTimer.Elapsed += CheckMinuteScrollPosition;

            FormatScrollTimer = new Timer
            {
                AutoReset = true,
                Interval = Device.RuntimePlatform == Device.UWP ? 300 : 150
            };

            FormatScrollTimer.Elapsed += CheckFormatScrollPosition;

            Animate = Device.RuntimePlatform == Device.UWP ? true : false;

            

            var currentTime = FLE.Value as DateTime?;
            int currentHour;
            int currentHourAMPM;
            int currentMinute;

            if (currentTime != null)
            {
                CurrentYear = currentTime.Value.Year;
                CurrentMonth = currentTime.Value.Month;
                CurrentDay = currentTime.Value.Day;
                currentHour = currentTime.Value.Hour;
                currentMinute = currentTime.Value.Minute;
            }
            else
            {
                CurrentYear = DateTime.Now.Year;
                CurrentMonth = DateTime.Now.Month;
                CurrentDay = DateTime.Now.Day;
                currentHour = DateTime.Now.Hour;
                currentMinute = DateTime.Now.Minute;
            }

            switch (currentHour)
            {
                case 0:
                    currentHourAMPM = 12;
                    CurrentFormat = 0;
                    break;
                case 12:
                    currentHourAMPM = 12;
                    CurrentFormat = 1;
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    currentHourAMPM = currentHour;
                    CurrentFormat = 0;
                    break;
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                    currentHourAMPM = currentHour - 12;
                    CurrentFormat = 1;
                    break;
                default:
                    currentHourAMPM = 12;
                    CurrentFormat = 0;
                    break;
            }


            InitializeComponent();
            PickerSetup();

            var formatTapGestureRecognizer = new TapGestureRecognizer();
            formatTapGestureRecognizer.Tapped += FormatTapGestureRecognizer_Tapped;
            foreach (var item in mainStackFormat.Children)
            {
                item.GestureRecognizers.Add(formatTapGestureRecognizer);
            }

            CurrentHour = currentHourAMPM;
            CurrentMinute = currentMinute;

            var initialSelectionHour = mainStackHours.Children.Where(x => Convert.ToInt32((x as Label).Text).Equals(CurrentHour)).FirstOrDefault();
            var initialSelectionMinute = mainStackMinutes.Children.Where(x => Convert.ToInt32((x as Label).Text).Equals(CurrentMinute)).FirstOrDefault();
            var initialSelectionFormat = mainStackFormat.Children.Where(x => (x as Label).Text.Equals(CurrentFormat.Equals(0) ? "AM" : "PM")).FirstOrDefault();
            if (initialSelectionHour != null && initialSelectionMinute != null && initialSelectionFormat != null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(300);
                    ScrollHours.ScrollToAsync(initialSelectionHour, ScrollToPosition.Center, false);
                    ScrollMinutes.ScrollToAsync(initialSelectionMinute, ScrollToPosition.Center, false);
                    await ScrollFormat.ScrollToAsync(initialSelectionFormat, ScrollToPosition.Center, false);

                });
            }

            ScrollPositionHours = ScrollHours.ScrollY;
            ScrollPositionMinutes = ScrollMinutes.ScrollY;
            ScrollPositionFormat = ScrollFormat.ScrollY;

            HourScrollTimer.Start();
            MinuteScrollTimer.Start();
            FormatScrollTimer.Start();
            HourScrollTimer.Enabled = false;
            MinuteScrollTimer.Enabled = false;
            FormatScrollTimer.Enabled = false;

        }

        private void PickerSetup()
        {
            //populate hour picker
            for (int pad = 0; pad < 6; pad++)
            {
                mainStackHours.Children.Add(new Label()
                {
                    Text = "13",
                    TextColor = Color.Transparent,
                    HeightRequest = 20
                });
            }
            for (int h = 1; h <= 12; h++)
            {
                mainStackHours.Children.Add(new Label()
                {
                    Text = h.ToString(),
                    FontSize = 24,
                    HorizontalTextAlignment = TextAlignment.Center,
                    AutomationId = "Hour" + h.ToString()
                });
            }
            for (int pad = 0; pad < 6; pad++)
            {
                mainStackHours.Children.Add(new Label()
                {
                    Text = "13",
                    TextColor = Color.Transparent,
                    HeightRequest = 20
                });
            }
            //attach tapgesturerecognizer to each hour item
            var hourTapGestureRecognizer = new TapGestureRecognizer();
            hourTapGestureRecognizer.Tapped += HourTapGestureRecognizer_Tapped;
            foreach (var item in mainStackHours.Children)
            {
                item.GestureRecognizers.Add(hourTapGestureRecognizer);
            }

            //populate minute picker
            for (int pad = 0; pad < 6; pad++)
            {
                mainStackMinutes.Children.Add(new Label()
                {
                    Text = "99",
                    TextColor = Color.Transparent,
                    HeightRequest = 20
                });
            }
            for (int m = 0; m <= 9; m++)
            {
                mainStackMinutes.Children.Add(new Label()
                {
                    Text = "0" + m.ToString(),
                    FontSize = 24,
                    HorizontalTextAlignment = TextAlignment.Center,
                    AutomationId = "Minute" + "0" + m.ToString()
                });
            }

            for (int m = 10; m <= 59; m++)
            {
                mainStackMinutes.Children.Add(new Label()
                {
                    Text = m.ToString(),
                    FontSize = 24,
                    HorizontalTextAlignment = TextAlignment.Center,
                    AutomationId = "Minute" + m.ToString()
                });
            }
            for (int pad = 0; pad < 6; pad++)
            {
                mainStackMinutes.Children.Add(new Label()
                {
                    Text = "99",
                    TextColor = Color.Transparent,
                    HeightRequest = 20
                });
            }

            //attach tapgesturerecognizer to each item in minute picker
            var minuteTapGestureRecognizer = new TapGestureRecognizer();
            minuteTapGestureRecognizer.Tapped += MinuteTapGestureRecognizer_Tapped;
            foreach (var item in mainStackMinutes.Children)
            {
                item.GestureRecognizers.Add(minuteTapGestureRecognizer);
            }

            //populate format picker

            for (int pad = 0; pad < 6; pad++)
            {
                mainStackFormat.Children.Add(new Label()
                {
                    Text = "99",
                    TextColor = Color.Transparent,
                    HeightRequest = 20
                });
            }
            mainStackFormat.Children.Add(new Label()
            {
                Text = "AM",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 24,
                AutomationId = "AMFormat"
            });

            mainStackFormat.Children.Add(new Label()
            {
                Text = "PM",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 24,
                AutomationId = "PMFormat"
            });
            for (int pad = 0; pad < 6; pad++)
            {
                mainStackFormat.Children.Add(new Label()
                {
                    Text = "99",
                    TextColor = Color.Transparent,
                    HeightRequest = 20
                });
            }
        }

        private void HourTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            HourScrollTimer.Enabled = false;

            var senderLabel = sender as Label;
            if (string.IsNullOrEmpty(senderLabel.Text) || senderLabel.Text == "13")
            {
                return;
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(10);
                await ScrollHours.ScrollToAsync(senderLabel, ScrollToPosition.Center, Animate);
            });

            CurrentHour = Convert.ToInt32(senderLabel.Text);
            int offset = CurrentFormat.Equals(1) && CurrentHour != 12 ? 12 : 0;
            if (CurrentFormat.Equals(0) && CurrentHour == 12)
            {
                offset = -12;
            }
            DateTime newDateTime = new DateTime(CurrentYear, CurrentMonth, CurrentDay, CurrentHour + offset, CurrentMinute, 0);
            FLE.Value = newDateTime;
            FLE.Text = newDateTime.ToString("hh:mm tt");

            HourScrollTimer.Enabled = true;

        }

        private void MinuteTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

            MinuteScrollTimer.Enabled = false;

            var senderLabel = sender as Label;
            if (string.IsNullOrEmpty(senderLabel.Text) || senderLabel.Text == "99")
                return;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(10);
                await ScrollMinutes.ScrollToAsync(senderLabel, ScrollToPosition.Center, false);
            });

            CurrentMinute = Convert.ToInt32(senderLabel.Text);
            int offset = CurrentFormat.Equals(1) && CurrentHour != 12 ? 12 : 0;
            if (CurrentFormat.Equals(0) && CurrentHour == 12)
            {
                offset = -12;
            }
            DateTime newDateTime = new DateTime(CurrentYear, CurrentMonth, CurrentDay, CurrentHour + offset, CurrentMinute, 0);
            FLE.Value = newDateTime;
            FLE.Text = newDateTime.ToString("hh:mm tt");

            MinuteScrollTimer.Enabled = true;
        }

        private void FormatTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            FormatScrollTimer.Enabled = false;

            var senderLabel = sender as Label;

            if (string.IsNullOrEmpty(senderLabel.Text) || senderLabel.Text == "99")
                return;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(10);
                await ScrollFormat.ScrollToAsync(senderLabel, ScrollToPosition.Center, false);
            });

            CurrentFormat = senderLabel.Text.Equals("AM") ? 0 : 1;
            int offset = CurrentFormat.Equals(1) && CurrentHour != 12 ? 12 : 0;
            if (CurrentFormat.Equals(0) && CurrentHour == 12)
            {
                offset = -12;
            }
            DateTime newDateTime = new DateTime(CurrentYear, CurrentMonth, CurrentDay, CurrentHour + offset, CurrentMinute, 0);
            FLE.Value = newDateTime;
            FLE.Text = newDateTime.ToString("hh:mm tt");

            FormatScrollTimer.Enabled = true;
        }




        private void PopupPage_Disappearing(object sender, EventArgs e)
        {
            FLE.UnFocusEntry();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            FLE.UnFocusEntry();
            PopupNavigation.Instance.PopAsync();
        }

        private void ScrollHours_Scrolled(object sender, ScrolledEventArgs e)
        {
            HourScrollTimer.Enabled = true;

            if (e.ScrollY < (mainStackHours.Children.Where(x => (x as Label).Text.Equals("13")).ToList())[1].Y)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(10);
                    await ScrollHours.ScrollToAsync(mainStackHours.Children.Where(x => (x as Label).Text != "13").FirstOrDefault(), ScrollToPosition.Center, Animate);
                    
                });
            }
           else if (e.ScrollY > (mainStackHours.Children.Where(x => (x as Label).Text.Equals("9")).FirstOrDefault().Y))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(10);
                    await ScrollHours.ScrollToAsync(mainStackHours.Children.Where(x => (x as Label).Text != "13").LastOrDefault(), ScrollToPosition.Center, Animate);
                });
            }
            
        }

        private void ScrollMinutes_Scrolled(object sender, ScrolledEventArgs e)
        {
            MinuteScrollTimer.Enabled = true;
            
            if (e.ScrollY < (mainStackMinutes.Children.Where(x => (x as Label).Text.Equals("99")).ToList())[1].Y)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(10);
                    await ScrollMinutes.ScrollToAsync(mainStackMinutes.Children.Where(x => (x as Label).Text != "99").FirstOrDefault(), ScrollToPosition.Center, Animate);
                });
            }
            else if (e.ScrollY > mainStackMinutes.Children.Where(x => (x as Label).Text.Equals("56")).FirstOrDefault().Y)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(10);
                    await ScrollMinutes.ScrollToAsync(mainStackMinutes.Children.Where(x => (x as Label).Text != "99").LastOrDefault(), ScrollToPosition.Center, Animate);
                });
            }
            

        }

        private void ScrollFormat_Scrolled(object sender, ScrolledEventArgs e)
        {
            FormatScrollTimer.Enabled = true;
            if (e.ScrollY < (mainStackFormat.Children.Where(x => (x as Label).Text.Equals("99")).ToList())[1].Y)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(10);
                    await ScrollFormat.ScrollToAsync(mainStackFormat.Children.Where(x => (x as Label).Text != "99").FirstOrDefault(), ScrollToPosition.Center, Animate);
                });
            }
            else if (e.ScrollY > (mainStackFormat.Children.Where(x => (x as Label).Text.Equals("99")).ToList())[3].Y)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(10);
                    await ScrollFormat.ScrollToAsync(mainStackFormat.Children.Where(x => (x as Label).Text != "99").LastOrDefault(), ScrollToPosition.Center, Animate);
                });
            }


        }

        

        private void CheckHourScrollPosition(object sender, ElapsedEventArgs e)
        {

            if (ScrollHours.ScrollY.Equals(ScrollPositionHours))
            {
                HourScrollTimer.Enabled = false;
                var centerLabel = (mainStackHours.Children.Where(x => (x as Label).Bounds.Top < (ScrollHours.ScrollY + 112.5) &&
                       (x as Label).Bounds.Bottom > (ScrollHours.ScrollY + 112.5))).FirstOrDefault() as Label;
                if (centerLabel != null && centerLabel.Text != "13")
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(10);
                        await ScrollHours.ScrollToAsync(centerLabel, ScrollToPosition.Center, Animate);
                    });

                    CurrentHour = Convert.ToInt32(centerLabel.Text);
                    int offset = CurrentFormat.Equals(1) && CurrentHour != 12 ? 12 : 0;
                    if (CurrentFormat.Equals(0) && CurrentHour == 12)
                    {
                        offset = -12;
                    }
                    DateTime newDateTime = new DateTime(CurrentYear, CurrentMonth, CurrentDay, CurrentHour + offset, CurrentMinute, 0);

                    FLE.Value = newDateTime;
                    FLE.Text = newDateTime.ToString("hh:mm tt");

                }
            }
            else
            {
                HourScrollTimer.Enabled = true;
                ScrollPositionHours = ScrollHours.ScrollY;
            }

        }

        private void CheckMinuteScrollPosition(object sender, ElapsedEventArgs e)
        {
            if (ScrollMinutes.ScrollY.Equals(ScrollPositionMinutes))
            {
                MinuteScrollTimer.Enabled = false;
                var centerLabel = (mainStackMinutes.Children.Where(x => (x as Label).Bounds.Top < (ScrollMinutes.ScrollY + 112.5) &&
                       (x as Label).Bounds.Bottom > (ScrollMinutes.ScrollY + 112.5))).FirstOrDefault() as Label;
                if (centerLabel != null && centerLabel.Text != "99")
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(10);
                        await ScrollMinutes.ScrollToAsync(centerLabel, ScrollToPosition.Center, Animate);
                    });

                    CurrentMinute = Convert.ToInt32(centerLabel.Text);
                    int offset = CurrentFormat.Equals(1) && CurrentHour != 12 ? 12 : 0;
                    if (CurrentFormat.Equals(0) && CurrentHour == 12)
                    {
                        offset = -12;
                    }
                    DateTime newDateTime = new DateTime(CurrentYear, CurrentMonth, CurrentDay, CurrentHour + offset, CurrentMinute, 0);

                    FLE.Value = newDateTime;
                    FLE.Text = newDateTime.ToString("hh:mm tt");

                }
            }
            else
            {
                MinuteScrollTimer.Enabled = true;
                ScrollPositionMinutes = ScrollMinutes.ScrollY;
            }
        }

        private void CheckFormatScrollPosition(object sender, ElapsedEventArgs e)
        {
            if (ScrollFormat.ScrollY.Equals(ScrollPositionFormat))
            {
                FormatScrollTimer.Enabled = false;
                var centerLabel = (mainStackFormat.Children.Where(x => (x as Label).Bounds.Top < (ScrollFormat.ScrollY + 112.5) &&
                       (x as Label).Bounds.Bottom > (ScrollFormat.ScrollY + 112.5))).FirstOrDefault() as Label;
                if (centerLabel != null && centerLabel.Text != "99")
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(10);
                        await ScrollFormat.ScrollToAsync(centerLabel, ScrollToPosition.Center, Animate);
                    });

                    CurrentFormat = centerLabel.Text.Equals("AM") ? 0 : 1;
                    int offset = CurrentFormat.Equals(1) && CurrentHour != 12 ? 12 : 0;
                    if (CurrentFormat.Equals(0) && CurrentHour == 12)
                    {
                        offset = -12;
                    }
                    DateTime newDateTime = new DateTime(CurrentYear, CurrentMonth, CurrentDay, CurrentHour + offset, CurrentMinute, 0);

                    FLE.Value = newDateTime;
                    FLE.Text = newDateTime.ToString("hh:mm tt");
                }
            }
            else
            {
                FormatScrollTimer.Enabled = true;
                ScrollPositionFormat = ScrollFormat.ScrollY;
            }
        }



    }

   

    
}
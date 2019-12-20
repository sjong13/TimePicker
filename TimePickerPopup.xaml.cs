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
        private bool hourPressed = false;
        private bool minutePressed = false;
        private bool formatPressed = false;
        private TaskCompletionSource<DateTime> taskCompletion;

        private int hour;
        private int minute;
        private string format;
        private DateTime dateTime;

        public DateTime CurrentDateTime
        {
            get { return dateTime; }
            set { dateTime = value; OnPropertyChanged(); }
        }


        public string Format
        {
            get { return format; }
            set { format = value; OnPropertyChanged(); }
        }


        public int Minute
        {
            get { return minute; }
            set { minute = value; OnPropertyChanged(); }
        }


        public int Hour
        {
            get { return hour; }
            set { hour = value; OnPropertyChanged(); }
        }


        public TimePickerPopup(DateTime dt)
        {
            CurrentDateTime = dt;
            Hour = CurrentDateTime.Hour > 12 ? CurrentDateTime.Hour - 12 : CurrentDateTime.Hour.Equals(0) ? 12 : CurrentDateTime.Hour;
            Minute = CurrentDateTime.Minute;
            Format = CurrentDateTime.Hour < 12 ? "AM" : "PM";

            InitializeComponent();
        }

        public static async Task<DateTime> GetSelectedDateTime(object obj = null)
        {
            DateTime dt;
            if (obj == null)
            {
                dt = DateTime.Now;
            }
            else
            {
                var success = DateTime.TryParse(obj.ToString(), out dt);
                dt = success ? dt : DateTime.Now;
            }
            TimePickerPopup pop = new TimePickerPopup(dt);

            var result = await pop.GetSelectedDateTimeInternal(dt);

            return result;
        }

        private async Task<DateTime> GetSelectedDateTimeInternal(DateTime dt)
        {
            taskCompletion = new TaskCompletionSource<DateTime>();

            await PopupNavigation.Instance.PushAsync(this);

            return await taskCompletion.Task;
        }

   
        private void PopupPage_Disappearing(object sender, EventArgs e)
        {
            if (taskCompletion != null)
            {
                taskCompletion.TrySetResult(CurrentDateTime);

                taskCompletion = null;
            }
        }

        private async void BackgroundTapped(object sender, EventArgs e)
        {
            if(taskCompletion != null)
            {
                taskCompletion.TrySetResult(CurrentDateTime);

                taskCompletion = null;
            }

            await PopupNavigation.Instance.PopAsync();
        }

        private void AddHourButtonPressed(object sender, EventArgs e)
        {
            if (!hourPressed)
            {
                hourPressed = true;
                Hour = Hour > 11 ? 1 : Hour + 1;
                Timer hourTimer = new Timer(200);
                hourTimer.Start();
                int count = 0;
                hourTimer.Elapsed += (s, args) =>
                {
                    if (hourPressed)
                    {
                        if (count > 1)
                        {
                            hourTimer.Interval = 50;
                        }
                        Hour = Hour > 11 ? 1 : Hour + 1;
                        count++;
                    }
                    else
                    {
                        hourTimer.Stop();
                        UpdateDateTime();
                    }
                };
            }        
        }

        private void SubtractHourButtonPressed(object sender, EventArgs e)
        {
            if (!hourPressed)
            {
                hourPressed = true;
                Hour = Hour < 2 ? 12 : Hour - 1;
                Timer hourTimer = new Timer(200);
                hourTimer.Start();
                int count = 0;
                hourTimer.Elapsed += (s, args) =>
                {
                    if (hourPressed)
                    {
                        if (count > 1)
                        {
                            hourTimer.Interval = 50;
                        }
                        Hour = Hour < 2 ? 12 : Hour - 1;
                        count++;
                    }
                    else
                    {
                        hourTimer.Stop();
                        UpdateDateTime();
                    }
                };
            }
        }

        private void UpdateDateTime()
        {
            if (Format != null)
            {
                int offset = Format.Equals("PM") && Hour < 12 ? 12 : Format.Equals("AM") && Hour.Equals(12) ? -12 : 0;
                CurrentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Hour + offset, Minute, 0);
            }
        }

        private async void DoneButtonPressed(object sender, EventArgs e)
        {

            if (taskCompletion != null)
            {
                taskCompletion.TrySetResult(CurrentDateTime);

                taskCompletion = null;
            }

            await PopupNavigation.Instance.PopAsync();
        }

        private void HourButtonReleased(object sender, EventArgs e)
        {
            hourPressed = false;
        }

        private void AddMinuteButtonPressed(object sender, EventArgs e)
        {
            if (!minutePressed)
            {
                minutePressed = true;
                Minute = Minute > 58 ? 1 : Minute + 1;
                Timer minuteTimer = new Timer(200);
                minuteTimer.Start();
                int count = 0;
                minuteTimer.Elapsed += (s, args) =>
                {
                    if (minutePressed)
                    {
                        if (count > 1)
                        {
                            minuteTimer.Interval = 50;
                        }
                        Minute = Minute > 58 ? 0 : Minute + 1;
                        count++;
                    }
                    else
                    {
                        minuteTimer.Stop();
                        UpdateDateTime();
                    }
                };
            }
        }

        private void MinuteButtonReleased(object sender, EventArgs e)
        {
            minutePressed = false;
        }

        private void SubtractMinuteButtonPressed(object sender, EventArgs e)
        {
            if (!minutePressed)
            {
                minutePressed = true;
                Minute = Minute < 1 ? 59 : Minute - 1;
                Timer minuteTimer = new Timer(200);
                minuteTimer.Start();
                int count = 0;
                minuteTimer.Elapsed += (s, args) =>
                {
                    if (minutePressed)
                    {
                        if (count > 1)
                        {
                            minuteTimer.Interval = 50;
                        }
                        Minute = Minute < 1 ? 59 : Minute - 1;
                        count++;
                    }
                    else
                    {
                        minuteTimer.Stop();
                        UpdateDateTime();
                    }
                };
            }
        }

        private void AddFormatButtonPressed(object sender, EventArgs e)
        {
            if(!formatPressed)
            {
                formatPressed = true;
                Format = Format.Equals("AM") ? "PM" : "AM";
                UpdateDateTime();
            }
        }

        private void SubtractFormatButtonPressed(object sender, EventArgs e)
        {
            if (!formatPressed)
            {
                formatPressed = true;
                Format = Format.Equals("AM") ? "PM" : "AM";
                UpdateDateTime();
            }
        }

        private void FormatButtonReleased(object sender, EventArgs e)
        {
            formatPressed = false;
        }
    }

   

    
}

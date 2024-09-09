using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TimerBudilnik
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private TimeSpan _time;
        private List<DateTime> alarms;
        private DispatcherTimer _alarmChecker;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            alarms = new List<DateTime>();
            _alarmChecker = new DispatcherTimer();
            _alarmChecker.Interval = TimeSpan.FromSeconds(30); // Проверяем будильники каждые 30 секунд
            _alarmChecker.Tick += CheckAlarms;
            _alarmChecker.Start();
        }

        // Таймер

        private void StartTimerButton_Click(object sender, RoutedEventArgs e)
        {
            int hours = int.Parse(HoursTextBox.Text);
            int minutes = int.Parse(MinutesTextBox.Text);
            int seconds = int.Parse(SecondsTextBox.Text);
            _time = new TimeSpan(hours, minutes, seconds);
            _timer.Start();
        }

        private void StopTimerButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void ResetTimerButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _time = TimeSpan.Zero;
            TimerDisplay.Text = "00:00:00";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_time.TotalSeconds > 0)
            {
                _time = _time.Subtract(TimeSpan.FromSeconds(1));
                TimerDisplay.Text = _time.ToString(@"hh\:mm\:ss");
            }
            else
            {
                _timer.Stop();
                MessageBox.Show("Время вышло!");
            }
        }

        // Будильник

        private void SetAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime alarmDate = AlarmDatePicker.SelectedDate.Value;
                TimeSpan alarmTime = TimeSpan.Parse(AlarmTimeTextBox.Text);
                DateTime alarm = alarmDate + alarmTime;
                alarms.Add(alarm);
                AlarmsListBox.Items.Add(alarm.ToString("g"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неверный формат времени или даты. Попробуйте еще раз.");
            }
        }

        private void CheckAlarms(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            foreach (var alarm in alarms.ToArray())
            {
                if (alarm <= now)
                {
                    MessageBox.Show($"Будильник на {alarm.ToString("g")} сработал!");
                    alarms.Remove(alarm);
                    AlarmsListBox.Items.Remove(alarm.ToString("g"));
                }
            }
        }

        // Удаление будильника
        private void DeleteAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmsListBox.SelectedItem != null)
            {
                // Получаем выбранный будильник
                string selectedAlarm = AlarmsListBox.SelectedItem.ToString();

                // Находим будильник по тексту и удаляем его из списка
                DateTime alarmToRemove = DateTime.Parse(selectedAlarm);
                alarms.Remove(alarmToRemove);

                // Удаляем будильник из ListBox
                AlarmsListBox.Items.Remove(selectedAlarm);

                MessageBox.Show("Будильник удален.");
            }
            else
            {
                MessageBox.Show("Выберите будильник для удаления.");
            }
        }

        // Заметки

        private void SaveNotesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение пути к папке "Загрузки"
                string downloadsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string filePath = System.IO.Path.Combine(downloadsPath, "notes.txt");

                // Сохранение файла
                File.WriteAllText(filePath, NotesTextBox.Text);

                MessageBox.Show($"Заметки сохранены в папку Загрузки! Путь к файлу: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении заметок: " + ex.Message);
            }
        }
    }
}

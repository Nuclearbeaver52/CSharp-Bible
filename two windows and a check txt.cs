Описание (Description): > 🖥️ WPF Multi-Window Validation System — Приложение с двумя окнами, реализующее строгую проверку текстовых данных (метрики длины и раскладки) с визуальной обратной связью.

2. Главный файл логики (MainWindow.xaml.cs)
Я добавил подробные комментарии, объясняющие зачем и как работает каждый блок.

C#
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace WpfApp2Screens
{
    /// <summary>
    /// Логика главного окна. 
    /// Здесь происходит управление файлом, валидация текста и связь со вторым окном.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Путь к файлу. Храним в корне папки приложения.
        private const string FilePath = "description.txt";
        
        // Храним исходный текст, чтобы понимать, насколько сильно пользователь его изменил.
        private string _initialText;

        // Ссылка на экземпляр второго окна, чтобы управлять его состоянием.
        private SecondaryWindow _secondaryWindow;

        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
            // Гарантируем, что если закроется главное окно, закроется и второе (через событие Closing).
            this.Closing += MainWindow_Closing;
        }

        private void InitializeApplication()
        {
            try
            {
                // Проверяем наличие файла. Если его нет — создаем дефолтный про Koenigsegg.
                if (!File.Exists(FilePath))
                {
                    _initialText = "Описание объекта: Гиперкар «Кенигсегг Йеско Аттак»...";
                    File.WriteAllText(FilePath, _initialText);
                }
                else
                {
                    _initialText = File.ReadAllText(FilePath);
                }
                DescriptionTextBox.Text = _initialText;
            }
            catch (Exception ex)
            {
                // Обработка ошибок доступа к диску. Если файл нельзя прочитать, работать нельзя.
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Критическая ошибка");
                Application.Current.Shutdown();
            }

            // Создаем и сразу показываем второе окно (визуализацию).
            _secondaryWindow = new SecondaryWindow();
            _secondaryWindow.Show();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newText = DescriptionTextBox.Text;

            try
            {
                // Вызываем нашу кастомную проверку.
                CheckValidation(newText, _initialText);

                // Если валидация прошла (исключение не вылетело):
                File.WriteAllText(FilePath, newText);
                _initialText = newText;
                _secondaryWindow.ShowWarning(false); // Убираем "красную плашку" во втором окне.
                MessageBox.Show("Изменения сохранены!", "Успех");
            }
            catch (InvalidOperationException ioEx)
            {
                // Если валидация провалена — включаем режим тревоги во втором окне.
                _secondaryWindow.ShowWarning(true);
                MessageBox.Show(ioEx.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Метод проверки данных. Демонстрирует работу с Regex и математическую логику.
        /// </summary>
        private void CheckValidation(string newText, string originalText)
        {
            // ПРОВЕРКА 1: Длина текста не должна меняться более чем на 10%.
            int diff = Math.Abs(newText.Length - originalText.Length);
            if (diff > originalText.Length * 0.1)
            {
                throw new InvalidOperationException("Текст слишком сильно изменен по объему (+/- 10%).");
            }

            // ПРОВЕРКА 2: Запрет на использование кириллицы и латиницы одновременно.
            // Используем регулярные выражения (Regex) для поиска символов разных алфавитов.
            bool hasCyrillic = Regex.IsMatch(newText, @"[\p{IsCyrillic}]");
            bool hasLatin = Regex.IsMatch(newText, @"[a-zA-Z]");

            if (hasCyrillic && hasLatin)
            {
                throw new InvalidOperationException("Смешение языков (RU/EN) запрещено.");
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // При закрытии главного окна уведомляем пользователя и принудительно закрываем зависимое окно.
            if (_secondaryWindow != null && _secondaryWindow.IsLoaded)
            {
                _secondaryWindow.Close();
            }
        }
    }
}
3. Второстепенное окно (SecondaryWindow.xaml.cs)
Здесь демонстрируется, как программно менять UI из другого окна.

C#
using System.Windows;

namespace WpfApp2Screens
{
    /// <summary>
    /// Окно визуализации. Служит для отображения контента или предупреждения об ошибке.
    /// </summary>
    public partial class SecondaryWindow : Window
    {
        public SecondaryWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Переключает состояние интерфейса.
        /// Если isWarning = true, картинка скрывается, а красная панель с текстом показывается.
        /// </summary>
        public void ShowWarning(bool isWarning)
        {
            // Visibility.Visible - показать, Visibility.Collapsed - полностью убрать.
            WarningOverlay.Visibility = isWarning ? Visibility.Visible : Visibility.Collapsed;
            
            // Скрываем изображение, если данные не валидны.
            VisualContentImage.Visibility = isWarning ? Visibility.Hidden : Visibility.Visible;
        }
    }
}

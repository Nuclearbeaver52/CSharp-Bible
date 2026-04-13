Здесь я использовал LINQ, чтобы поиск нужного логина и пароля занимал всего одну строчку.

C#
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpBible.Projects.SimpleAuth
{
    /// <summary>
    /// Простая модель пользователя.
    /// </summary>
    public class User
    {
        public string Username { get; set; }
        // ВАЖНО: В реальных проектах пароли хранятся в виде хэшей (зашифрованными), 
        // но для простой задачи оставляем обычный текст.
        public string Password { get; set; } 
    }

    /// <summary>
    /// Логика регистрации и входа без использования БД.
    /// </summary>
    public class AuthManager
    {
        // Наша "база данных" в виде обычного списка
        private List<User> _usersDb = new List<User>();

        public AuthManager()
        {
            // Можно сразу добавить тестового админа при запуске
            _usersDb.Add(new User { Username = "admin", Password = "123" });
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        public bool Register(string username, string password)
        {
            // Any() проверяет, есть ли уже в списке кто-то с таким логином
            if (_usersDb.Any(u => u.Username == username))
            {
                // Логин занят
                return false; 
            }

            // Добавляем в список
            _usersDb.Add(new User { Username = username, Password = password });
            return true;
        }

        /// <summary>
        /// Вход в систему
        /// </summary>
        public bool Login(string username, string password)
        {
            // FirstOrDefault() ищет первую запись, где совпадает и логин, и пароль.
            // Если не найдет — вернет null.
            var user = _usersDb.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Успешный вход
                return true;
            }
            
            // Неверный логин или пароль
            return false;
        }
    }
}
2. Как это использовать (Пример проверки)
Если ты будешь тестировать это в консоли или привязывать к кнопкам в WPF, логика вызова будет выглядеть так:

C#
AuthManager auth = new AuthManager();

// Пробуем зайти под дефолтным админом
bool isAdminLogged = auth.Login("admin", "123"); 
// Вернет true

// Пробуем зарегистрировать нового
bool isRegistered = auth.Register("ivan", "qwerty"); 
// Вернет true

// Пробуем зарегистрировать с таким же именем
bool isRegisteredAgain = auth.Register("ivan", "55555"); 
// Вернет false (такой логин уже есть)

// Заходим под новым пользователем
bool isIvanLogged = auth.Login("ivan", "qwerty"); 
// Вернет true

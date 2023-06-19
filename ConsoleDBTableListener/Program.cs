
namespace ConsoleDBTableListener
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var _tracker = new TableTracker("Server=(localdb)\\mssqllocaldb;Database=Tickets;Trusted_Connection=True;", "Tickets");
            _tracker.OnRecordsFound += SoundAlarm;

            void SoundAlarm() => System.Media.SystemSounds.Asterisk.Play();
            Console.WriteLine("Простое приложение для уведомления о добавлении строк в таблицу БД");
            Console.WriteLine("Лог работы пишется в консоль");
            Console.ReadKey();

        }
    }
}
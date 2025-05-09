namespace Open_TK
{
    class Program
    {
        // Главный метод проекта. Запуск игры
        static void Main(string[] args)
        {
            // Создание окна (1400 х 800)
            using (Game game = new Game(1400, 800))
            {
                game.Run();
            }
        }
    }
}
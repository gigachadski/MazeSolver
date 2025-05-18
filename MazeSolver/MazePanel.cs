using System;

namespace MazeSolver
{
    /// <summary>  
    /// Клас MazePanel відповідає за відображення лабіринту, його редагування  
    /// та взаємодію з користувачем через графічний інтерфейс.  
    /// </summary>  
    public class MazePanel : Panel
    {
        // Розмір однієї клітинки лабіринту в пікселях  
        private readonly int _cellSize;

        /// <summary>  
        /// Двовимірний масив, що представляє лабіринт.  
        /// Значення 0 — прохідна клітинка, 1 — стіна.  
        /// </summary>  
        public int[,] Maze { get; private set; }

        /// <summary>  
        /// Початкова точка лабіринту.  
        /// </summary>  
        public Point Start { get; private set; }

        /// <summary>  
        /// Кінцева точка лабіринту.  
        /// </summary>  
        public Point End { get; private set; }

        /// <summary>  
        /// Список точок, що представляє знайдений шлях у лабіринті.  
        /// </summary>  
        public List<Point> Path { get; private set; }

        /// <summary>  
        /// Кількість рядків у лабіринті.  
        /// </summary>  
        public int Rows => Maze.GetLength(0);

        /// <summary>  
        /// Кількість стовпців у лабіринті.  
        /// </summary>  
        public int Cols => Maze.GetLength(1);

        /// <summary>  
        /// Подія, яка викликається при зміні лабіринту (наприклад, редагування стін, зміна старту/фінішу).  
        /// </summary>  
        public event EventHandler MazeChanged;

        /// <summary>  
        /// Конструктор класу MazePanel.  
        /// Ініціалізує лабіринт із заданими розмірами та налаштовує графічний інтерфейс.  
        /// </summary>  
        /// <param name="rows">Кількість рядків у лабіринті.</param>  
        /// <param name="cols">Кількість стовпців у лабіринті.</param>  
        /// <param name="cellSize">Розмір однієї клітинки в пікселях.</param>  
        public MazePanel(int rows, int cols, int cellSize)
        {
            _cellSize = cellSize;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;

            // Ініціалізація лабіринту  
            InitializeMaze(rows, cols);

            // Підписка на подію кліку миші для редагування лабіринту  
            MouseClick += OnMazeClick;
        }

        /// <summary>  
        /// Ініціалізує лабіринт із заданою кількістю рядків і стовпців.  
        /// Скидає лабіринт до порожнього стану, встановлює стартову і кінцеву точки,  
        /// а також змінює розмір панелі відповідно до нових розмірів лабіринту.  
        /// </summary>  
        /// <param name="rows">Кількість рядків у лабіринті.</param>  
        /// <param name="cols">Кількість стовпців у лабіринті.</param>  
        public void InitializeMaze(int rows, int cols)
        {
            // Створює новий масив лабіринту із заданими розмірами.  
            Maze = new int[rows, cols];

            // Встановлює стартову точку у верхньому лівому куті.  
            Start = new Point(0, 0);

            // Встановлює кінцеву точку у нижньому правому куті.  
            End = new Point(rows - 1, cols - 1);

            // Очищає будь-який існуючий шлях.  
            Path = null;

            // Змінює розмір панелі відповідно до нових розмірів лабіринту.  
            Size = new Size(cols * _cellSize, rows * _cellSize);

            // Викликає подію MazeChanged, щоб повідомити слухачів про оновлення.  
            MazeChanged?.Invoke(this, EventArgs.Empty);

            // Перемальовує панель, щоб відобразити зміни.  
            Invalidate();
        }

        /// <summary>  
        /// Генерує випадковий лабіринт із гарантованим шляхом від старту до фінішу.  
        /// </summary>  
        /// <param name="extraProbability">Ймовірність створення додаткових "дірок" у стінах лабіринту (значення за замовчуванням: 0.3).</param>  
        public void GenerateRandomMaze(double extraProbability = 0.3)
        {
            var rnd = new Random();

            // 1) Спочатку ставимо ВСІ клітинки в стіну  
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    Maze[i, j] = 1;

            // 2) Побудова гарантованого шляху від Start до End (random walk)  
            var current = Start;
            Maze[current.X, current.Y] = 0;

            var visited = new HashSet<Point> { current };
            var pathStack = new List<Point> { current };

            while (!current.Equals(End))
            {
                // Зібрати всі сусідні клітинки (по 4 напрямках), в межах і неповторні  
                var neighbors = new List<Point>();
                int[] dx = { -1, 1, 0, 0 };
                int[] dy = { 0, 0, -1, 1 };

                for (int k = 0; k < 4; k++)
                {
                    int nx = current.X + dx[k];
                    int ny = current.Y + dy[k];
                    var np = new Point(nx, ny);
                    if (nx >= 0 && nx < Rows && ny >= 0 && ny < Cols && !visited.Contains(np))
                        neighbors.Add(np);
                }

                if (neighbors.Count == 0)
                {
                    // Якщо застряли в тупику — повертаємося назад  
                    pathStack.RemoveAt(pathStack.Count - 1);
                    current = pathStack[pathStack.Count - 1];
                    continue;
                }

                // Випадково обираємо наступну клітинку  
                var next = neighbors[rnd.Next(neighbors.Count)];
                // Прокладаємо коридор  
                Maze[next.X, next.Y] = 0;
                visited.Add(next);
                pathStack.Add(next);
                current = next;
            }

            // 3) Додаємо додаткові “дірки” для унікальності  
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (Maze[i, j] == 1 && rnd.NextDouble() < extraProbability)
                        Maze[i, j] = 0;
                }
            }

            // 4) Переконаємося, що Start і End прохідні  
            Maze[Start.X, Start.Y] = 0;
            Maze[End.X, End.Y] = 0;

            Path = null;
            MazeChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>  
        /// Встановлює шлях у лабіринті, який буде відображено на панелі.  
        /// </summary>  
        /// <param name="path">Список точок (Point), що представляють шлях у лабіринті.</param>  
        /// <remarks>  
        /// Ця функція оновлює властивість Path, яка зберігає поточний шлях,  
        /// і викликає метод Invalidate(), щоб примусити панель перемалюватися  
        /// з урахуванням нового шляху.  
        /// </remarks>  
        public void SetPath(List<Point> path)
        {
            Path = path;
            Invalidate();
        }


        public bool IsValidCell(int row, int col)
        {
            return row >= 0 && row < Rows && col >= 0 && col < Cols;
        }

        /// <summary>  
        /// Редагує клітинку лабіринту відповідно до заданого режиму редагування.  
        /// </summary>  
        /// <param name="row">Рядок клітинки, яку потрібно редагувати.</param>  
        /// <param name="col">Стовпець клітинки, яку потрібно редагувати.</param>  
        /// <param name="mode">Режим редагування (наприклад, стіна, старт, фініш).</param>  
        /// <remarks>  
        /// Ця функція перевіряє, чи є клітинка дійсною, і змінює її стан залежно від режиму редагування.  
        /// Якщо клітинка є стартом або фінішем, вона не може бути змінена на стіну.  
        /// У разі встановлення старту або фінішу перевіряється, чи клітинка не є стіною.  
        /// Після редагування викликається подія MazeChanged і оновлюється відображення лабіринту.  
        /// </remarks>  
        /// <exception cref="InvalidOperationException">  
        /// Викидається, якщо намагаються встановити старт або фініш на стіну.  
        /// </exception>  
        public void EditCell(int row, int col, EditMode mode)
        {
            if (!IsValidCell(row, col))
                return;

            var point = new Point(row, col);

            switch (mode)
            {
                case EditMode.ToggleWall:
                    // Не змінювати, якщо це старт або фініш  
                    if (point.Equals(Start) || point.Equals(End))
                        return;

                    Maze[row, col] = (Maze[row, col] == 0) ? 1 : 0;
                    break;

                case EditMode.SetStart:
                    if (Maze[row, col] == 1)
                        throw new InvalidOperationException("Неможливо встановити старт на стіну.");
                    Start = point;
                    break;

                case EditMode.SetEnd:
                    if (Maze[row, col] == 1)
                        throw new InvalidOperationException("Неможливо встановити фініш на стіну.");
                    End = point;
                    break;
            }

            Path = null;
            MazeChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>  
        /// Обробляє подію кліку миші на панелі лабіринту.  
        /// Визначає, яка клітинка була натиснута, і змінює її стан відповідно до поточного режиму редагування.  
        /// </summary>  
        /// <param name="sender">Об'єкт, що викликав подію (зазвичай MazePanel).</param>  
        /// <param name="e">Аргументи події, що містять інформацію про клік миші.</param>  
        private void OnMazeClick(object sender, MouseEventArgs e)
        {
            try
            {
                // Визначає стовпець і рядок клітинки, на яку натиснули, на основі координат кліку.  
                int col = e.X / _cellSize;
                int row = e.Y / _cellSize;

                // Отримує посилання на головну форму, щоб дізнатися поточний режим редагування.  
                var form = FindForm() as MainForm;
                if (form != null)
                {
                    // Редагує клітинку відповідно до поточного режиму (наприклад, стіна, старт, фініш).  
                    EditCell(row, col, form.CurrentEditMode);
                }
            }
            catch (Exception ex)
            {
                // Відображає повідомлення про помилку, якщо щось пішло не так.  
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            try
            {
                // Малювання сітки лабіринту
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        Rectangle cellRect = new Rectangle(j * _cellSize, i * _cellSize, _cellSize, _cellSize);
                        g.FillRectangle((Maze[i, j] == 1) ? Brushes.Black : Brushes.White, cellRect);
                        g.DrawRectangle(Pens.LightGray, cellRect);
                    }
                }

                // Виділення стартової клітинки
                Rectangle startRect = new Rectangle(Start.Y * _cellSize, Start.X * _cellSize, _cellSize, _cellSize);
                g.FillRectangle(Brushes.LightGreen, startRect);
                g.DrawRectangle(Pens.Green, startRect);

                // Виділення кінцевої клітинки
                Rectangle endRect = new Rectangle(End.Y * _cellSize, End.X * _cellSize, _cellSize, _cellSize);
                g.FillRectangle(Brushes.LightCoral, endRect);
                g.DrawRectangle(Pens.Red, endRect);

                // Відображення шляху
                if (Path != null && Path.Count > 0)
                {
                    foreach (Point p in Path)
                    {
                        Rectangle cellRect = new Rectangle(p.Y * _cellSize, p.X * _cellSize, _cellSize, _cellSize);
                        g.FillRectangle(Brushes.Yellow, cellRect);
                        g.DrawRectangle(Pens.Goldenrod, cellRect);
                    }
                }
            }
            catch (Exception ex)
            {
                // Логування помилки замість показу MessageBox при малюванні
                Console.WriteLine($"Помилка при малюванні: {ex.Message}");
            }
        }
    }
}
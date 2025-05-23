namespace MazeSolver
{
    // Клас, що реалізує алгоритми пошуку шляху (Дейкстра, A* з різними евристиками)
    public class MazeSolver
    {
        private readonly int[,] _maze;
        private readonly int _rows;
        private readonly int _cols;
        private readonly Point _start;
        private readonly Point _end;
        private readonly Dictionary<Point, List<Point>> _graph;

        public MazeSolver(int[,] maze, Point start, Point end)
        {
            if (maze == null) throw new ArgumentNullException(nameof(maze));
            _maze = maze;
            _rows = maze.GetLength(0);
            _cols = maze.GetLength(1);

            if (!IsValid(start) || !IsValid(end))
                throw new ArgumentException("Start or End out of bounds.");
            if (maze[start.X, start.Y] == 1 || maze[end.X, end.Y] == 1)
                throw new ArgumentException("Start or End on a wall.");
            if (start == end)
                throw new ArgumentException("Start equals End.");

            _start = start;
            _end = end;

            // Створюємо граф списком суміжності
            var graphBuilder = new MazeGraph(maze);
            _graph = graphBuilder.AdjacencyList;
        }

        private bool IsValid(Point p) =>
            p.X >= 0 && p.X < _rows && p.Y >= 0 && p.Y < _cols;

        /// <summary>  
        /// Виконує пошук шляху в лабіринті за допомогою алгоритму Дейкстри.  
        /// </summary>  
        /// <remarks>  
        /// Алгоритм Дейкстри знаходить найкоротший шлях у графі без використання евристики.  
        /// Він працює шляхом поступового розширення найменшої вартості шляху від стартової точки до всіх інших вершин.  
        ///  
        /// Основні етапи:  
        /// 1. Ініціалізація відкритого списку (open) вузлом стартової точки.  
        /// 2. Поки є вузли у відкритому списку:  
        ///    - Вибирається вузол із найменшою вартістю (g).  
        ///    - Якщо вузол є кінцевою точкою, будується результат і повертається.  
        ///    - Вузол додається до закритого списку (closed).  
        ///    - Для кожного сусіда вузла:  
        ///      - Обчислюється нова вартість шляху.  
        ///      - Якщо сусід ще не у відкритому списку або нова вартість менша за попередню,  
        ///        додається/оновлюється вузол у відкритому списку.  
        /// 3. Якщо шлях не знайдено, повертається результат із порожнім шляхом.  
        /// </remarks>  
        /// <returns>  
        /// Об'єкт PathfindingResult, що містить знайдений шлях, кількість ітерацій та статус успішності.  
        /// </returns>  
        public PathfindingResult FindPathDijkstra()
        {
            int iterations = 0;
            var open = new List<Node>();
            var closed = new HashSet<Point>();

            // Додаємо стартовий вузол до відкритого списку  
            open.Add(new Node(_start, 0, 0, null));

            while (open.Count > 0)
            {
                iterations++;
                // Сортуємо вузли за вартістю (g) і вибираємо найменший  
                open.Sort((a, b) => a.Cost.CompareTo(b.Cost));
                var current = open[0];
                open.RemoveAt(0);

                // Якщо досягнуто кінцевої точки, будуємо результат  
                if (current.Position.Equals(_end))
                {
                    return BuildResult(current, iterations);
                }

                closed.Add(current.Position);

                // Отримуємо сусідів поточного вузла  
                if (!_graph.TryGetValue(current.Position, out var neighbors))
                    continue;

                foreach (var nb in neighbors)
                {
                    if (closed.Contains(nb)) continue;

                    double newCost = current.Cost + 1;
                    var exist = open.FirstOrDefault(n => n.Position.Equals(nb));

                    // Якщо сусід ще не у відкритому списку або нова вартість менша  
                    if (exist == null || newCost < exist.Cost)
                    {
                        var node = new Node(nb, newCost, newCost, current);
                        if (exist != null) open.Remove(exist);
                        open.Add(node);
                    }
                }
            }

            // Якщо шлях не знайдено, повертаємо порожній результат  
            return new PathfindingResult(null, iterations);
        }

        // 2) A* з манхеттенською евристикою
        public PathfindingResult FindPathAStarManhattan()
        {
            int iterations = 0;
            var open = new List<Node>();
            var closed = new HashSet<Point>();

            double h0 = HeuristicManhattan(_start, _end);
            open.Add(new Node(_start, 0, h0, null));

            while (open.Count > 0)
            {
                iterations++;
                open.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                var current = open[0];
                open.RemoveAt(0);

                if (current.Position.Equals(_end))
                {
                    return BuildResult(current, iterations);
                }

                closed.Add(current.Position);

                if (!_graph.TryGetValue(current.Position, out var neighbors))
                    continue;

                foreach (var nb in neighbors)
                {
                    if (closed.Contains(nb)) continue;

                    double newCost = current.Cost + 1;
                    double priority = newCost + HeuristicManhattan(nb, _end);
                    var exist = open.FirstOrDefault(n => n.Position.Equals(nb));

                    if (exist == null || newCost < exist.Cost)
                    {
                        var node = new Node(nb, newCost, priority, current);
                        if (exist != null) open.Remove(exist);
                        open.Add(node);
                    }
                }
            }

            return new PathfindingResult(null, iterations);
        }

        // 3) A* з евклідовою евристикою
        public PathfindingResult FindPathAStarEuclidean()
        {
            int iterations = 0;
            var open = new List<Node>();
            var closed = new HashSet<Point>();

            double h0 = HeuristicEuclidean(_start, _end);
            open.Add(new Node(_start, 0, h0, null));

            while (open.Count > 0)
            {
                iterations++;
                open.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                var current = open[0];
                open.RemoveAt(0);

                if (current.Position.Equals(_end))
                {
                    return BuildResult(current, iterations);
                }

                closed.Add(current.Position);

                if (!_graph.TryGetValue(current.Position, out var neighbors))
                    continue;

                foreach (var nb in neighbors)
                {
                    if (closed.Contains(nb)) continue;

                    double newCost = current.Cost + 1;
                    double priority = newCost + HeuristicEuclidean(nb, _end);
                    var exist = open.FirstOrDefault(n => n.Position.Equals(nb));

                    if (exist == null || newCost < exist.Cost)
                    {
                        var node = new Node(nb, newCost, priority, current);
                        if (exist != null) open.Remove(exist);
                        open.Add(node);
                    }
                }
            }

            return new PathfindingResult(null, iterations);
        }

        /// <summary>  
        /// Виконує пошук шляху в лабіринті за допомогою вибраного алгоритму.  
        /// </summary>  
        /// <param name="algorithm">Алгоритм пошуку шляху, який потрібно використати.</param>  
        /// <returns>Результат пошуку шляху, що включає знайдений шлях, кількість ітерацій та статус успішності.</returns>  
        /// <exception cref="ArgumentException">Викидається, якщо переданий алгоритм не підтримується.</exception>  
        public PathfindingResult SolveByAlgorithm(PathfindingAlgorithm algorithm)
        {
            return algorithm switch
            {
                PathfindingAlgorithm.Dijkstra => FindPathDijkstra(),
                PathfindingAlgorithm.AStarManhattan => FindPathAStarManhattan(),
                PathfindingAlgorithm.AStarEuclidean => FindPathAStarEuclidean(),
                _ => throw new ArgumentException($"Непідтримуваний алгоритм: {algorithm}"),
            };
        }

        // Манхеттенська евристика
        private double HeuristicManhattan(Point a, Point b) =>
            Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        // Евклідова евристика
        private double HeuristicEuclidean(Point a, Point b) =>
            Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

        // Відновлення шляху
        private PathfindingResult BuildResult(Node endNode, int iterations)
        {
            var path = new List<Point>();
            var cur = endNode;
            while (cur != null)
            {
                path.Add(cur.Position);
                cur = cur.Parent;
            }
            path.Reverse();
            return new PathfindingResult(path, iterations);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MazeSolver;

namespace MazeSolver
{
    // Головна форма 
    public partial class MainForm : Form
    {
        // Поле для SplitContainer, щоб була видимість у всіх методах
        private SplitContainer splitContainer;


        private MazePanel mazePanel = new MazePanel(15, 15, CellSize);
        private Button btnSolve = new Button();
        private Button btnClear = new Button();
        private Button btnGenerate = new Button();
        private ComboBox cbAlgorithm = new ComboBox();
        private GroupBox gbEditMode = new GroupBox();
        private GroupBox gbSize = new GroupBox();
        private RadioButton rbToggleWall = new RadioButton();
        private RadioButton rbSetStart = new RadioButton();
        private RadioButton rbSetEnd = new RadioButton();
        private Label lblAlgorithm = new Label();
        private Label lblResults = new Label();
        private NumericUpDown nudRows = new NumericUpDown();
        private NumericUpDown nudCols = new NumericUpDown();
        private Label lblRows = new Label();
        private Label lblCols = new Label();
        private Button btnUpdateSize = new Button();
        private StatusStrip statusStrip = new StatusStrip();
        private ToolStripStatusLabel statusLabel = new ToolStripStatusLabel();

        // Constants
        private const int CellSize = 30;
        public new const int Margin = 20;
        private const int MIN_SIZE = 5;
        private const int MAX_SIZE_ROWS = 25;
        private const int MAX_SIZE_COLS = 40;

        public EditMode CurrentEditMode { get; private set; } = EditMode.ToggleWall;

        public MainForm()
        {
            InitializeComponents();
            this.WindowState = FormWindowState.Maximized;
        }

        private void InitializeComponents()
        {
            // Form setup
            Text = "Сучасний Розв'язувач Лабіринту";
            BackColor = Color.FromArgb(240, 240, 240);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;

            // SplitContainer setup
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.7),
                IsSplitterFixed = false
            };
            Controls.Add(splitContainer);

            // MazePanel setup
            mazePanel.Dock = DockStyle.Fill;
            mazePanel.MazeChanged += MazePanel_Changed;
            splitContainer.Panel1.Controls.Add(mazePanel);

            // Initialize all controls on the right panel
            InitializeEditModeControls();
            InitializeSizeControls();
            InitializeAlgorithmControls();
            InitializeButtons();
            InitializeResultsLabel();
            InitializeStatusStrip();
        }

        /// <summary>  
        /// Ініціалізує елементи керування для вибору режиму редагування лабіринту.  
        /// </summary>  
        /// <remarks>  
        /// Ця функція створює три радіокнопки для вибору режиму редагування:  
        /// - "Редагувати стіни" (за замовчуванням)  
        /// - "Встановити старт"  
        /// - "Встановити фініш"  
        ///  
        /// Всі кнопки додаються до групового контейнера (GroupBox),  
        /// який розташовується у правій панелі інтерфейсу (Panel2).  
        ///  
        /// Подія CheckedChanged для кожної кнопки підписана на метод RbEditMode_CheckedChanged,  
        /// який змінює поточний режим редагування залежно від вибраної кнопки.  
        /// </remarks>  
        private void InitializeEditModeControls()
        {
            // Radio buttons  
            rbToggleWall.Text = "Редагувати стіни";
            rbToggleWall.Dock = DockStyle.Top;
            rbToggleWall.Checked = true;
            rbToggleWall.CheckedChanged += RbEditMode_CheckedChanged;

            rbSetStart.Text = "Встановити старт";
            rbSetStart.Dock = DockStyle.Top;
            rbSetStart.CheckedChanged += RbEditMode_CheckedChanged;

            rbSetEnd.Text = "Встановити фініш";
            rbSetEnd.Dock = DockStyle.Top;
            rbSetEnd.CheckedChanged += RbEditMode_CheckedChanged;

            // GroupBox  
            gbEditMode.Text = "Режим редагування";
            gbEditMode.Dock = DockStyle.Top;
            gbEditMode.Height = 120;
            gbEditMode.Controls.AddRange(new Control[] { rbToggleWall, rbSetStart, rbSetEnd });
            splitContainer.Panel2.Controls.Add(gbEditMode);
        }

        /// <summary>  
        /// Ініціалізує елементи керування для налаштування розміру лабіринту.  
        /// </summary>  
        /// <remarks>  
        /// Цей метод створює елементи інтерфейсу для зміни розмірів лабіринту:  
        /// - Два текстові підписи ("Рядки" і "Стовпці").  
        /// - Два елементи NumericUpDown для введення кількості рядків і стовпців.  
        /// - Кнопку "Оновити розмір" для застосування змін.  
        ///  
        /// Всі ці елементи додаються до групового контейнера (GroupBox),  
        /// який розташовується у правій панелі інтерфейсу (Panel2).  
        ///  
        /// Подія Click для кнопки "Оновити розмір" підписана на метод BtnUpdateSize_Click,  
        /// який виконує перевірку введених значень і оновлює розмір лабіринту.  
        /// </remarks>  
        private void InitializeSizeControls()
        {
            // Labels  
            lblRows.Text = "Рядки:";
            lblRows.Dock = DockStyle.Top;
            lblCols.Text = "Стовпці:";
            lblCols.Dock = DockStyle.Top;

            // NumericUpDown  
            nudRows.Minimum = MIN_SIZE;
            nudRows.Maximum = MAX_SIZE_ROWS;
            nudRows.Value = mazePanel.Rows;
            nudRows.Dock = DockStyle.Top;

            nudCols.Minimum = MIN_SIZE;
            nudCols.Maximum = MAX_SIZE_COLS;
            nudCols.Value = mazePanel.Cols;
            nudCols.Dock = DockStyle.Top;

            // Button  
            btnUpdateSize.Text = "Оновити розмір";
            btnUpdateSize.Dock = DockStyle.Top;
            btnUpdateSize.Click += BtnUpdateSize_Click;

            // GroupBox  
            gbSize.Text = "Налаштування розміру поля";
            gbSize.Dock = DockStyle.Top;
            gbSize.Height = 140;
            gbSize.Controls.AddRange(new Control[] { lblRows, nudRows, lblCols, nudCols, btnUpdateSize });
            splitContainer.Panel2.Controls.Add(gbSize);
        }

        /// <summary>  
        /// Ініціалізує елементи керування для вибору алгоритму пошуку шляху.  
        /// </summary>  
        /// <remarks>  
        /// Цей метод створює текстовий підпис ("Алгоритм пошуку") та випадаючий список (ComboBox),  
        /// який дозволяє користувачеві вибрати один із доступних алгоритмів пошуку шляху:  
        /// - "Дейкстра"  
        /// - "A* (манхеттенська евристика)"  
        /// - "A* (евклідова евристика)"  
        ///  
        /// Випадаючий список налаштований так, щоб користувач міг вибрати лише один із варіантів (DropDownList).  
        /// За замовчуванням вибраний перший елемент ("Дейкстра").  
        /// Обидва елементи додаються до правої панелі інтерфейсу (Panel2) у SplitContainer.  
        /// </remarks>  
        private void InitializeAlgorithmControls()
        {
            // Algorithm label  
            lblAlgorithm.Text = "Алгоритм пошуку:";
            lblAlgorithm.Dock = DockStyle.Top;
            splitContainer.Panel2.Controls.Add(lblAlgorithm);

            // ComboBox  
            cbAlgorithm.Items.AddRange(new object[] {
               "Дейкстра",
               "A* (манхеттенська евристика)",
               "A* (евклідова евристика)"
           });
            cbAlgorithm.SelectedIndex = 0;
            cbAlgorithm.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAlgorithm.Dock = DockStyle.Top;
            splitContainer.Panel2.Controls.Add(cbAlgorithm);
        }

        private void InitializeButtons()
        {
            // Solve button
            btnSolve.Text = "Пошук шляху";
            btnSolve.Dock = DockStyle.Top;
            btnSolve.Click += BtnSolve_Click;
            splitContainer.Panel2.Controls.Add(btnSolve);

            // Clear button
            btnClear.Text = "Очистити лабіринт";
            btnClear.Dock = DockStyle.Top;
            btnClear.Click += BtnClear_Click;
            splitContainer.Panel2.Controls.Add(btnClear);

            // Generate button
            btnGenerate.Text = "Випадковий лабіринт";
            btnGenerate.Dock = DockStyle.Top;
            btnGenerate.Click += BtnGenerate_Click;
            splitContainer.Panel2.Controls.Add(btnGenerate);
        }

        private void InitializeResultsLabel()
        {
            lblResults.Text = "Результати:";
            lblResults.Dock = DockStyle.Top;
            lblResults.Height = 80;
            splitContainer.Panel2.Controls.Add(lblResults);
        }

        private void InitializeStatusStrip()
        {
            statusStrip.Dock = DockStyle.Bottom;
            statusStrip.Items.Add(statusLabel);
            Controls.Add(statusStrip);
        }

        // Event handlers
        private void MazePanel_Changed(object? sender, EventArgs e)
        {
            lblResults.Text = "Результати:";
        }

        private void RbEditMode_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbSetStart.Checked) CurrentEditMode = EditMode.SetStart;
            else if (rbSetEnd.Checked) CurrentEditMode = EditMode.SetEnd;
            else CurrentEditMode = EditMode.ToggleWall;
        }

        /// <summary>  
        /// Оновлює текст статусного рядка внизу форми.  
        /// </summary>  
        /// <param name="message">Текст повідомлення, який потрібно відобразити у статусному рядку.</param>  
        /// <remarks>  
        /// Ця функція змінює текст статусного рядка, використовуючи передане повідомлення.  
        /// Вона може бути викликана для інформування користувача про поточний стан програми,  
        /// наприклад, про успішне виконання дії або виникнення помилки.  
        /// </remarks>  
        private void UpdateStatus(string message)
        {
            statusLabel.Text = message;
        }

        private void BtnUpdateSize_Click(object? sender, EventArgs e)
        {
            try
            {
                int rows = (int)nudRows.Value;
                int cols = (int)nudCols.Value;

                if (rows < MIN_SIZE || rows > MAX_SIZE_ROWS ||
                    cols < MIN_SIZE || cols > MAX_SIZE_COLS)
                {
                    MessageBox.Show(
                        $"Розмір повинен бути між {MIN_SIZE} і {MAX_SIZE_ROWS} для рядків та {MIN_SIZE} і {MAX_SIZE_COLS} для стовпців.",
                        "Неприпустимий розмір",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                mazePanel.InitializeMaze(rows, cols);
                UpdateStatus($"Розмір лабіринту змінено на {rows}x{cols}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при оновленні розміру: " + ex.Message,
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Помилка при оновленні розміру.");
            }
        }

        private void BtnSolve_Click(object? sender, EventArgs e)
        {
            try
            {
                UpdateStatus("Виконується пошук шляху...");
                PathfindingAlgorithm algorithm;
                string algorithmName = cbAlgorithm.SelectedItem!.ToString()!;

                switch (algorithmName)
                {
                    case "Дейкстра":
                        algorithm = PathfindingAlgorithm.Dijkstra;
                        break;
                    case "A* (манхеттенська евристика)":
                        algorithm = PathfindingAlgorithm.AStarManhattan;
                        break;
                    case "A* (евклідова евристика)":
                        algorithm = PathfindingAlgorithm.AStarEuclidean;
                        break;
                    default:
                        throw new ArgumentException("Невідомий алгоритм");
                }

                var solver = new MazeSolver(mazePanel.Maze, mazePanel.Start, mazePanel.End);
                var result = solver.SolveByAlgorithm(algorithm);

                if (result.IsPathFound)
                {
                    mazePanel.SetPath(result.Path);
                    lblResults.Text =
                        $"Алгоритм: {algorithmName}\n" +
                        $"Довжина шляху: {result.Path.Count}\n" +
                        $"Ітерацій: {result.IterationCount}";
                    SavePathToFile(result.Path, algorithmName, result.Path.Count, result.IterationCount);
                    UpdateStatus($"Шлях знайдено! Довжина: {result.Path.Count}, Ітерації: {result.IterationCount}");
                    MessageBox.Show(
                        $"Шлях знайдено!\nДовжина шляху: {result.Path.Count}\n" +
                        $"Кількість ітерацій: {result.IterationCount}\n" +
                        $"Результати збережено у файл results.txt",
                        "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblResults.Text = "Шлях не знайдено!";
                    UpdateStatus("Шлях не знайдено! Перевірте, чи існує прохід між стартом і фінішем.");
                    MessageBox.Show("Шлях не знайдено!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при пошуку шляху: " + ex.Message,
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Помилка при пошуку шляху.");
            }
        }

        /// <summary>  
        /// Очищає лабіринт, скидаючи його до початкового стану.  
        /// </summary>  
        /// <remarks>  
        /// Ця функція викликає метод InitializeMaze для повторної ініціалізації лабіринту  
        /// з поточними розмірами, видаляючи всі стіни, шлях та інші зміни.  
        /// Також оновлює статус програми, щоб відобразити успішне очищення.  
        /// У разі виникнення помилки відображає повідомлення з описом помилки.  
        /// </remarks>  
        private void BtnClear_Click(object? sender, EventArgs e)
        {
            try
            {
                // Повторна ініціалізація лабіринту з поточними розмірами  
                mazePanel.InitializeMaze(mazePanel.Rows, mazePanel.Cols);

                // Оновлення статусу програми  
                UpdateStatus("Лабіринт очищено.");
            }
            catch (Exception ex)
            {
                // Відображення повідомлення про помилку у разі невдачі  
                MessageBox.Show("Помилка при очищенні лабіринту: " + ex.Message,
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>  
        /// Генерує випадковий лабіринт із гарантованим шляхом від старту до фінішу.  
        /// </summary>  
        /// <remarks>  
        /// Ця функція викликає метод GenerateRandomMaze() у панелі лабіринту (MazePanel),  
        /// який створює новий лабіринт із випадковим розташуванням стін,  
        /// але з обов'язковим прохідним шляхом між стартовою і кінцевою точками.  
        /// У разі успіху оновлює статус програми.  
        /// Якщо виникає помилка, відображає повідомлення з описом помилки.  
        /// </remarks>  
        private void BtnGenerate_Click(object? sender, EventArgs e)
        {
            try
            {
                // Викликає метод для генерації випадкового лабіринту  
                mazePanel.GenerateRandomMaze();

                // Оновлює статус у нижній панелі  
                UpdateStatus("Згенеровано випадковий лабіринт.");
            }
            catch (Exception ex)
            {
                // Відображає повідомлення про помилку у разі невдачі  
                MessageBox.Show("Помилка при генерації лабіринту: " + ex.Message,
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>  
        /// Зберігає результати пошуку шляху у текстовий файл.  
        /// </summary>  
        /// <param name="path">Список точок (Point), що представляють знайдений шлях.</param>  
        /// <param name="algorithm">Назва алгоритму, який використовувався для пошуку шляху.</param>  
        /// <param name="pathLength">Довжина знайденого шляху.</param>  
        /// <param name="iterations">Кількість ітерацій, виконаних під час пошуку шляху.</param>  
        /// <exception cref="ArgumentNullException">Викидається, якщо шлях (path) дорівнює null.</exception>  
        /// <exception cref="IOException">Викидається, якщо виникла помилка запису у файл.</exception>  
        private void SavePathToFile(List<Point> path, string algorithm, int pathLength, int iterations)
        {

            if (path == null)

                throw new ArgumentNullException(nameof(path), "Шлях не може бути null");



            try

            {

                using var writer = new StreamWriter("results.txt");

                writer.WriteLine("======================================");

                writer.WriteLine("      Результати пошуку шляху         ");

                writer.WriteLine("======================================");

                writer.WriteLine($"Алгоритм пошуку: {algorithm}");

                writer.WriteLine($"Довжина шляху: {pathLength}");

                writer.WriteLine($"Кількість ітерацій: {iterations}");

                writer.WriteLine("======================================");

                writer.WriteLine("Шлях (координати):");

                foreach (Point p in path)

                    writer.WriteLine($"({p.X}, {p.Y})");

                writer.WriteLine("======================================");

                writer.WriteLine($"Час запису: {DateTime.Now}");

            }

            catch (IOException ioEx)

            {

                throw new IOException("Помилка запису у файл", ioEx);

            }

        }

    }
}
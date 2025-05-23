

namespace MazeSolver 
{
    // Головна форма 
    public partial class MainForm : Form
    {
        // Поле для SplitContainer, щоб була видимість у всіх методах
        private SplitContainer splitContainer;

        // Ініціалізація MazePanel та інших контролів
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
        private void BtnClear_Click(object? sender, EventArgs e)
        {
            try
            {
                mazePanel.InitializeMaze(mazePanel.Rows, mazePanel.Cols);
                UpdateStatus("Лабіринт очищено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при очищенні лабіринту: " + ex.Message,
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnGenerate_Click(object? sender, EventArgs e)
        {
            try
            {
                mazePanel.GenerateRandomMaze();
                UpdateStatus("Згенеровано випадковий лабіринт.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при генерації лабіринту: " + ex.Message,
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        $"Результати збережено у файл.",
                        "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblResults.Text = $"Алгоритм: {algorithmName}\n" +
                                      $"Шлях не знайдено!\n" +
                                      $"Ітерацій: {result.IterationCount}";
                    UpdateStatus("Шлях не знайдено! Перевірте, чи існує прохід між стартом і фінішем.");
                    MessageBox.Show("Шлях не знайдено!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show($"Помилка налаштування пошуку: {argEx.Message}",
                               "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateStatus($"Помилка: {argEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при пошуку шляху: " + ex.Message,
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Помилка при пошуку шляху.");
            }
        }

        private void SavePathToFile(List<Point> path, string algorithm, int pathLength, int iterations) 
        {
            if (path == null)
            {
                UpdateStatus("Шлях не знайдено, збереження скасовано.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстові файли (*.txt)|*.txt|Всі файли (*.*)|*.*";
            saveFileDialog.Title = "Зберегти результати пошуку шляху";
            string safeAlgorithmName = string.Join("_", algorithm.Split(Path.GetInvalidFileNameChars()));
            saveFileDialog.FileName = $"Результати_{safeAlgorithmName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.AddExtension = true;
            saveFileDialog.OverwritePrompt = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    using (var writer = new StreamWriter(filePath))
                    {
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
                    UpdateStatus($"Результати збережено у файл: {filePath}");
                    MessageBox.Show($"Результати успішно збережено у файл:\n{filePath}", "Збереження файлу", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show($"Помилка запису у файл: {ioEx.Message}", "Помилка збереження", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus($"Помилка запису у файл: {ioEx.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Невідома помилка при збереженні файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus($"Помилка збереження: {ex.Message}");
                }
            }
            else
            {
                UpdateStatus("Збереження файлу скасовано користувачем.");
            }
        }
    }
}
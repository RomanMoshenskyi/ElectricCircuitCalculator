using System.Drawing.Imaging;

namespace ElectricCircuitCalculator
{
    public partial class Form1 : Form
    {
        private Bitmap drawingCanvas;

        private readonly List<(int x, int y, string resistance)> resistors;

        public Form1()
        {
            InitializeComponent();
            drawingCanvas = new Bitmap(circuitPictureBox.Width, circuitPictureBox.Height);
            resistors = new List<(int x, int y, string resistance)>();
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            try
            {
                string circuitExpression = circuitExpressionTextBox.Text;
                double totalResistance = CalculateTotalResistance(circuitExpression);
                resultLabel.Text = $"Total Resistance: {Math.Round(totalResistance, 3)} Ohms";
                drawingCanvas = new Bitmap(circuitPictureBox.Width, circuitPictureBox.Height);
                // Draw resistors
                circuitPictureBox.Image = drawingCanvas;
                DrawResistors(circuitExpression);
            }
            catch (Exception)
            {
                MessageBox.Show($"Здається ви використовуєте незрозумілі символи", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void DrawResistors(string expression)
        {
            using Graphics g = Graphics.FromImage(drawingCanvas);
            Stack<double> values = new();
            int x = 20;
            int def_y = 20;
            int y = 20;

            for (int i = 0; i < expression.Length; i++)
            {
                char currentChar = expression[i];
                char pChar = i > 0 ? expression[i - 1] : ' ';

                if (char.IsDigit(currentChar))
                {
                    string number = "";
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        number += expression[i];
                        i++;
                    }
                    i--;

                    double resistance = double.Parse(number);
                    resistors.Add((x, y, resistance.ToString()));
                    values.Push(resistance);

                    // Draw resistor
                    DrawResistor(g, x, y, resistance.ToString());

                    // Draw connection line before resistor
                    if (pChar == '+')
                    {
                        if (expression[i - 3] == ')')
                        {
                            g.DrawLine(Pens.Black, x, y + 10, x - 75, y + 10);
                        }
                        else
                        {

                            g.DrawLine(Pens.Black, x, y + 10, x - 50, y + 10);
                            g.DrawLine(Pens.Black, x + 50, y + 10, x + 75, y + 10);
                        }
                    }
                    else
                    {

                        g.DrawLine(Pens.Black, x, y + 10, x - 25, y + 10);
                        g.DrawLine(Pens.Black, x + 50, y + 10, x + 75, y + 10);
                    }
                    if (pChar == '*')
                    {
                        g.DrawLine(Pens.White, x, y + 10, x - 25, y + 10);
                        g.DrawLine(Pens.White, x + 50, y + 10, x + 75, y + 10);
                        g.DrawLine(Pens.Black, x, y + 10, x, y - 40);
                        g.DrawLine(Pens.Black, x + 50, y + 10, x + 50, y - 40);
                    }
                    else if (pChar == '(')
                    {
                        if (expression[i - 3] == '*')
                        {
                            g.DrawLine(Pens.Black, x + 25, y, x + 25, y - 40);
                            g.DrawLine(Pens.White, x, y + 10, x - 25, y + 10);
                        }
                    }
                }
                else if (currentChar == '+')
                {

                    if (pChar == '(')
                    {
                        y += 50;
                    }
                    else if (pChar == ')')
                    {
                        y = def_y;
                        x -= 100;
                    }
                    x += 100;
                }
                else if (currentChar == '*')
                {
                    if (pChar == '(')
                    {
                        y -= 50;
                    }
                    else if (pChar == ')')
                    {
                        y += 50;
                        x -= 150;

                    }
                    y += 50;
                }
                else if (currentChar == '(')
                {
                    if (pChar == '+')
                    {

                        if (expression[i - 2] == ')')
                        {
                            x -= 50;
                        }
                        else if (char.IsDigit(expression[i - 2]))
                            x -= 50;
                        y -= 50;
                        x += 50;

                    }
                    else if (pChar == '*')
                    {
                        if (expression[i - 2] == ')')
                        {
                            x -= 50;
                        }
                        y -= 50;
                        x -= 50;
                    }
                    else if (pChar == '(')
                    {
                        y -= 50;
                    }
                    y += 50;
                }
                else if (currentChar == ')')
                {
                    if (pChar == ')')
                    {
                        y += 50;
                        x -= 100;
                        y = def_y;
                    }
                    if (pChar == ')')
                    {
                        y += 50;
                        if (char.IsDigit(expression[i - 3]))
                        {
                            g.DrawLine(Pens.Black, x + 25, y, x + 25, y - 40);
                            g.DrawLine(Pens.White, x + 50, y + 10, x + 75, y + 10);
                        }
                    }
                    y -= 50;
                    x += 100;
                }
            }
        }

        private static void DrawResistor(Graphics g, int x, int y, string resistance)
        {
            // Draw resistor as a rectangle
            Rectangle rect = new(x, y, 50, 20);
            g.FillRectangle(Brushes.Gray, rect);

            // Draw resistance value inside the rectangle
            Font font = new("Arial", 8);
            g.DrawString(resistance, font, Brushes.Black, x + 10, y + 5);
        }

        private static double CalculateTotalResistance(string expression)
        {
            return EvaluateExpression(expression);
        }
        private static double EvaluateExpression(string expression)
        {
            Stack<double> values = new();
            Stack<char> operators = new();

            for (int i = 0; i < expression.Length; i++)
            {
                char currentChar = expression[i];

                if (char.IsDigit(currentChar))
                {
                    string number = "";
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        number += expression[i];
                        i++;
                    }
                    i--;

                    values.Push(double.Parse(number));
                }
                else if (currentChar == '(')
                {
                    operators.Push(currentChar);
                }
                else if (currentChar == ')')
                {
                    while (operators.Count > 0 && operators.Peek() != '(')
                    {
                        values.Push(PerformOperation(operators.Pop(), values.Pop(), values.Pop()));
                    }
                    operators.Pop();
                }
                else if (currentChar == '+' || currentChar == '*')
                {
                    while (operators.Count > 0 && HasPrecedence(currentChar, operators.Peek()))
                    {
                        values.Push(PerformOperation(operators.Pop(), values.Pop(), values.Pop()));
                    }
                    operators.Push(currentChar);
                }
            }

            while (operators.Count > 0)
            {
                values.Push(PerformOperation(operators.Pop(), values.Pop(), values.Pop()));
            }

            return values.Pop();
        }
        private static bool HasPrecedence(char op1, char op2)
        {
            if (op2 == '(' || op2 == ')')
                return false;
            if ((op1 == '*' && op2 == '+') || (op1 == '+' && op2 == '*'))
                return false;
            return true;
        }
        private static double PerformOperation(char op, double b, double a)
        {
            return op switch
            {
                '+' => a + b,
                '*' => 1 / ((1 / a) + (1 / b)),
                _ => throw new ArgumentException("Невідомий оператор"),
            };
        }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    string fileContent = File.ReadAllText(filePath);
                    circuitExpressionTextBox.Text = fileContent;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg|All Files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = saveFileDialog.FileName;

                    // Get file extension
                    string extension = Path.GetExtension(filePath).ToLower();

                    // Saving an image depending on the resolution
                    switch (extension)
                    {
                        case ".png":
                            // Set a transparent background
                            SaveImageWithTransparentBackground(filePath, drawingCanvas);
                            break;
                        case ".jpg":
                        case ".jpeg":
                            // Set a white background
                            SaveImageWithWhiteBackground(filePath, drawingCanvas);
                            break;
                        default:
                            MessageBox.Show("Unsupported file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    MessageBox.Show("Circuit scheme saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private static void SaveImageWithTransparentBackground(string filePath, Bitmap image)
        {
            // Create a new Bitmap with a transparent background
            Bitmap newImage = new(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(image, 0, 0, image.Width, image.Height);
            }

            // Save image in PNG format with a transparent background
            newImage.Save(filePath, ImageFormat.Png);
        }

        private static void SaveImageWithWhiteBackground(string filePath, Bitmap image)
        {
            // Create a new Bitmap with a white background
            Bitmap newImage = new(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.Clear(Color.White);
                g.DrawImage(image, 0, 0, image.Width, image.Height);
            }

            // Save image as JPG with white background
            newImage.Save(filePath, ImageFormat.Jpeg);
        }
    }
}
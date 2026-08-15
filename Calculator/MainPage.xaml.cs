using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Calculator
{
    /// <summary>
    /// Hauptseite der Taschenrechner-Anwendung mit Gamepad-Navigation.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string _currentValue = "0";
        private string _previousValue = "";
        private string _operator = "";
        private bool _isNewEntry = true;

        /// <summary>
        /// Initialisiert eine neue Instanz der MainPage-Klasse.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Behandelt das Klick-Ereignis aller Schaltflächen.
        /// </summary>
        /// <param name="sender">Die Schaltfläche, die das Ereignis ausgelöst hat.</param>
        /// <param name="e">Ereignisdaten.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            switch (button.Name)
            {
                case "Btn0":
                case "Btn1":
                case "Btn2":
                case "Btn3":
                case "Btn4":
                case "Btn5":
                case "Btn6":
                case "Btn7":
                case "Btn8":
                case "Btn9":
                    InputNumber(button.Content.ToString());
                    break;
                case "BtnAdd":
                    InputOperator("+");
                    break;
                case "BtnSubtract":
                    InputOperator("−");
                    break;
                case "BtnMultiply":
                    InputOperator("×");
                    break;
                case "BtnDivide":
                    InputOperator("÷");
                    break;
                case "BtnEquals":
                    InputEquals();
                    break;
                case "BtnClear":
                    Clear();
                    break;
                case "BtnPlusMinus":
                    PlusMinus();
                    break;
                case "BtnPercent":
                    Percent();
                    break;
            }
        }

        /// <summary>
        /// Verarbeitet die Eingabe einer Ziffer.
        /// </summary>
        /// <param name="digit">Die eingegebene Ziffer.</param>
        private void InputNumber(string digit)
        {
            if (_isNewEntry)
            {
                _currentValue = digit == "0" ? "0" : digit;
                _isNewEntry = false;
                if (digit == "0" && _currentValue == "0")
                    _currentValue = "0";
            }
            else
            {
                if (_currentValue == "0" && digit != ".")
                    _currentValue = digit;
                else
                    _currentValue += digit;
            }

            UpdateDisplay();
        }

        /// <summary>
        /// Verarbeitet die Eingabe eines Operators.
        /// </summary>
        /// <param name="op">Der eingegebene Operator.</param>
        private void InputOperator(string op)
        {
            if (!string.IsNullOrEmpty(_previousValue) && !_isNewEntry)
            {
                InputEquals();
            }

            _previousValue = _currentValue;
            _operator = op;
            _isNewEntry = true;
            UpdateExpressionDisplay();
        }

        /// <summary>
        /// Führt die Berechnung durch und zeigt das Ergebnis an.
        /// </summary>
        private void InputEquals()
        {
            if (string.IsNullOrEmpty(_previousValue) || string.IsNullOrEmpty(_operator))
                return;

            double previous = 0;
            double current = 0;

            if (!double.TryParse(_previousValue, out previous) || !double.TryParse(_currentValue, out current))
            {
                Display.Text = "Fehler";
                return;
            }

            double result = PerformCalculation(previous, current, _operator);

            if (double.IsInfinity(result) || double.IsNaN(result))
            {
                Display.Text = "Fehler";
                ExpressionDisplay.Text = "";
                _currentValue = "0";
                _previousValue = "";
                _operator = "";
                _isNewEntry = true;
                return;
            }

            _currentValue = FormatResult(result);
            _previousValue = "";
            _operator = "";
            _isNewEntry = true;
            ExpressionDisplay.Text = "";
            UpdateDisplay();
        }

        /// <summary>
        /// Setzt den Taschenrechner in den Ausgangszustand zurück.
        /// </summary>
        private void Clear()
        {
            _currentValue = "0";
            _previousValue = "";
            _operator = "";
            _isNewEntry = true;
            ExpressionDisplay.Text = "";
            UpdateDisplay();
        }

        /// <summary>
        /// Wechselt das Vorzeichen des aktuellen Wertes.
        /// </summary>
        private void PlusMinus()
        {
            double value = 0;
            if (double.TryParse(_currentValue, out value))
            {
                value = -value;
                _currentValue = FormatResult(value);
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Wandelt den aktuellen Wert in einen Prozentwert um.
        /// </summary>
        private void Percent()
        {
            double value = 0;
            if (double.TryParse(_currentValue, out value))
            {
                value = value / 100.0;
                _currentValue = FormatResult(value);
                _isNewEntry = true;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Führt die arithmetische Berechnung mit den beiden Operanden durch.
        /// </summary>
        /// <param name="first">Der erste Operand.</param>
        /// <param name="second">Der zweite Operand.</param>
        /// <param name="op">Der Operator.</param>
        /// <returns>Das Berechnungsergebnis.</returns>
        private double PerformCalculation(double first, double second, string op)
        {
            switch (op)
            {
                case "+":
                    return first + second;
                case "−":
                    return first - second;
                case "×":
                    return first * second;
                case "÷":
                    if (second == 0)
                        return double.NaN;
                    return first / second;
                default:
                    return second;
            }
        }

        /// <summary>
        /// Aktualisiert die Hauptanzeige mit dem aktuellen Wert.
        /// </summary>
        private void UpdateDisplay()
        {
            Display.Text = _currentValue;
        }

        /// <summary>
        /// Aktualisiert die Ausdrucksanzeige mit dem vorherigen Wert und dem Operator.
        /// </summary>
        private void UpdateExpressionDisplay()
        {
            if (!string.IsNullOrEmpty(_previousValue) && !string.IsNullOrEmpty(_operator))
            {
                ExpressionDisplay.Text = _previousValue + " " + _operator;
            }
        }

        /// <summary>
        /// Formatiert ein Ergebnis und entfernt nachfolgende Nullen.
        /// </summary>
        /// <param name="value">Der zu formatierende Wert.</param>
        /// <returns>Der formatierte Wert als Zeichenfolge.</returns>
        private string FormatResult(double value)
        {
            if (value == (long)value)
                return ((long)value).ToString();

            string formatted = value.ToString("F10");
            formatted = formatted.TrimEnd('0');
            if (formatted.EndsWith("."))
                formatted = formatted.TrimEnd('.');

            return formatted;
        }
    }
}

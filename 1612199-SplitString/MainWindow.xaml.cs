using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _1612199_SplitString
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public static bool IsNumberic(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsDigit(s[i]) == false)
                    return false;
            }

            return true;
        }

        private void BtnSum_Click(object sender, RoutedEventArgs e)
        {
            // xoa tat ca cac khoang trang
            string line = txbInput.Text;
            var tokens = line.Split(new string[] { " " },
                StringSplitOptions.RemoveEmptyEntries)
                .Select(token => token.Trim()) 
                .ToArray();

            // kiem tra co bao nhieu toan hang
            var strCountOperands = line.Split(new string[] { "+", "-" },
                StringSplitOptions.RemoveEmptyEntries)
                .Select(token => token.Trim())
                .ToArray();

            int nCountOperands = strCountOperands.Length;

            StringBuilder sb = new StringBuilder();
            foreach (var s in tokens)
            {
                sb.Append(s);
            }

            string expression = sb.ToString();

            // kiem tra tinh dung dan cua bieu thuc
            bool bIsTrueExpression = true;

            int nCountOperator = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '+' || expression[i] == '-')
                {
                    if ( !char.IsDigit(expression[i + 1]))
                    {
                        bIsTrueExpression = false;
                    }
                    nCountOperator++;
                }
                else
                {
                    if (char.IsLetter(expression[i]) == true)
                    {
                        bIsTrueExpression = false;
                    }
                }
            }

            if (bIsTrueExpression == false ||
                nCountOperator + 1 != nCountOperands) 
            {
                MessageBox.Show("Biểu thức bạn nhập vào bị sai", "Thông báo",
                      MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // danh sach cac toan hang
                var Operands = expression.Split(new string[] { "+", "-" },
                StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

                int nSize = Operands.Length;

                CFraction[] ArrayFraction = new CFraction[nSize];

                for (int i = 0; i < nSize; i++)
                {
                    ArrayFraction[i] = new CFraction();
                    ArrayFraction[i].FromString(Operands[i]);
                }


                // danh sach cac toan tu
                List<char> Operators = new List<char>();
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == '+' || expression[i] == '-')
                    {
                        Operators.Add(expression[i]);
                    }
                }

                // tinh tong
                CFraction Sum = new CFraction(ArrayFraction[0]);

                for (int i = 1; i < nSize; i++)
                {
                    if (Operators[i - 1] == '+')
                    {
                        Sum = Sum + ArrayFraction[i];
                    }
                    if (Operators[i - 1] == '-')
                    {
                        Sum = Sum - ArrayFraction[i];
                    }
                }

                txbResult.Text = Sum.Simplify().ToString();
            }
        }
    }

    public class CFraction
    {
        const int ERROR_DENOMINATOR = 0;
        private int _num;
        private int _den;

        public CFraction()
        {
            _num = 0;
            _den = 1;
        }
        public CFraction(int num, int den = 1)
        {
            _num = num;
            _den = den == 0 ? 1 : den;
        }
        public CFraction(CFraction cf)
        {
            this._num = cf._num;
            this._den = cf._den;
        }
        ~CFraction()
        {
          
        }

        public int Numerator { get => _num; set => _num = value; }
        public int Denominator { get => _den; set => _den = value == 0 ? 1 : value; }

        public CFraction Simplify()
        {
            if (this.Denominator != 0 && this.Numerator != 0)
            {
                int nMin = Math.Abs(this.Numerator);

                if (this.Denominator < 0)
                {
                    this.Numerator = -this.Numerator;
                    this.Denominator = -this.Denominator;
                }

                if (Math.Abs(this.Numerator) > Math.Abs(this.Denominator))
                {
                    nMin = Math.Abs(this.Denominator);
                }

                int nCommonDivisor = 1;
                for (int i = nMin; i >= 1; i--)
                {
                    if (this.Numerator % i == 0 && this.Denominator % i == 0)
                    {
                        nCommonDivisor = i;
                        break;
                    }
                }

                this.Numerator /= nCommonDivisor;
                this.Denominator /= nCommonDivisor;
            }

            CFraction result = new CFraction(this);

            return result;
        }

        public static CFraction operator + (CFraction fr1, CFraction fr2)
        {
            CFraction result = new CFraction(fr1.Numerator * fr2.Denominator
                + fr2.Numerator * fr1.Denominator, fr1.Denominator * fr2.Denominator);

            return result;
        }
        public static CFraction operator -(CFraction fr1, CFraction fr2)
        {
            CFraction result = new CFraction(fr1.Numerator * fr2.Denominator
                 - fr2.Numerator * fr1.Denominator, fr1.Denominator * fr2.Denominator);

            return result;
        }

        public void FromString(string str)
        {
            if (str.IndexOf('/') != -1)
            {
                var tokens = str.Split(new string[] { "/" },
                StringSplitOptions.RemoveEmptyEntries)
                .Select(token => token.Trim())
                .ToArray();

                if (MainWindow.IsNumberic(tokens[0]) == false ||
                    MainWindow.IsNumberic(tokens[1]) == false)
                {
                    this.Numerator = 0;
                    this.Denominator = ERROR_DENOMINATOR;
                }
                else
                {
                    this.Numerator = int.Parse(tokens[0]);
                    this.Denominator = int.Parse(tokens[1]);
                }
            }
            else
            {
                this.Numerator = int.Parse(str);
                this.Denominator = 1;
            }
        }

        public int ToInt(out bool isComplete)
        {
            int result = 0;

            CFraction fr = new CFraction(this);

            fr.Simplify();

            if (fr.Denominator == 1)
            {
                result = fr.Numerator;
                isComplete = true;
            }
            else
            {
                result = 0;
                isComplete = false;
            }

            return result;
        }

        public override string ToString()
        {
            string result;

            if (this.Denominator == 1)
                result = this.Numerator.ToString();
            else
                result = this.Numerator.ToString() + "/" + this.Denominator.ToString();

            return result;
        }
    }
}

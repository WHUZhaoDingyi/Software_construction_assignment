using System;
using System.Windows.Forms;

namespace CalculatorApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 按钮点击事件：加法运算
        private void btnAdd_Click(object sender, EventArgs e)
        {
            double num1 = Convert.ToDouble(txtNum1.Text);
            double num2 = Convert.ToDouble(txtNum2.Text);
            double result = num1 + num2;
            lblResult.Text = "结果: " + result.ToString();
        }

        // 按钮点击事件：减法运算
        private void btnSubtract_Click(object sender, EventArgs e)
        {
            double num1 = Convert.ToDouble(txtNum1.Text);
            double num2 = Convert.ToDouble(txtNum2.Text);
            double result = num1 - num2;
            lblResult.Text = "结果: " + result.ToString();
        }

        // 按钮点击事件：乘法运算
        private void btnMultiply_Click(object sender, EventArgs e)
        {
            double num1 = Convert.ToDouble(txtNum1.Text);
            double num2 = Convert.ToDouble(txtNum2.Text);
            double result = num1 * num2;
            lblResult.Text = "结果: " + result.ToString();
        }

        // 按钮点击事件：除法运算
        private void btnDivide_Click(object sender, EventArgs e)
        {
            double num1 = Convert.ToDouble(txtNum1.Text);
            double num2 = Convert.ToDouble(txtNum2.Text);
            if (num2 != 0)
            {
                double result = num1 / num2;
                lblResult.Text = "结果: " + result.ToString();
            }
            else
            {
                lblResult.Text = "错误: 除数不能为零";
            }
        }
    }
}

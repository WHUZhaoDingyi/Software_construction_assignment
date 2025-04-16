using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OrderManagement.Domain;
using OrderManagement.Service;

namespace OrderManagement.WinFormsApp
{
    public partial class AddOrderForm : Form
    {
        private OrderService _service;

        public AddOrderForm(OrderService service)
        {
            InitializeComponent();
            _service = service;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var order = new Order
            {
                OrderNumber = textBox1.Text,
                Customer = textBox2.Text,
                OrderDetails = new List<OrderDetails>
                {
                    new OrderDetails { ProductName = textBox3.Text, Amount = decimal.Parse(textBox4.Text) }
                }
            };
            try
            {
                _service.AddOrder(order);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}    
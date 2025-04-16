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
    public partial class EditOrderForm : Form
    {
        private OrderService _service;
        private Order _order;

        public EditOrderForm(OrderService service, Order order)
        {
            InitializeComponent();
            _service = service;
            _order = order;
            textBox1.Text = _order.OrderNumber;
            textBox2.Text = _order.Customer;
            if (_order.OrderDetails.Count > 0)
            {
                textBox3.Text = _order.OrderDetails[0].ProductName;
                textBox4.Text = _order.OrderDetails[0].Amount.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _order.OrderNumber = textBox1.Text;
            _order.Customer = textBox2.Text;
            if (_order.OrderDetails.Count > 0)
            {
                _order.OrderDetails[0].ProductName = textBox3.Text;
                _order.OrderDetails[0].Amount = decimal.Parse(textBox4.Text);
            }
            try
            {
                _service.UpdateOrder(_order);
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
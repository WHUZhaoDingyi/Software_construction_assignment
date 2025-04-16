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
    public partial class Form1 : Form
    {
        private OrderService _service;
        private BindingSource _orderBindingSource;
        private BindingSource _orderDetailsBindingSource;

        public Form1()
        {
            InitializeComponent();
            using (var context = new OrderDbContext())
            {
                _service = new OrderService(context);
            }
            _orderBindingSource = new BindingSource();
            _orderDetailsBindingSource = new BindingSource();
            _orderDetailsBindingSource.DataSource = _orderBindingSource;
            _orderDetailsBindingSource.DataMember = "OrderDetails";
            dataGridView1.DataSource = _orderBindingSource;
            dataGridView2.DataSource = _orderDetailsBindingSource;
            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = _service.SortOrders();
            _orderBindingSource.DataSource = orders;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var addForm = new AddOrderForm(_service);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadOrders();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var order = (Order)dataGridView1.SelectedRows[0].DataBoundItem;
                try
                {
                    _service.DeleteOrder(order.Id);
                    LoadOrders();
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var order = (Order)dataGridView1.SelectedRows[0].DataBoundItem;
                var editForm = new EditOrderForm(_service, order);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadOrders();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var keyword = textBox1.Text;
            var orders = _service.QueryOrders(o => o.OrderNumber.Contains(keyword) || o.Customer.Contains(keyword));
            _orderBindingSource.DataSource = orders;
        }
    }
}    
using System;
using System.Windows.Forms;
using OrderCore;
using System.Collections.Generic;

namespace OrderWinForm
{
    public partial class MainForm : Form
    {
        private readonly OrderService orderService = new OrderService();
        private BindingSource orderBS = new BindingSource();
        private BindingSource detailBS = new BindingSource();

        public MainForm()
        {
            InitializeComponent();
            InitializeDataBinding();
            LoadSampleData();
        }

        private void InitializeDataBinding()
        {
            // 主表绑定
            orderBS.DataSource = new List<Order>();
            dgvOrders.DataSource = orderBS;
            dgvOrders.AutoGenerateColumns = false;

            // 明细表绑定
            detailBS.DataMember = "Details";
            detailBS.DataSource = orderBS;
            dgvDetails.DataSource = detailBS;
            dgvDetails.AutoGenerateColumns = false;

            // 配置列
            dgvOrders.Columns.AddRange(
                new DataGridViewTextBoxColumn { DataPropertyName = "OrderId", HeaderText = "订单号" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Customer", HeaderText = "客户" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "总金额" }
            );

            dgvDetails.Columns.AddRange(
                new DataGridViewTextBoxColumn { DataPropertyName = "Item.Name", HeaderText = "商品" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "数量" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "小计" }
            );

            dgvDetails.CellFormatting += DgvDetails_CellFormatting;
            dgvDetails.DataError += (s, e) => e.Cancel = true;
        }

        
        private void DgvDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvDetails.Columns[e.ColumnIndex].DataPropertyName == "Item.Name" && e.Value is Goods goods)
            {
                e.Value = goods.Name;
                e.FormattingApplied = true;
            }
        }


        private void LoadSampleData()
        {
            var goods = new Goods { Name = "手机", Price = 2999 };
            var order = new Order("2023001") { Customer = "张三" };
            order.Details.Add(new OrderDetails { Item = goods, Quantity = 1 });
            orderService.AddOrder(order);
            RefreshData();
        }

        private void RefreshData() => orderBS.DataSource = orderService.QueryOrders().ToList();

        // 按钮点击事件
        private void BtnAdd_Click(object sender, EventArgs e)
        {
        // 创建新订单时必须初始化Customer
            var newOrder = new Order("") { Customer = "新客户" }; 
            using var editForm = new EditOrderForm(newOrder);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                orderService.AddOrder(editForm.CurrentOrder);
                RefreshData();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (orderBS.Current is Order order) EditOrder(order.Clone());
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (orderBS.Current is Order order)
            {
                orderService.RemoveOrder(order.OrderId);
                RefreshData();
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var keyword = Microsoft.VisualBasic.Interaction.InputBox("输入查询关键词");
            orderBS.DataSource = orderService.QueryOrders(o => 
                o.OrderId.Contains(keyword) || 
                o.Customer.Contains(keyword) || 
                o.Details.Exists(d => d.Item.Name.Contains(keyword))
            ).ToList();
        }

        private void EditOrder(Order order)
        {
            using var form = new EditOrderForm(order);
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(order.OrderId)) 
                    orderService.AddOrder(form.CurrentOrder);
                else 
                    orderService.UpdateOrder(form.CurrentOrder);
                RefreshData();
            }
        }
    }
}
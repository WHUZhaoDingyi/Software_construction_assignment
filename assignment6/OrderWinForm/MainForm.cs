using System;
using System.Windows.Forms;
using OrderCore;
using System.Collections.Generic;
using System.Linq;

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

            // 清除已有列并配置新列
            dgvOrders.Columns.Clear();
            dgvDetails.Columns.Clear();
            
            // 配置订单列
            dgvOrders.Columns.AddRange(
                new DataGridViewTextBoxColumn { DataPropertyName = "OrderId", HeaderText = "订单号", ReadOnly = true },
                new DataGridViewTextBoxColumn { DataPropertyName = "Customer", HeaderText = "客户", ReadOnly = true },
                new DataGridViewTextBoxColumn { DataPropertyName = "CreateTime", HeaderText = "创建时间", ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm:ss" } },
                new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "总金额", ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "¥#,##0.00" } }
            );

            // 配置明细列
            dgvDetails.Columns.AddRange(
                new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "序号", Width = 50, ReadOnly = true },
                new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "商品", Width = 120, ReadOnly = true },
                new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "数量", Width = 60, ReadOnly = true },
                new DataGridViewTextBoxColumn { DataPropertyName = "ItemPrice", HeaderText = "单价", Width = 80, ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "¥#,##0.00" } },
                new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "小计", Width = 100, ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "¥#,##0.00" } }
            );

            // 添加错误处理器
            dgvDetails.CellFormatting += DgvDetails_CellFormatting;
            dgvDetails.DataError += DgvDetails_DataError;
            dgvOrders.DataError += DgvOrders_DataError;
            
            // 选择订单后自动显示明细
            dgvOrders.SelectionChanged += (s, e) => detailBS.ResetBindings(false);
        }

        private void DgvOrders_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
            Console.WriteLine($"订单表错误: {e.Exception.Message}");
        }

        private void DgvDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
            Console.WriteLine($"明细表错误: {e.Exception.Message}");
        }
        
        private void DgvDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var column = dgvDetails.Columns[e.ColumnIndex];
                if (column.DataPropertyName == "ItemName")
                {
                    try
                    {
                        var row = dgvDetails.Rows[e.RowIndex];
                        var detail = row.DataBoundItem as OrderDetails;
                        if (detail?.Item != null)
                        {
                            e.Value = detail.Item.Name;
                            e.FormattingApplied = true;
                        }
                        else if (!string.IsNullOrEmpty(detail?.ItemName))
                        {
                            e.Value = detail.ItemName;
                            e.FormattingApplied = true;
                        }
                        else
                        {
                            e.Value = "未选择商品";
                            e.FormattingApplied = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"格式化错误: {ex.Message}");
                        e.Value = "格式错误";
                        e.FormattingApplied = true;
                    }
                }
            }
        }

        private void LoadSampleData()
        {
            try
            {
                // 创建样例商品
                var phone = new Goods { Id = "P001", Name = "手机", Price = 4999 };
                var laptop = new Goods { Id = "P002", Name = "笔记本电脑", Price = 6999 };
                var headset = new Goods { Id = "P003", Name = "耳机", Price = 899 };
                
                // 创建样例订单
                var order1 = new Order("ORD2023001") { Customer = "张三" };
                order1.AddDetails(new OrderDetails { Item = phone, Quantity = 1 });
                order1.AddDetails(new OrderDetails { Item = headset, Quantity = 1 });
                
                var order2 = new Order("ORD2023002") { Customer = "李四" };
                order2.AddDetails(new OrderDetails { Item = laptop, Quantity = 1 });
                
                // 添加样例订单
                orderService.AddOrder(order1);
                orderService.AddOrder(order2);
                
                RefreshData();
                
                MessageBox.Show("已加载示例数据（2个订单）", "系统提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载样例数据失败: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshData() => orderBS.DataSource = orderService.QueryOrders().ToList();

        // 按钮点击事件
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // 生成订单号
                string newOrderId = $"ORD{DateTime.Now:yyyyMMddHHmmss}";
                var newOrder = new Order(newOrderId) { Customer = "新客户" };
                
                using var editForm = new EditOrderForm(newOrder);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    try 
                    {
                        orderService.AddOrder(editForm.CurrentOrder);
                        RefreshData();
                        MessageBox.Show($"订单 {newOrderId} 创建成功！", "操作成功", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"添加订单失败: {ex.Message}", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建订单时发生错误: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (orderBS.Current is Order order)
            {
                try
                {
                    EditOrder(order.Clone());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"编辑订单时发生错误: {ex.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("请先选择一个订单", "提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (orderBS.Current is Order order)
            {
                try
                {
                    var result = MessageBox.Show($"确定要删除订单 {order.OrderId} 吗？", "确认删除", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                    if (result == DialogResult.Yes)
                    {
                        orderService.RemoveOrder(order.OrderId);
                        RefreshData();
                        MessageBox.Show("订单删除成功！", "操作成功", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除订单失败: {ex.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("请先选择一个订单", "提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var keyword = Microsoft.VisualBasic.Interaction.InputBox("请输入查询关键词（订单号/客户名/商品名）:", "查询订单");
                
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var results = orderService.QueryOrders(o => 
                        o.OrderId.Contains(keyword) || 
                        o.Customer.Contains(keyword) || 
                        o.Details.Exists(d => 
                            (d.Item?.Name?.Contains(keyword) ?? false) ||
                            (d.ItemName?.Contains(keyword) ?? false)
                        )
                    ).ToList();
                    
                    orderBS.DataSource = results;
                    
                    if (results.Count > 0)
                    {
                        MessageBox.Show($"共找到 {results.Count} 个匹配的订单", "查询结果", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("没有找到匹配的订单", "查询结果", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"查询订单时发生错误: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditOrder(Order order)
        {
            try
            {
                using var form = new EditOrderForm(order);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    orderService.UpdateOrder(form.CurrentOrder);
                    RefreshData();
                    MessageBox.Show($"订单 {order.OrderId} 修改成功！", "操作成功", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"修改订单失败: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
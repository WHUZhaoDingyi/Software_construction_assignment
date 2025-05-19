using System;
using System.Windows.Forms;
using OrderCore;
using System.Collections.Generic;
using System.Linq;

namespace OrderWinForm
{
    public partial class EditOrderForm : Form
    {
        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Order CurrentOrder { get; private set; }
        private BindingSource detailsBS = new BindingSource();
        private List<Goods> availableGoods = new List<Goods>();

        public EditOrderForm(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "订单不能为空");
                
            CurrentOrder = order;
            InitializeComponent();
            InitializeDataBinding();
            LoadGoodsData();
        }

        private void InitializeDataBinding()
        {
            // 禁用订单号编辑，如果已有订单号
            if (!string.IsNullOrEmpty(CurrentOrder.OrderId))
                txtOrderId.ReadOnly = true;
                
            // 基础信息绑定
            txtOrderId.DataBindings.Add("Text", CurrentOrder, "OrderId");
            txtCustomer.DataBindings.Add("Text", CurrentOrder, "Customer");

            // 明细绑定
            detailsBS.DataSource = CurrentOrder.Details;
            dgvDetails.DataSource = detailsBS;
            dgvDetails.AutoGenerateColumns = false;

            // 配置列
            dgvDetails.Columns.Clear();
            dgvDetails.Columns.AddRange(
                new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "序号", Width = 50, ReadOnly = true },
                new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "商品", Width = 120, ReadOnly = true },
                new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "数量", Width = 60, ReadOnly = true },
                new DataGridViewTextBoxColumn { DataPropertyName = "ItemPrice", HeaderText = "单价", Width = 80, ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "¥#,##0.00" } },
                new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "小计", Width = 80, ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "¥#,##0.00" } }
            );

            // 添加事件处理
            dgvDetails.CellFormatting += DgvDetails_CellFormatting;
            dgvDetails.DataError += DgvDetails_DataError;

            // 设置数量输入为数字
            txtQuantity.Text = "1";
        }

        private void LoadGoodsData()
        {
            try
            {
                // 添加一些示例商品
                availableGoods = new List<Goods>
                {
                    new Goods { Id = "P001", Name = "手机", Price = 4999 },
                    new Goods { Id = "P002", Name = "笔记本电脑", Price = 6999 },
                    new Goods { Id = "P003", Name = "耳机", Price = 899 },
                    new Goods { Id = "P004", Name = "平板电脑", Price = 3999 },
                    new Goods { Id = "P005", Name = "智能手表", Price = 1999 },
                };
                
                // 商品选择
                cmbGoods.DisplayMember = "Name";
                cmbGoods.ValueMember = "Id";
                cmbGoods.DataSource = availableGoods;
                
                // 添加商品选择变更事件
                cmbGoods.SelectedIndexChanged += CmbGoods_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载商品数据时发生错误: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbGoods_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 显示选中商品的价格信息
            if (cmbGoods.SelectedItem is Goods selectedGoods)
            {
                // 在这里可以显示商品的其他信息，比如价格
                // 例如：lblPrice.Text = $"¥{selectedGoods.Price:N2}";
            }
        }

        private void DgvDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

        private void DgvDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dgvDetails.Columns[e.ColumnIndex].DataPropertyName == "ItemName")
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

        private void BtnAddDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbGoods.SelectedItem is Goods goods)
                {
                    if (!int.TryParse(txtQuantity.Text, out int qty) || qty <= 0)
                    {
                        MessageBox.Show("请输入有效的商品数量", "输入错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    try
                    {
                        CurrentOrder.AddDetails(new OrderDetails { Item = goods, Quantity = qty });
                        detailsBS.ResetBindings(false);
                        // 重置数量为1
                        txtQuantity.Text = "1";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"添加明细失败: {ex.Message}", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("请选择一个商品", "提示", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加明细时发生错误: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证订单信息
                if (string.IsNullOrWhiteSpace(CurrentOrder.OrderId))
                {
                    MessageBox.Show("订单号不能为空", "验证错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOrderId.Focus();
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(CurrentOrder.Customer))
                {
                    MessageBox.Show("客户名称不能为空", "验证错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomer.Focus();
                    return;
                }
                
                if (!CurrentOrder.Details.Any())
                {
                    MessageBox.Show("订单必须包含至少一个商品", "验证错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存订单时发生错误: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDetails.CurrentRow?.DataBoundItem is OrderDetails detail)
                {
                    var result = MessageBox.Show($"确定要删除\"{detail.ItemName}\"这个明细吗？", 
                        "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                    if (result == DialogResult.Yes)
                    {
                        CurrentOrder.Details.Remove(detail);
                        detailsBS.ResetBindings(false);
                    }
                }
                else
                {
                    MessageBox.Show("请先选择一个订单明细", "提示", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除明细时发生错误: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
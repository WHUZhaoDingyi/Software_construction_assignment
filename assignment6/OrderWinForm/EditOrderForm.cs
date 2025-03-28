using System.Windows.Forms;
using OrderCore;

namespace OrderWinForm
{
    public partial class EditOrderForm : Form
    {
        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Order CurrentOrder { get; private set; }
        private BindingSource detailsBS = new BindingSource();

        public EditOrderForm(Order order)
        {
            CurrentOrder = order;
            InitializeComponent();
            InitializeDataBinding();
        }

        private void InitializeDataBinding()
        {
            // 基础信息绑定
            txtOrderId.DataBindings.Add("Text", CurrentOrder, "OrderId");
            txtCustomer.DataBindings.Add("Text", CurrentOrder, "Customer");

            // 明细绑定
            detailsBS.DataSource = CurrentOrder.Details;
            dgvDetails.DataSource = detailsBS;
            dgvDetails.AutoGenerateColumns = false;

            // 商品选择
            cmbGoods.DisplayMember = "Name";
            cmbGoods.DataSource = new List<Goods>
            {
                new Goods { Name = "手机", Price = 2999 },
                new Goods { Name = "耳机", Price = 399 }
            };
        }

        private void BtnAddDetail_Click(object sender, EventArgs e)
        {
            if (cmbGoods.SelectedItem is Goods goods && 
                int.TryParse(txtQuantity.Text, out int qty) && qty > 0)
            {
                CurrentOrder.Details.Add(new OrderDetails { Item = goods, Quantity = qty });
                detailsBS.ResetBindings(false);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentOrder.OrderId))
            {
                MessageBox.Show("订单号不能为空");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
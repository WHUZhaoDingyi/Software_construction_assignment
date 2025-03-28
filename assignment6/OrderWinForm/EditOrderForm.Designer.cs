namespace OrderWinForm
{
    partial class EditOrderForm
    {
        private TextBox txtOrderId;
        private TextBox txtCustomer;
        private DataGridView dgvDetails;
        private ComboBox cmbGoods;
        private TextBox txtQuantity;
        private Button btnAddDetail;
        private Button btnSave;

        private void InitializeComponent()
        {
            this.txtOrderId = new TextBox();
            this.txtCustomer = new TextBox();
            this.dgvDetails = new DataGridView();
            this.cmbGoods = new ComboBox();
            this.txtQuantity = new TextBox();
            this.btnAddDetail = new Button();
            this.btnSave = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).BeginInit();
            this.SuspendLayout();

            // 布局参数
            int margin = 10;
            int yPos = 10;

            // txtOrderId
            txtOrderId.Location = new System.Drawing.Point(margin, yPos);
            txtOrderId.Width = 200;
            txtOrderId.PlaceholderText = "订单号";

            // txtCustomer
            yPos += 40;
            txtCustomer.Location = new System.Drawing.Point(margin, yPos);
            txtCustomer.Width = 200;
            txtCustomer.PlaceholderText = "客户";

            // 商品选择区域
            yPos += 40;
            cmbGoods.Location = new System.Drawing.Point(margin, yPos);
            cmbGoods.Width = 120;
            txtQuantity.Location = new System.Drawing.Point(margin + 130, yPos);
            txtQuantity.Width = 50;
            btnAddDetail.Location = new System.Drawing.Point(margin + 190, yPos);
            btnAddDetail.Text = "添加";

            // DataGridView
            dgvDetails.Top = yPos + 40;
            dgvDetails.Height = 200;
            dgvDetails.Width = 360;
            dgvDetails.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // 保存按钮
            btnSave.Dock = DockStyle.Bottom;
            btnSave.Height = 40;
            btnSave.Text = "保存";

            // 窗体设置
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.AddRange(new Control[] {
                txtOrderId, txtCustomer, cmbGoods, txtQuantity,
                btnAddDetail, dgvDetails, btnSave
            });
            this.Text = "编辑订单";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).EndInit();
            this.ResumeLayout(true);
        }
    }
}
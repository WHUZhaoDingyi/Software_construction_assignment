using System.Windows.Forms;

namespace OrderWinForm
{
    partial class EditOrderForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupBoxBase;
        private System.Windows.Forms.GroupBox groupBoxDetails;
        private System.Windows.Forms.Button btnDeleteDetail;
        private System.Windows.Forms.TextBox txtOrderId;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.DataGridView dgvDetails;
        private System.Windows.Forms.ComboBox cmbGoods;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Button btnAddDetail;
        private System.Windows.Forms.Button btnSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtOrderId = new System.Windows.Forms.TextBox();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.dgvDetails = new System.Windows.Forms.DataGridView();
            this.cmbGoods = new System.Windows.Forms.ComboBox();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.btnAddDetail = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBoxBase = new System.Windows.Forms.GroupBox();
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.btnDeleteDetail = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).BeginInit();
            this.SuspendLayout();

            // groupBoxBase
            this.groupBoxBase.Text = "订单信息";
            this.groupBoxBase.SetBounds(10, 10, 380, 80);
            this.groupBoxBase.Controls.AddRange(new System.Windows.Forms.Control[] { txtOrderId, txtCustomer });

            // txtOrderId
            txtOrderId.Location = new System.Drawing.Point(20, 30);
            txtOrderId.Width = 150;
            txtOrderId.PlaceholderText = "订单号";

            // txtCustomer
            txtCustomer.Location = new System.Drawing.Point(200, 30);
            txtCustomer.Width = 150;
            txtCustomer.PlaceholderText = "客户";

            // groupBoxDetails
            this.groupBoxDetails.Text = "订单明细";
            this.groupBoxDetails.SetBounds(10, 100, 380, 220);
            this.groupBoxDetails.Controls.AddRange(new System.Windows.Forms.Control[] { cmbGoods, txtQuantity, btnAddDetail, dgvDetails, btnDeleteDetail });

            // 商品选择区域
            cmbGoods.Location = new System.Drawing.Point(20, 30);
            cmbGoods.Width = 120;
            txtQuantity.Location = new System.Drawing.Point(150, 30);
            txtQuantity.Width = 50;
            btnAddDetail.Location = new System.Drawing.Point(210, 28);
            btnAddDetail.Text = "添加";
            btnAddDetail.BackColor = System.Drawing.Color.LightGreen;
            btnAddDetail.Height = 28;
            btnAddDetail.Width = 60;
            btnAddDetail.Click += new System.EventHandler(this.BtnAddDetail_Click);

            // DataGridView
            dgvDetails.Location = new System.Drawing.Point(20, 65);
            dgvDetails.Height = 110;
            dgvDetails.Width = 340;
            dgvDetails.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            // 删除明细按钮
            btnDeleteDetail.Text = "删除明细";
            btnDeleteDetail.Location = new System.Drawing.Point(20, 180);
            btnDeleteDetail.BackColor = System.Drawing.Color.LightCoral;
            btnDeleteDetail.Height = 28;
            btnDeleteDetail.Width = 80;
            btnDeleteDetail.Click += new System.EventHandler(this.BtnDeleteDetail_Click);

            // 保存按钮
            btnSave.Dock = System.Windows.Forms.DockStyle.Bottom;
            btnSave.Height = 40;
            btnSave.Text = "保存";
            btnSave.BackColor = System.Drawing.Color.LightBlue;
            btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            // 窗体设置
            this.ClientSize = new System.Drawing.Size(400, 370);
            this.Controls.Clear();
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                groupBoxBase, groupBoxDetails, btnSave
            });
            this.Text = "编辑订单";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).EndInit();
            this.ResumeLayout(true);
        }
    }
}
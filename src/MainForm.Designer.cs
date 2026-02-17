namespace TurekSimulator
{
	partial class MainForm
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.lblDay = new System.Windows.Forms.Label();
			this.lblCash = new System.Windows.Forms.Label();
			this.lblReputation = new System.Windows.Forms.Label();
			this.dgvInventory = new System.Windows.Forms.DataGridView();
			this.btnBuyBread = new System.Windows.Forms.Button();
			this.btnBuyMeat = new System.Windows.Forms.Button();
			this.btnBuyVeggies = new System.Windows.Forms.Button();
			this.btnBuySauce = new System.Windows.Forms.Button();
			this.btnServeSelected = new System.Windows.Forms.Button();
			this.btnEndDay = new System.Windows.Forms.Button();
			this.dgvCustomers = new System.Windows.Forms.DataGridView();
			this.pnlLog = new System.Windows.Forms.Panel();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.pnlCustomers = new System.Windows.Forms.Panel();
			this.pnlInventory = new System.Windows.Forms.Panel();
			this.pnlIngredients = new System.Windows.Forms.Panel();
			this.pnlStats = new System.Windows.Forms.Panel();
			this.pnlQueueView = new System.Windows.Forms.Panel();
			this.pnlControls = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
			this.pnlLog.SuspendLayout();
			this.pnlCustomers.SuspendLayout();
			this.pnlInventory.SuspendLayout();
			this.pnlIngredients.SuspendLayout();
			this.pnlStats.SuspendLayout();
			this.pnlControls.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblDay
			// 
			this.lblDay.AutoSize = true;
			this.lblDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
			this.lblDay.Location = new System.Drawing.Point(89, 36);
			this.lblDay.Name = "lblDay";
			this.lblDay.Size = new System.Drawing.Size(73, 25);
			this.lblDay.TabIndex = 12;
			this.lblDay.Text = "Dzień:";
			// 
			// lblCash
			// 
			this.lblCash.AutoSize = true;
			this.lblCash.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
			this.lblCash.Location = new System.Drawing.Point(758, 36);
			this.lblCash.Name = "lblCash";
			this.lblCash.Size = new System.Drawing.Size(67, 25);
			this.lblCash.TabIndex = 11;
			this.lblCash.Text = "Kasa:";
			// 
			// lblReputation
			// 
			this.lblReputation.AutoSize = true;
			this.lblReputation.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
			this.lblReputation.Location = new System.Drawing.Point(401, 36);
			this.lblReputation.Name = "lblReputation";
			this.lblReputation.Size = new System.Drawing.Size(115, 25);
			this.lblReputation.TabIndex = 10;
			this.lblReputation.Text = "Reputacja:";
			// 
			// dgvInventory
			// 
			this.dgvInventory.AllowUserToAddRows = false;
			this.dgvInventory.AllowUserToDeleteRows = false;
			this.dgvInventory.ColumnHeadersHeight = 29;
			this.dgvInventory.Location = new System.Drawing.Point(8, 10);
			this.dgvInventory.Name = "dgvInventory";
			this.dgvInventory.ReadOnly = true;
			this.dgvInventory.RowHeadersWidth = 51;
			this.dgvInventory.Size = new System.Drawing.Size(252, 137);
			this.dgvInventory.TabIndex = 9;
			this.dgvInventory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvInventory_CellContentClick);
			// 
			// btnBuyBread
			// 
			this.btnBuyBread.Location = new System.Drawing.Point(145, 53);
			this.btnBuyBread.Name = "btnBuyBread";
			this.btnBuyBread.Size = new System.Drawing.Size(80, 30);
			this.btnBuyBread.TabIndex = 8;
			this.btnBuyBread.Text = "Buła";
			this.btnBuyBread.Click += new System.EventHandler(this.btnBuyBread_Click);
			// 
			// btnBuyMeat
			// 
			this.btnBuyMeat.Location = new System.Drawing.Point(350, 53);
			this.btnBuyMeat.Name = "btnBuyMeat";
			this.btnBuyMeat.Size = new System.Drawing.Size(80, 30);
			this.btnBuyMeat.TabIndex = 7;
			this.btnBuyMeat.Text = "Mięso";
			this.btnBuyMeat.Click += new System.EventHandler(this.btnBuyMeat_Click);
			// 
			// btnBuyVeggies
			// 
			this.btnBuyVeggies.Location = new System.Drawing.Point(821, 53);
			this.btnBuyVeggies.Name = "btnBuyVeggies";
			this.btnBuyVeggies.Size = new System.Drawing.Size(106, 30);
			this.btnBuyVeggies.TabIndex = 6;
			this.btnBuyVeggies.Text = "Warzywa";
			this.btnBuyVeggies.Click += new System.EventHandler(this.btnBuyVeggies_Click);
			// 
			// btnBuySauce
			// 
			this.btnBuySauce.Location = new System.Drawing.Point(629, 53);
			this.btnBuySauce.Name = "btnBuySauce";
			this.btnBuySauce.Size = new System.Drawing.Size(80, 30);
			this.btnBuySauce.TabIndex = 5;
			this.btnBuySauce.Text = "Sosiwo";
			this.btnBuySauce.Click += new System.EventHandler(this.btnBuySauce_Click);
			// 
			// btnServeSelected
			// 
			this.btnServeSelected.Location = new System.Drawing.Point(10, 10);
			this.btnServeSelected.Name = "btnServeSelected";
			this.btnServeSelected.Size = new System.Drawing.Size(150, 50);
			this.btnServeSelected.TabIndex = 4;
			this.btnServeSelected.Text = "Obsłuż wybranego klienta";
			this.btnServeSelected.Click += new System.EventHandler(this.btnServeSelected_Click);
			// 
			// btnEndDay
			// 
			this.btnEndDay.Location = new System.Drawing.Point(10, 70);
			this.btnEndDay.Name = "btnEndDay";
			this.btnEndDay.Size = new System.Drawing.Size(150, 50);
			this.btnEndDay.TabIndex = 2;
			this.btnEndDay.Text = "Zakończ dzień";
			this.btnEndDay.Click += new System.EventHandler(this.btnEndDay_Click);
			// 
			// dgvCustomers
			// 
			this.dgvCustomers.ColumnHeadersHeight = 29;
			this.dgvCustomers.Location = new System.Drawing.Point(8, 10);
			this.dgvCustomers.Name = "dgvCustomers";
			this.dgvCustomers.RowHeadersWidth = 51;
			this.dgvCustomers.Size = new System.Drawing.Size(252, 362);
			this.dgvCustomers.TabIndex = 0;
			// 
			// pnlLog
			// 
			this.pnlLog.BackColor = System.Drawing.Color.Transparent;
			this.pnlLog.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pnlLog.Controls.Add(this.txtLog);
			this.pnlLog.Location = new System.Drawing.Point(1003, 0);
			this.pnlLog.Name = "pnlLog";
			this.pnlLog.Size = new System.Drawing.Size(260, 96);
			this.pnlLog.TabIndex = 13;
			this.pnlLog.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlLog_Paint);
			// 
			// txtLog
			// 
			this.txtLog.Location = new System.Drawing.Point(8, 11);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtLog.Size = new System.Drawing.Size(251, 76);
			this.txtLog.TabIndex = 2;
			// 
			// pnlCustomers
			// 
			this.pnlCustomers.BackColor = System.Drawing.Color.Transparent;
			this.pnlCustomers.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pnlCustomers.Controls.Add(this.dgvCustomers);
			this.pnlCustomers.Location = new System.Drawing.Point(1003, 111);
			this.pnlCustomers.Name = "pnlCustomers";
			this.pnlCustomers.Size = new System.Drawing.Size(260, 383);
			this.pnlCustomers.TabIndex = 14;
			// 
			// pnlInventory
			// 
			this.pnlInventory.BackColor = System.Drawing.Color.Transparent;
			this.pnlInventory.Controls.Add(this.dgvInventory);
			this.pnlInventory.Location = new System.Drawing.Point(1003, 533);
			this.pnlInventory.Name = "pnlInventory";
			this.pnlInventory.Size = new System.Drawing.Size(260, 147);
			this.pnlInventory.TabIndex = 15;
			// 
			// pnlIngredients
			// 
			this.pnlIngredients.BackColor = System.Drawing.Color.Transparent;
			this.pnlIngredients.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pnlIngredients.Controls.Add(this.btnBuySauce);
			this.pnlIngredients.Controls.Add(this.btnBuyVeggies);
			this.pnlIngredients.Controls.Add(this.btnBuyMeat);
			this.pnlIngredients.Controls.Add(this.btnBuyBread);
			this.pnlIngredients.Location = new System.Drawing.Point(1, 533);
			this.pnlIngredients.Name = "pnlIngredients";
			this.pnlIngredients.Size = new System.Drawing.Size(1002, 147);
			this.pnlIngredients.TabIndex = 16;
			// 
			// pnlStats
			// 
			this.pnlStats.BackColor = System.Drawing.Color.Transparent;
			this.pnlStats.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pnlStats.Controls.Add(this.lblReputation);
			this.pnlStats.Controls.Add(this.lblDay);
			this.pnlStats.Controls.Add(this.lblCash);
			this.pnlStats.Location = new System.Drawing.Point(1, 0);
			this.pnlStats.Name = "pnlStats";
			this.pnlStats.Size = new System.Drawing.Size(1002, 96);
			this.pnlStats.TabIndex = 17;
			// 
			// pnlQueueView
			// 
			this.pnlQueueView.BackColor = System.Drawing.Color.Black;
			this.pnlQueueView.BackgroundImage = global::TurekSimulator.Properties.Resources.PnlControls;
			this.pnlQueueView.Location = new System.Drawing.Point(55, 121);
			this.pnlQueueView.Name = "pnlQueueView";
			this.pnlQueueView.Size = new System.Drawing.Size(751, 363);
			this.pnlQueueView.TabIndex = 19;
			this.pnlQueueView.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// pnlControls
			// 
			this.pnlControls.BackColor = System.Drawing.Color.Transparent;
			this.pnlControls.Controls.Add(this.btnServeSelected);
			this.pnlControls.Controls.Add(this.btnEndDay);
			this.pnlControls.Location = new System.Drawing.Point(812, 356);
			this.pnlControls.Name = "pnlControls";
			this.pnlControls.Size = new System.Drawing.Size(170, 128);
			this.pnlControls.TabIndex = 18;
			this.pnlControls.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlControls_Paint);
			// 
			// MainForm
			// 
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.BackgroundImage = global::TurekSimulator.Properties.Resources.Background;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(1264, 681);
			this.Controls.Add(this.pnlControls);
			this.Controls.Add(this.pnlQueueView);
			this.Controls.Add(this.pnlStats);
			this.Controls.Add(this.pnlIngredients);
			this.Controls.Add(this.pnlInventory);
			this.Controls.Add(this.pnlCustomers);
			this.Controls.Add(this.pnlLog);
			this.DoubleBuffered = true;
			this.MaximumSize = new System.Drawing.Size(1280, 720);
			this.MinimumSize = new System.Drawing.Size(1280, 720);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Turek Simulator";
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
			this.pnlLog.ResumeLayout(false);
			this.pnlLog.PerformLayout();
			this.pnlCustomers.ResumeLayout(false);
			this.pnlInventory.ResumeLayout(false);
			this.pnlIngredients.ResumeLayout(false);
			this.pnlStats.ResumeLayout(false);
			this.pnlStats.PerformLayout();
			this.pnlControls.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblDay;
		private System.Windows.Forms.Label lblCash;
		private System.Windows.Forms.Label lblReputation;
		private System.Windows.Forms.DataGridView dgvInventory;
		private System.Windows.Forms.Button btnBuyBread;
		private System.Windows.Forms.Button btnBuyMeat;
		private System.Windows.Forms.Button btnBuyVeggies;
		private System.Windows.Forms.Button btnBuySauce;
		private System.Windows.Forms.Button btnServeSelected;
		private System.Windows.Forms.Button btnEndDay;
		private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.Panel pnlLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel pnlCustomers;
        private System.Windows.Forms.Panel pnlInventory;
        private System.Windows.Forms.Panel pnlIngredients;
        private System.Windows.Forms.Panel pnlStats;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Panel pnlQueueView;
    }
}

namespace WorkNet
{
    partial class FormWokers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWokers));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbWokers = new System.Windows.Forms.ToolStripButton();
            this.tbGrade = new System.Windows.Forms.ToolStripButton();
            this.tbGroup = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbAppend = new System.Windows.Forms.ToolStripButton();
            this.tbModify = new System.Windows.Forms.ToolStripButton();
            this.tbDelete = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = global::WorkNet.Properties.Resources.fio;
            this.toolStrip1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbWokers,
            this.tbGrade,
            this.tbGroup,
            this.toolStripSeparator1,
            this.tbAppend,
            this.tbModify,
            this.tbDelete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(896, 39);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbWokers
            // 
            this.tbWokers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbWokers.Image = ((System.Drawing.Image)(resources.GetObject("tbWokers.Image")));
            this.tbWokers.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbWokers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbWokers.Name = "tbWokers";
            this.tbWokers.Size = new System.Drawing.Size(36, 36);
            this.tbWokers.Tag = "1";
            this.tbWokers.Text = "Сотрудники";
            this.tbWokers.Click += new System.EventHandler(this.tbWokers_Click);
            // 
            // tbGrade
            // 
            this.tbGrade.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbGrade.Image = ((System.Drawing.Image)(resources.GetObject("tbGrade.Image")));
            this.tbGrade.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbGrade.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbGrade.Name = "tbGrade";
            this.tbGrade.Size = new System.Drawing.Size(36, 36);
            this.tbGrade.Tag = "3";
            this.tbGrade.Text = "Тарифы";
            this.tbGrade.Click += new System.EventHandler(this.tbWokers_Click);
            // 
            // tbGroup
            // 
            this.tbGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbGroup.Image = ((System.Drawing.Image)(resources.GetObject("tbGroup.Image")));
            this.tbGroup.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbGroup.Name = "tbGroup";
            this.tbGroup.Size = new System.Drawing.Size(36, 36);
            this.tbGroup.Tag = "2";
            this.tbGroup.Text = "Группы";
            this.tbGroup.Click += new System.EventHandler(this.tbWokers_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(0, 7, 0, 7);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbAppend
            // 
            this.tbAppend.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAppend.Image = ((System.Drawing.Image)(resources.GetObject("tbAppend.Image")));
            this.tbAppend.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbAppend.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAppend.Name = "tbAppend";
            this.tbAppend.Size = new System.Drawing.Size(36, 36);
            this.tbAppend.Text = "Добавить";
            this.tbAppend.Click += new System.EventHandler(this.tbAppend_Click);
            // 
            // tbModify
            // 
            this.tbModify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbModify.Image = ((System.Drawing.Image)(resources.GetObject("tbModify.Image")));
            this.tbModify.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbModify.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbModify.Name = "tbModify";
            this.tbModify.Size = new System.Drawing.Size(36, 36);
            this.tbModify.Text = "Изменить";
            this.tbModify.Click += new System.EventHandler(this.tbModify_Click);
            // 
            // tbDelete
            // 
            this.tbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbDelete.Image = ((System.Drawing.Image)(resources.GetObject("tbDelete.Image")));
            this.tbDelete.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbDelete.Name = "tbDelete";
            this.tbDelete.Size = new System.Drawing.Size(36, 36);
            this.tbDelete.Text = "Удалить";
            this.tbDelete.Click += new System.EventHandler(this.tbDelete_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 39);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(896, 274);
            this.panel1.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(896, 274);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DoubleClick += new System.EventHandler(this.dataGridView1_DoubleClick);
            // 
            // FormWokers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 313);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormWokers";
            this.Text = "FormWokers";
            this.Load += new System.EventHandler(this.FormWokers_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWokers_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripButton tbWokers;
        private System.Windows.Forms.ToolStripButton tbGrade;
        private System.Windows.Forms.ToolStripButton tbGroup;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tbAppend;
        private System.Windows.Forms.ToolStripButton tbModify;
        private System.Windows.Forms.ToolStripButton tbDelete;
    }
}
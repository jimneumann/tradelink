namespace ASP
{
    partial class ASP
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ASP));
            this._availresponses = new System.Windows.Forms.ComboBox();
            this._resnames = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this._newrespbox = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._togglemsgs = new System.Windows.Forms.Button();
            this._twithelp = new System.Windows.Forms.Button();
            this._librarysel = new System.Windows.Forms.Button();
            this._opttog = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this._newrespbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _availresponses
            // 
            this._availresponses.FormattingEnabled = true;
            this._availresponses.Location = new System.Drawing.Point(7, 31);
            this._availresponses.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._availresponses.Name = "_availresponses";
            this._availresponses.Size = new System.Drawing.Size(362, 28);
            this._availresponses.TabIndex = 1;
            this._availresponses.Text = "Select a response";
            this.toolTip1.SetToolTip(this._availresponses, "List of responses available in current response library");
            this._availresponses.SelectedIndexChanged += new System.EventHandler(this.Boxes_SelectedIndexChanged);
            // 
            // _resnames
            // 
            this._resnames.FormattingEnabled = true;
            this._resnames.HorizontalScrollbar = true;
            this._resnames.ItemHeight = 20;
            this._resnames.Location = new System.Drawing.Point(13, 99);
            this._resnames.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._resnames.Name = "_resnames";
            this._resnames.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._resnames.Size = new System.Drawing.Size(434, 124);
            this._resnames.TabIndex = 4;
            this._resnames.TabStop = false;
            this.toolTip1.SetToolTip(this._resnames, "Active Responses");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 280);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(468, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // _newrespbox
            // 
            this._newrespbox.Controls.Add(this._librarysel);
            this._newrespbox.Controls.Add(this._availresponses);
            this._newrespbox.Location = new System.Drawing.Point(14, 12);
            this._newrespbox.Name = "_newrespbox";
            this._newrespbox.Size = new System.Drawing.Size(435, 79);
            this._newrespbox.TabIndex = 16;
            this._newrespbox.TabStop = false;
            this._newrespbox.Text = "Start new Response (strategy)";
            // 
            // _togglemsgs
            // 
            this._togglemsgs.Location = new System.Drawing.Point(13, 231);
            this._togglemsgs.Name = "_togglemsgs";
            this._togglemsgs.Size = new System.Drawing.Size(27, 29);
            this._togglemsgs.TabIndex = 19;
            this._togglemsgs.Text = "!";
            this._togglemsgs.UseVisualStyleBackColor = true;
            this._togglemsgs.Click += new System.EventHandler(this._togglemsgs_Click);
            // 
            // _twithelp
            // 
            this._twithelp.Image = ((System.Drawing.Image)(resources.GetObject("_twithelp.Image")));
            this._twithelp.Location = new System.Drawing.Point(418, 231);
            this._twithelp.Name = "_twithelp";
            this._twithelp.Size = new System.Drawing.Size(31, 29);
            this._twithelp.TabIndex = 20;
            this.toolTip1.SetToolTip(this._twithelp, "submit bug report");
            this._twithelp.UseVisualStyleBackColor = true;
            this._twithelp.Click += new System.EventHandler(this._twithelp_Click_1);
            // 
            // _librarysel
            // 
            this._librarysel.Location = new System.Drawing.Point(385, 31);
            this._librarysel.Name = "_librarysel";
            this._librarysel.Size = new System.Drawing.Size(32, 28);
            this._librarysel.TabIndex = 21;
            this._librarysel.Text = "L";
            this.toolTip1.SetToolTip(this._librarysel, "view responses available in different dll library");
            this._librarysel.UseVisualStyleBackColor = true;
            this._librarysel.Click += new System.EventHandler(this.LoadDLL_Click);
            // 
            // _opttog
            // 
            this._opttog.Location = new System.Drawing.Point(46, 231);
            this._opttog.Name = "_opttog";
            this._opttog.Size = new System.Drawing.Size(28, 29);
            this._opttog.TabIndex = 21;
            this._opttog.Text = "?";
            this.toolTip1.SetToolTip(this._opttog, "change ASP options");
            this._opttog.UseVisualStyleBackColor = true;
            this._opttog.Click += new System.EventHandler(this._opttog_Click);
            // 
            // ASP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 302);
            this.Controls.Add(this._opttog);
            this.Controls.Add(this._twithelp);
            this.Controls.Add(this._togglemsgs);
            this.Controls.Add(this._newrespbox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this._resnames);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ASP";
            this.Text = "ASP";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this._newrespbox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _availresponses;
        private System.Windows.Forms.ListBox _resnames;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.GroupBox _newrespbox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button _togglemsgs;
        private System.Windows.Forms.Button _twithelp;
        private System.Windows.Forms.Button _librarysel;
        private System.Windows.Forms.Button _opttog;
    }
}


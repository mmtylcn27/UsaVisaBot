
namespace UsaVisa
{
    partial class MainForm
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtMinDate = new System.Windows.Forms.DateTimePicker();
            this.dtMaxDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtScheduleId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtFacilityId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtProxyUserPw = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtProxyUserId = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtProxyPort = new System.Windows.Forms.TextBox();
            this.txtProxyIp = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtWaitMs = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtErrorLog = new System.Windows.Forms.TextBox();
            this.chReAppointment = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mail:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(9, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtMail
            // 
            this.txtMail.Location = new System.Drawing.Point(107, 12);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(195, 22);
            this.txtMail.TabIndex = 2;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(107, 37);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(195, 22);
            this.txtPassword.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(9, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 22);
            this.label3.TabIndex = 4;
            this.label3.Text = "Min. Date:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtMinDate
            // 
            this.dtMinDate.Location = new System.Drawing.Point(107, 119);
            this.dtMinDate.Name = "dtMinDate";
            this.dtMinDate.Size = new System.Drawing.Size(195, 22);
            this.dtMinDate.TabIndex = 5;
            this.dtMinDate.Value = new System.DateTime(2024, 5, 20, 0, 0, 0, 0);
            // 
            // dtMaxDate
            // 
            this.dtMaxDate.Location = new System.Drawing.Point(107, 147);
            this.dtMaxDate.Name = "dtMaxDate";
            this.dtMaxDate.Size = new System.Drawing.Size(195, 22);
            this.dtMaxDate.TabIndex = 7;
            this.dtMaxDate.Value = new System.DateTime(2024, 7, 31, 0, 0, 0, 0);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(9, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 22);
            this.label4.TabIndex = 6;
            this.label4.Text = "Max. Date:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(107, 340);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 28);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Başlat";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtScheduleId
            // 
            this.txtScheduleId.Location = new System.Drawing.Point(107, 64);
            this.txtScheduleId.Name = "txtScheduleId";
            this.txtScheduleId.Size = new System.Drawing.Size(195, 22);
            this.txtScheduleId.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(9, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Schedule Id:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(222, 340);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(80, 28);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "Durdur";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtFacilityId
            // 
            this.txtFacilityId.Location = new System.Drawing.Point(107, 91);
            this.txtFacilityId.Name = "txtFacilityId";
            this.txtFacilityId.Size = new System.Drawing.Size(195, 22);
            this.txtFacilityId.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(9, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "Facility Id:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProxyUserPw
            // 
            this.txtProxyUserPw.Location = new System.Drawing.Point(107, 312);
            this.txtProxyUserPw.Name = "txtProxyUserPw";
            this.txtProxyUserPw.Size = new System.Drawing.Size(195, 22);
            this.txtProxyUserPw.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(9, 314);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 15);
            this.label7.TabIndex = 20;
            this.label7.Text = "Proxy User Pw:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProxyUserId
            // 
            this.txtProxyUserId.Location = new System.Drawing.Point(107, 285);
            this.txtProxyUserId.Name = "txtProxyUserId";
            this.txtProxyUserId.Size = new System.Drawing.Size(195, 22);
            this.txtProxyUserId.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(9, 287);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 15);
            this.label8.TabIndex = 18;
            this.label8.Text = "Proxy User Id:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProxyPort
            // 
            this.txtProxyPort.Location = new System.Drawing.Point(107, 258);
            this.txtProxyPort.Name = "txtProxyPort";
            this.txtProxyPort.Size = new System.Drawing.Size(195, 22);
            this.txtProxyPort.TabIndex = 17;
            // 
            // txtProxyIp
            // 
            this.txtProxyIp.Location = new System.Drawing.Point(107, 233);
            this.txtProxyIp.Name = "txtProxyIp";
            this.txtProxyIp.Size = new System.Drawing.Size(195, 22);
            this.txtProxyIp.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(9, 260);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 20);
            this.label9.TabIndex = 15;
            this.label9.Text = "Proxy Port:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(9, 235);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 20);
            this.label10.TabIndex = 14;
            this.label10.Text = "Proxy Ip:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtWaitMs
            // 
            this.txtWaitMs.Location = new System.Drawing.Point(107, 175);
            this.txtWaitMs.Name = "txtWaitMs";
            this.txtWaitMs.Size = new System.Drawing.Size(195, 22);
            this.txtWaitMs.TabIndex = 23;
            this.txtWaitMs.Text = "15000";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(9, 177);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 15);
            this.label11.TabIndex = 22;
            this.label11.Text = "Re-Check Ms:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtLog);
            this.groupBox1.Location = new System.Drawing.Point(315, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(366, 165);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 18);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(360, 144);
            this.txtLog.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtErrorLog);
            this.groupBox2.Location = new System.Drawing.Point(315, 183);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(366, 175);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Error Log";
            // 
            // txtErrorLog
            // 
            this.txtErrorLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorLog.Location = new System.Drawing.Point(3, 18);
            this.txtErrorLog.Multiline = true;
            this.txtErrorLog.Name = "txtErrorLog";
            this.txtErrorLog.ReadOnly = true;
            this.txtErrorLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErrorLog.Size = new System.Drawing.Size(360, 154);
            this.txtErrorLog.TabIndex = 0;
            // 
            // chReAppointment
            // 
            this.chReAppointment.AutoSize = true;
            this.chReAppointment.Checked = true;
            this.chReAppointment.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chReAppointment.Location = new System.Drawing.Point(107, 203);
            this.chReAppointment.Name = "chReAppointment";
            this.chReAppointment.Size = new System.Drawing.Size(173, 17);
            this.chReAppointment.TabIndex = 26;
            this.chReAppointment.Text = "ReAppointment current date";
            this.chReAppointment.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 383);
            this.Controls.Add(this.chReAppointment);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtWaitMs);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtProxyUserPw);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtProxyUserId);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtProxyPort);
            this.Controls.Add(this.txtProxyIp);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtFacilityId);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.txtScheduleId);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.dtMaxDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtMinDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtMail);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtMinDate;
        private System.Windows.Forms.DateTimePicker dtMaxDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtScheduleId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtFacilityId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtProxyUserPw;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtProxyUserId;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtProxyPort;
        private System.Windows.Forms.TextBox txtProxyIp;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtWaitMs;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtErrorLog;
        private System.Windows.Forms.CheckBox chReAppointment;
    }
}


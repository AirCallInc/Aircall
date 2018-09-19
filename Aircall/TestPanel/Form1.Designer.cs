namespace TestPanel
{
    partial class Form1
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
            this.btnCreateCustomerProfile = new System.Windows.Forms.Button();
            this.btnAddAddress = new System.Windows.Forms.Button();
            this.btnUpdateAddress = new System.Windows.Forms.Button();
            this.btnCreateSubscription = new System.Windows.Forms.Button();
            this.btnCreatePaymentProfile = new System.Windows.Forms.Button();
            this.btnChargeCustomerProfile = new System.Windows.Forms.Button();
            this.btnUpdateSubscription = new System.Windows.Forms.Button();
            this.btnGetSubscription = new System.Windows.Forms.Button();
            this.btnGetTransaction = new System.Windows.Forms.Button();
            this.btnGetSubscriptionTransaction = new System.Windows.Forms.Button();
            this.btnStartService = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCreateCustomerProfile
            // 
            this.btnCreateCustomerProfile.Location = new System.Drawing.Point(21, 12);
            this.btnCreateCustomerProfile.Name = "btnCreateCustomerProfile";
            this.btnCreateCustomerProfile.Size = new System.Drawing.Size(232, 30);
            this.btnCreateCustomerProfile.TabIndex = 0;
            this.btnCreateCustomerProfile.Text = "Create Customer Profile";
            this.btnCreateCustomerProfile.UseVisualStyleBackColor = true;
            this.btnCreateCustomerProfile.Click += new System.EventHandler(this.btnCreateCustomerProfile_Click);
            // 
            // btnAddAddress
            // 
            this.btnAddAddress.Location = new System.Drawing.Point(286, 12);
            this.btnAddAddress.Name = "btnAddAddress";
            this.btnAddAddress.Size = new System.Drawing.Size(232, 30);
            this.btnAddAddress.TabIndex = 1;
            this.btnAddAddress.Text = "Add Address";
            this.btnAddAddress.UseVisualStyleBackColor = true;
            this.btnAddAddress.Click += new System.EventHandler(this.btnAddAddress_Click);
            // 
            // btnUpdateAddress
            // 
            this.btnUpdateAddress.Location = new System.Drawing.Point(554, 12);
            this.btnUpdateAddress.Name = "btnUpdateAddress";
            this.btnUpdateAddress.Size = new System.Drawing.Size(232, 30);
            this.btnUpdateAddress.TabIndex = 2;
            this.btnUpdateAddress.Text = "Update Address";
            this.btnUpdateAddress.UseVisualStyleBackColor = true;
            this.btnUpdateAddress.Click += new System.EventHandler(this.btnUpdateAddress_Click);
            // 
            // btnCreateSubscription
            // 
            this.btnCreateSubscription.Location = new System.Drawing.Point(21, 61);
            this.btnCreateSubscription.Name = "btnCreateSubscription";
            this.btnCreateSubscription.Size = new System.Drawing.Size(232, 30);
            this.btnCreateSubscription.TabIndex = 3;
            this.btnCreateSubscription.Text = "Create Subscription";
            this.btnCreateSubscription.UseVisualStyleBackColor = true;
            this.btnCreateSubscription.Click += new System.EventHandler(this.btnCreateSubscription_Click);
            // 
            // btnCreatePaymentProfile
            // 
            this.btnCreatePaymentProfile.Location = new System.Drawing.Point(821, 12);
            this.btnCreatePaymentProfile.Name = "btnCreatePaymentProfile";
            this.btnCreatePaymentProfile.Size = new System.Drawing.Size(232, 30);
            this.btnCreatePaymentProfile.TabIndex = 4;
            this.btnCreatePaymentProfile.Text = "Create Payment Profile";
            this.btnCreatePaymentProfile.UseVisualStyleBackColor = true;
            this.btnCreatePaymentProfile.Click += new System.EventHandler(this.btnCreatePaymentProfile_Click);
            // 
            // btnChargeCustomerProfile
            // 
            this.btnChargeCustomerProfile.Location = new System.Drawing.Point(286, 61);
            this.btnChargeCustomerProfile.Name = "btnChargeCustomerProfile";
            this.btnChargeCustomerProfile.Size = new System.Drawing.Size(232, 30);
            this.btnChargeCustomerProfile.TabIndex = 5;
            this.btnChargeCustomerProfile.Text = "Charge Customer Profile";
            this.btnChargeCustomerProfile.UseVisualStyleBackColor = true;
            this.btnChargeCustomerProfile.Click += new System.EventHandler(this.btnChargeCustomerProfile_Click);
            // 
            // btnUpdateSubscription
            // 
            this.btnUpdateSubscription.Location = new System.Drawing.Point(554, 61);
            this.btnUpdateSubscription.Name = "btnUpdateSubscription";
            this.btnUpdateSubscription.Size = new System.Drawing.Size(232, 30);
            this.btnUpdateSubscription.TabIndex = 6;
            this.btnUpdateSubscription.Text = "Update Subscription";
            this.btnUpdateSubscription.UseVisualStyleBackColor = true;
            this.btnUpdateSubscription.Click += new System.EventHandler(this.btnUpdateSubscription_Click);
            // 
            // btnGetSubscription
            // 
            this.btnGetSubscription.Location = new System.Drawing.Point(821, 61);
            this.btnGetSubscription.Name = "btnGetSubscription";
            this.btnGetSubscription.Size = new System.Drawing.Size(232, 30);
            this.btnGetSubscription.TabIndex = 7;
            this.btnGetSubscription.Text = "Get Subscription";
            this.btnGetSubscription.UseVisualStyleBackColor = true;
            this.btnGetSubscription.Click += new System.EventHandler(this.btnGetSubscription_Click);
            // 
            // btnGetTransaction
            // 
            this.btnGetTransaction.Location = new System.Drawing.Point(21, 114);
            this.btnGetTransaction.Name = "btnGetTransaction";
            this.btnGetTransaction.Size = new System.Drawing.Size(232, 30);
            this.btnGetTransaction.TabIndex = 8;
            this.btnGetTransaction.Text = "Get Transaction";
            this.btnGetTransaction.UseVisualStyleBackColor = true;
            this.btnGetTransaction.Click += new System.EventHandler(this.btnGetTransaction_Click);
            // 
            // btnGetSubscriptionTransaction
            // 
            this.btnGetSubscriptionTransaction.Location = new System.Drawing.Point(286, 114);
            this.btnGetSubscriptionTransaction.Name = "btnGetSubscriptionTransaction";
            this.btnGetSubscriptionTransaction.Size = new System.Drawing.Size(232, 30);
            this.btnGetSubscriptionTransaction.TabIndex = 9;
            this.btnGetSubscriptionTransaction.Text = "Get Subscription Transaction";
            this.btnGetSubscriptionTransaction.UseVisualStyleBackColor = true;
            this.btnGetSubscriptionTransaction.Click += new System.EventHandler(this.btnGetSubscriptionTransaction_Click);
            // 
            // btnStartService
            // 
            this.btnStartService.Location = new System.Drawing.Point(554, 114);
            this.btnStartService.Name = "btnStartService";
            this.btnStartService.Size = new System.Drawing.Size(232, 30);
            this.btnStartService.TabIndex = 10;
            this.btnStartService.Text = "Start Service";
            this.btnStartService.UseVisualStyleBackColor = true;
            this.btnStartService.Click += new System.EventHandler(this.btnStartService_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(35, 190);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(239, 20);
            this.textBox1.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1077, 450);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnStartService);
            this.Controls.Add(this.btnGetSubscriptionTransaction);
            this.Controls.Add(this.btnGetTransaction);
            this.Controls.Add(this.btnGetSubscription);
            this.Controls.Add(this.btnUpdateSubscription);
            this.Controls.Add(this.btnChargeCustomerProfile);
            this.Controls.Add(this.btnCreatePaymentProfile);
            this.Controls.Add(this.btnCreateSubscription);
            this.Controls.Add(this.btnUpdateAddress);
            this.Controls.Add(this.btnAddAddress);
            this.Controls.Add(this.btnCreateCustomerProfile);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateCustomerProfile;
        private System.Windows.Forms.Button btnAddAddress;
        private System.Windows.Forms.Button btnUpdateAddress;
        private System.Windows.Forms.Button btnCreateSubscription;
        private System.Windows.Forms.Button btnCreatePaymentProfile;
        private System.Windows.Forms.Button btnChargeCustomerProfile;
        private System.Windows.Forms.Button btnUpdateSubscription;
        private System.Windows.Forms.Button btnGetSubscription;
        private System.Windows.Forms.Button btnGetTransaction;
        private System.Windows.Forms.Button btnGetSubscriptionTransaction;
        private System.Windows.Forms.Button btnStartService;
        private System.Windows.Forms.TextBox textBox1;
    }
}


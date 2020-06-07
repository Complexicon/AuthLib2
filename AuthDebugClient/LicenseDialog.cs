using AuthLib;
using System;
using System.Windows.Forms;

namespace AuthDebugClient {
	public partial class LicenseDialog : Form {
		private readonly string url;
		private readonly AuthManager manager;

		public LicenseDialog(string url, AuthManager managerRef) {
			this.url = url;
			manager = managerRef;
			InitializeComponent();
		}

		private void Button1_Click(object sender, EventArgs e) {
			if(textBox1.Text != "") {
				var resp = manager.CreateAndValidateLicense(url, textBox1.Text);
				MessageBox.Show(resp.ToString());
				if(resp == ServerResponse.SUCCESS) {
					DialogResult = DialogResult.OK;
					Close();
				}
			}
		}
	}
}

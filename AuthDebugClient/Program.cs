using AuthLib;
using System;
using System.IO;
using System.Windows.Forms;

namespace AuthDebugClient {
	static class Program {

		static void Main() {
			var pubkey = "<RSAKeyValue><Modulus>uJUoIklvO8oex1cC3CxUevjNC2craY3IbKqvEBRLVeOB8Sq6b6Ozr+QHekhpZ6Iouc/6ydKyS47S0OmMG7yWgsTz8EIVVbhOJmKXS8NGkHBeSeJla9PJ/0umEn21zl68SCj2Vir8DkcW3+i6Md2z0PblImdDzKKS4Zi56tTTacFav59WfZHPJkd48oZZneFQ5J1rVS3VsPhjIluU4AZhr1ciZQWjr55S9U2yCf+dj+WlxdYsZ9sXIXU2OSUiENqju7EI9V9QRcA0U8uZAY9224zylypNF9e2j2BFtScvARI5lU+sAVsg9tn2+9uvYiBylqlXdttGX0L7G6Qe0e8NMgBtuiOciStzLp0Bbl8m6jZ8RaX6SNB65kdgoRrQkQsjzQQSzXxMPt6Ohgv2LwmE1IhxMZ0F+tY7s2cYS1sHwUoP2rYWxzSZnuJwvAtIWjCT4LQM+9iQSNrYKn6kF/LqkXlFKEIvRpq7Bu5Ir7LkfFPtgEhGpypkwCoasQ1cIKBh7VZSog2BFHXE8FN6/arQaAAz+S4grkeJUE+Ov68gW5Xed9enw+sO3OHmMuR3jA/zkyfJ28AWATrv/gi2LxVIX9b9vCWSZephWf/YmWwYIzOt6CiuCD8L4BaFdM80S6chbgrcZv6dGk7lGH3Pv+1HB+9Agb2UohTCeqXle87EoAU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try {
				File.Delete(Path.Combine(Path.GetTempPath(), "license.dat"));
			} catch { }
			
			AuthManager mgr = new AuthManager(pubkey, Path.GetTempPath());

			if (mgr.TryReadLicense() && mgr.ValidateLicense()) MainFunc();
			else {
				MessageBox.Show("Failed to Read License File!");
				if(new LicenseDialog("http://localhost:8000/license.php", mgr).ShowDialog() == DialogResult.OK) MainFunc();
				else {
					MessageBox.Show("Not Activated");
					Environment.Exit(0);
				}
			}
		}

		static void MainFunc() => MessageBox.Show("Is Activated");

	}
}

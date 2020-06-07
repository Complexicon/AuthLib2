using System;
using System.IO;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using static AuthLib.ServerResponse;

namespace AuthLib {
	public class AuthManager {
		private const string QueryString = "Select UUID From Win32_ComputerSystemProduct";

		private readonly RSACryptoServiceProvider r;
		private readonly string dataPath;
		private readonly string licensePath;
		private readonly byte[] hwid;
		private readonly string hwidB64;
		private byte[] license = new byte[16];

		public AuthManager(string xmlKey, string programDataPath) {
			r = new RSACryptoServiceProvider();
			r.FromXmlString(xmlKey);

			dataPath = programDataPath;
			licensePath = Path.Combine(dataPath, "license.dat");

			foreach (ManagementObject mo in new ManagementObjectSearcher(QueryString).Get()) {
				hwid = Guid.Parse(mo["UUID"].ToString()).ToByteArray();
				break;
			}

			hwidB64 = Convert.ToBase64String(hwid);

		}

		/// <summary>
		/// Reads the License File. Returns true or false depending on if was successful.
		/// </summary>
		public bool TryReadLicense() {
			try {
				license = File.ReadAllBytes(licensePath);
				return true;
			} catch { return false; }
		}


		/// <summary>
		/// Tries to Validate the License File. Make sure to use TryReadLicense() first!
		/// </summary>
		public bool ValidateLicense() => r.VerifyData(hwid, new SHA256CryptoServiceProvider(), license);


		/// <summary>
		/// Connects to URL and Retrieves Signed Base64 License and Saves to file. Returns A ServerResponse
		/// </summary>
		public ServerResponse CreateAndValidateLicense(string url, string serial) {
			try {
				var connectstring = url + "?serial=" + serial + "&hwid=" + hwidB64.Replace('+', '-').Replace('/', '_').Replace("=", "%3D");
				var responseString = new WebClient().DownloadString(connectstring);

				if(responseString == "serial_invalid") return INVALID_SERIAL;
				if(responseString == "noargs") return INVALID_ARGS;

				license = Convert.FromBase64String(responseString);

				if(ValidateLicense()) {
					Directory.CreateDirectory(dataPath);
					File.WriteAllBytes(licensePath, license);
					return SUCCESS;
				} else return INVALID_LICENSE;
			} 
			catch(WebException) { return OFFLINE; }
			catch { return OTHER; }
		}
	}

	public enum ServerResponse {
		SUCCESS,
		INVALID_ARGS,
		INVALID_SERIAL,
		INVALID_LICENSE,
		OFFLINE,
		OTHER = -1
	}

}

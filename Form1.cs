using System;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using System.Numerics;

namespace Lab1_sending_data_by_mail
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		private void label1_Click(object sender, EventArgs e)
		{

		}
		private void label3_Click(object sender, EventArgs e)
		{

		}
		private void button1_Click(object sender, EventArgs e)
		{
			int p = Convert.ToInt32(textBox6.Text); int q = Convert.ToInt32(textBox7.Text); int d = Convert.ToInt32(textBox8.Text);
			string shifr = ALG_SHIFR_RSA(p, q, d);

			ToSent(textBox1.Text, textBox5.Text, Send(textBox1.Text, textBox2.Text, textBox5.Text, textBox3.Text, shifr));

		}
		private void button2_Click(object sender, EventArgs e)
		{
		}
		private MailMessage Send(string from, string to, string password, string subject, string body)
		{
			MailMessage message = new MailMessage(from, to);
			message.Subject = subject;
			message.Body = body;
			SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
			smtp.Credentials = new NetworkCredential(from, password);
			smtp.EnableSsl = true;
			smtp.Send(message);
			return message;
		}
		private void ToSent(string login, string password, MailMessage message)
		{
			ImapClient imap = new ImapClient();
			imap.Connect("imap.gmail.com", 143, true);
			imap.Authenticate(login, password);
			FolderNamespace folderNamespaceSent = new FolderNamespace('/', "Отправленные");
			IMailFolder folderSent = imap.GetFolder(folderNamespaceSent);
			folderSent.Open(FolderAccess.ReadOnly);
			MimeMessage mimeMessage = (MimeMessage)message;
			folderSent.Append(mimeMessage);
		}
		private string ALG_SHIFR_RSA(int p, int q, int d)
		{
			string message = textBox4.Text;
			string soob = "";
			int n = p * q;
			int fi = (p - 1) * (q - 1);
			int e = 0;
			for (int k = 1; k > 0; k++)
			{
				if ((fi * k + 1) % d == 0 & ((fi * k + 1) / d) < fi)
				{
					e = (fi * k + 1) / d;
					break;
				}
			}
			textBox9.Text = ("( " + e + ", " + n + " )");
			textBox10.Text = ("( " + d + ", " + n + " )");
			char[] message_arr = message.ToCharArray();
			int[] nom_bukv = FIO(message_arr);
			int[] shifrr = new int[message.Length];
			textBox12.Text = textBox4.Text;
			for (int i = 0; i < nom_bukv.Length; i++)
			{
				BigInteger ii = nom_bukv[i], j = e, t = n;
				shifrr[i] = (int)BigInteger.ModPow(ii, j, t);
				textBox11.Text = textBox11.Text + shifrr[i] + " ";
				soob = soob + Convert.ToString(shifrr[i]) + " ";
			}
			soob = soob + "\nЭЦП: " + Convert.ToString((int)BigInteger.ModPow(shifrr[nom_bukv.Length - 1], d, n));
			textBox13.Text = Convert.ToString((int)BigInteger.ModPow(shifrr[nom_bukv.Length-1], d, n));
			return soob;
		}
		class ALG_SHIFR_GOST
		{
			int[,] table;
			public ALG_SHIFR_GOST()
			{
				table = new int[,] { {1,13,4,6,7,5,14,4},{15,11,11,12,13,8,11,10}, {13,4,10,7,10,1,4,9},
				{0,1,0,1,1,13,12,2}, {5,3,7,5,0,10,6,13}, {7,15,2,15,8,3,13,8 }, {10,5,1,13,9,4,15,0},
				{4,9,13,8,15,2,10,14}, {9,0,3,4,14,14,2,6}, {2,10,6,10,4,15,3,11}, {3,14,8,9,6,12,8,1},
				{14,7,5,14,12,7,1,12}, {6,6,9,0,11,6,0,7}, {11,8,12,3,2,0,7,15},{8,2,15,11,5,9,5,5},
				{12,12,14,2,3,11,9,3}};
			}
			public char[] Alfavit()
			{
				char[] alf = new char[33];
				int j = 0;
				for (char i = 'А'; i <= 'Я'; i++)
				{
					alf[j] = i;
					j++;
				}
				alf[32] = ' ';

				return alf;
			}
			private int[,] binar()
			{
				int[,] alf1 = new int[33, 8];
				int[] alf2 = new int[33];
				int j = 0;
				for (int i = 192; i <= 223; i++)
				{
					alf2[j] = i;
					j++;
				}
				alf2[32] = 32;

				for (int i = 0; i < 32; i++)
				{
					j = 0;
					while (alf2[i] > 1)
					{
						alf1[i, 7 - j] = alf2[i] % 2;
						alf2[i] = alf2[i] / 2;
						j++;
					}
					alf1[i, 0] = 1;
				}
				alf1[32, 2] = 1;

				return alf1;
			}
			public int[,] Char_in_bin(char[] a)
			{
				int[,] a_bin = new int[a.Length, 8];
				char[] alf = Alfavit();
				int[,] bin = binar();
				for (int i = 0; i < a.Length; i++)
				{
					for (int j = 0; j < alf.Length; j++)
					{
						if (a[i] == alf[j])
						{
							for (int l = 0; l < 8; l++)
							{
								a_bin[i, l] = bin[j, l];
							}
						}
					}
				}
				return a_bin;
			}
			private void Mod_32(int a, int b, int c, out int d, out int e)
			{
				int sum = a + b + c;
				if (sum == 2)
				{
					d = 0; e = 1;
				}
				else if (sum == 3)
				{
					d = 1; e = 1;
				}
				else
				{
					d = sum; e = 0;
				}
			}
			public void Mod_2(int a, int b, out int c)
			{
				int sum = a + b;
				if (sum == 2)
				{
					c = 0;
				}
				else
				{
					c = sum;
				}
			}
			private void choose(int[,] a)
			{
				int[] dec = new int[8];
				int[,] bufer = new int[4, 8];
				int r = 7;
				for (int i = 3; i >= 0; i--)
				{
					for (int j = 7; j >= 0; j--)
					{
						if (j == 7 | j == 3)
						{
							dec[r] = 1 * a[i, j] + 2 * a[i, j - 1] + 4 * a[i, j - 2] + 8 * a[i, j - 3];
							r--;
						}
					}
				}
				r = 7;
				int t;
				int f = 0, e = 3; string result = "";
				Console.Write("\n\nЭлементы замены: ");
				for (int i = 7; i >= 0; i--)
				{
					t = table[dec[r], i];
					result = t + " " + result;
					if (t >= 1)
					{
						while (t > 1)
						{
							bufer[e, 7 - f] = t % 2;
							t /= 2;
							f++;
						}
						bufer[e, 7 - f] = 1;
					}
					if (i % 2 == 0)
					{
						f = 0; e = e - 1;
					}
					else
					{
						f = 4;
					}
					r--;
				}
				Console.Write(result);
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						a[i, j] = bufer[i, j];
					}
				}
			}
			private void shift(int[,] a)
			{
				int[] sd = new int[32];
				int r = 0;
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						sd[r] = a[i, j];
						r++;
					}
				}
				r = 11;
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						a[i, j] = sd[r];
						if (r == 31)
						{
							r = -1;
						}
						r++;
					}
				}
			}
		}
		public int[] FIO(char[] a)
		{
			ALG_SHIFR_GOST gost = new ALG_SHIFR_GOST();
			char[] alf = gost.Alfavit();
			int[] vihod = new int[a.Length];
			for (int i = 0; i < a.Length; i++)
			{
				for (int j = 0; j < alf.Length; j++)
				{
					if (a[i] == alf[j])
					{
						vihod[i] = j + 1;
					}
				}
			}
			return vihod;
		}
	}
}

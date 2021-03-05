using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Diagnostics;
using RSACrypto;

namespace RSACrypto
{
    public partial class EDForm : Form
    {
        static ulong p, q, e, d, n = 0;
        static string stringImage = "";
        static string cipherText = "";

        OpenFileDialog openFile = new OpenFileDialog();

        public EDForm()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            DisableUI();
        }

        public string Encrypt(string image)
        {
            string hexImage = image;
            char[] hexImageArray = hexImage.ToCharArray();
            string cipher = "";
            progressBar1.Maximum = hexImageArray.Length;
            for (int i = 0; i < hexImageArray.Length; i++)
            {
                Application.DoEvents();
                progressBar1.Value = i;
                if (cipher == "")
                {
                    cipher += RSAlgorithm.BigMod(hexImageArray[i], e, n);
                }
                else
                {
                    cipher += "-" + RSAlgorithm.BigMod(hexImageArray[i], e, n);
                }
            }
            return cipher;
        }

        public string Decrypt(string cipherText)
        {
            char[] cipherArray = cipherText.ToCharArray();
            int i = 0;
            string decryptedCipher = "";
            progressBar2.Maximum = cipherArray.Length;
            try
            {
                for (; i < cipherArray.Length; i++)
                {
                    Application.DoEvents();
                    string cipher = "";
                    progressBar2.Value = i;
                    int j;
                    for (j = i; cipherArray[j] != '-'; j++)
                    {
                        cipher += cipherArray[j];
                    }

                    i = j;

                    ulong cipherValue = Convert.ToUInt64(cipher);
                    decryptedCipher += ((char)RSAlgorithm.BigMod(cipherValue, d, n)).ToString();
                }
            }
            catch (Exception) { }
            return decryptedCipher;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (button3.Text == "Set Details")
            {
                if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
                {
                    MessageBox.Show("Enter Valid Detail For RSA", "ERROR");
                }

                else
                {
                    if (RSACrypto.RSAHelper.IsPrime(Convert.ToInt32(textBox2.Text)))
                    {
                        p = Convert.ToUInt64(textBox2.Text);
                    }
                    else
                    {
                        textBox2.Text = "";
                        MessageBox.Show("Enter Prime Number");
                        return;
                    }
                    if (RSACrypto.RSAHelper.IsPrime(Convert.ToInt32(textBox3.Text)))
                    {
                        q = Convert.ToUInt64(textBox3.Text);
                    }
                    else
                    {
                        textBox3.Text = "";
                        MessageBox.Show("Enter Prime Number");
                        return;
                    }

                    EDForm.e = Convert.ToUInt64(textBox4.Text);

                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    textBox4.Enabled = false;
                    button4.Enabled = true;
                    button3.Text = "ReSet Details";
                }
            }
            else
            {
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                button4.Enabled = false;
                button3.Text = "Set Details";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            n = RSACrypto.RSAlgorithm.N(p, q);
            button1.Enabled = false;
            DisableUI();

            //Get running time for encription
            Stopwatch TimerForEncription = new Stopwatch();
            TimerForEncription.Start();
            String en = Encrypt(stringImage);
            TimerForEncription.Stop();
            //End running time calculation

            File.WriteAllText(textBox5.Text, en);
            MessageBox.Show("Encryption Done");
            button1.Enabled = true;
            button2.Enabled = true;
            button4.Enabled = true;

            button6.Enabled = true;
            button7.Enabled = true;
            button9.Enabled = true;
            button10.Enabled = true;
            button5.Enabled = true;
            GetRunningTime(TimerForEncription.ElapsedMilliseconds.ToString());
        }

        private void GetRunningTime(string EncriptionTime)
        {
            //add a new row

            int n = dataGridView1.Rows.Add();
            dataGridView1.Rows[n].Cells[0].Value = "Encription";
            dataGridView1.Rows[n].Cells[1].Value = tblFileName2.Text.ToString();
            dataGridView1.Rows[n].Cells[2].Value = tblImageSize2.Text.ToString();
            dataGridView1.Rows[n].Cells[3].Value = tblImageResolution2.Text.ToString();
            dataGridView1.Rows[n].Cells[4].Value = "Image";
            dataGridView1.Rows[n].Cells[5].Value = EncriptionTime;

            dataGridView1.Rows[n].Cells[6].Value = textBox2.Text.ToString();
            dataGridView1.Rows[n].Cells[7].Value = textBox3.Text.ToString();
            dataGridView1.Rows[n].Cells[8].Value = textBox4.Text.ToString();
            dataGridView1.Rows[n].Cells[9].Value = textBox9.Text.ToString();
            dataGridView1.Rows[n].Cells[10].Value = textBox8.Text.ToString();

        }
        private void button6_Click(object sender, EventArgs e)
        {
            button10.Enabled = false;
            DisableUI();

            //Get running time for encription
            Stopwatch TimerForEncription = new Stopwatch();
            TimerForEncription.Start();
            String de = Decrypt(cipherText);
            TimerForEncription.Stop();

            pictureBox1.Image = RSACrypto.RSAHelper.ConvertByteToImage(RSACrypto.RSAHelper.DecodeHex(de));
            MessageBox.Show("Decryption Done");
            pictureBox1.Image.Save(textBox6.Text, System.Drawing.Imaging.ImageFormat.Jpeg);
            MessageBox.Show("Picture Saved");

            button6.Enabled = true;
            button7.Enabled = true;
            button9.Enabled = true;
            button10.Enabled = true;

            button1.Enabled = true;
            button2.Enabled = true;
            button4.Enabled = true;
            GetDecriptionResult(TimerForEncription.ElapsedMilliseconds.ToString());
        }
        private void GetDecriptionResult(string EncriptionTime)
        {
            //add a new row

            int n = dataGridView1.Rows.Add();
            dataGridView1.Rows[n].Cells[0].Value = "Decription";
            dataGridView1.Rows[n].Cells[1].Value = tblFileName2.Text.ToString();
            dataGridView1.Rows[n].Cells[2].Value = tblImageSize2.Text.ToString();
            dataGridView1.Rows[n].Cells[3].Value = tblImageResolution2.Text.ToString();
            dataGridView1.Rows[n].Cells[4].Value = "Image";
            dataGridView1.Rows[n].Cells[5].Value = EncriptionTime;

            dataGridView1.Rows[n].Cells[6].Value = textBox2.Text.ToString();
            dataGridView1.Rows[n].Cells[7].Value = textBox3.Text.ToString();
            dataGridView1.Rows[n].Cells[8].Value = textBox4.Text.ToString();
            dataGridView1.Rows[n].Cells[9].Value = textBox9.Text.ToString();
            dataGridView1.Rows[n].Cells[10].Value = textBox8.Text.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (button8.Text == "Set Details")
            {
                if (textBox9.Text == "" || textBox8.Text == "")
                {
                    MessageBox.Show("Enter Valid Detail For RSA", "ERROR");
                }
                else
                {
                    if (Convert.ToInt32(textBox9.Text) > 0)
                    {
                        d = Convert.ToUInt64(textBox9.Text);
                    }
                    else
                    {
                        textBox9.Text = "";
                        MessageBox.Show("Enter Valid Number");
                        return;
                    }
                    if (Convert.ToInt32(textBox8.Text) > 0)
                    {
                        n = Convert.ToUInt64(textBox8.Text);
                    }
                    else
                    {
                        textBox8.Text = "";
                        MessageBox.Show("Enter Valid Number");
                        return;
                    }

                    textBox9.Enabled = false;
                    textBox8.Enabled = false;
                    button8.Text = "ReSet Details";
                    button7.Enabled = true;

                }
            }
            else
            {
                textBox9.Text = "";
                textBox8.Text = "";
                textBox9.Enabled = true;
                textBox8.Enabled = true;
                button8.Text = "Set Details";
                button7.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stringImage = BitConverter.ToString(RSACrypto.RSAHelper.ConvertImageToByte(pictureBox1.Image));
            MessageBox.Show("Image Load Successfully");
            groupBox4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog save1 = new SaveFileDialog();
            save1.Filter = "Cipher|*.cipher";
            if (save1.ShowDialog() == DialogResult.OK)
            {
                textBox5.Text = save1.FileName;
                button5.Enabled = true;
            }
            else
            {
                textBox5.Text = "";
                button5.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OpenFileDialog open1 = new OpenFileDialog();
            openFile.Filter = "Image or Text Files|*.jpg;*.txt";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFile.FileName;
                pictureBox1.Image = Image.FromFile(textBox1.Text);
                button2.Enabled = true;
                FileInfo fi = new FileInfo(textBox1.Text);

                label9.Text = "File Name: " + fi.Name;
                label10.Text = "Image Resolution: " + pictureBox1.Image.PhysicalDimension.Height + " X " + pictureBox1.Image.PhysicalDimension.Width;

                double imageMB = ((fi.Length / 1024f) / 1024f);

                label11.Text = "Image Size: " + imageMB.ToString(".##") + "MB";

                tblFileName2.Text = fi.Name.ToString();
                tblImageResolution2.Text = pictureBox1.Image.PhysicalDimension.Height.ToString() + " X " + pictureBox1.Image.PhysicalDimension.Width.ToString();
                tblImageSize2.Text = imageMB.ToString(".##") + "MB";
            }
            else
            {
                textBox1.Text = "";
                label9.Text = "File Name: ";
                label10.Text = "Image Resolution: ";
                label11.Text = "Image Size: ";

                tblFileName2.Text = "";
                tblImageResolution2.Text = "";
                tblImageSize2.Text = "";

                button2.Enabled = false;

            }
        }

        private void DisableUI()
        {
            button2.Enabled = false;
            groupBox4.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;

            button9.Enabled = false;
            groupBox5.Enabled = false;
            button7.Enabled = false;
            button6.Enabled = false;
        }

        private void EnableUI()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            groupBox4.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog open1 = new OpenFileDialog
            {
                Filter = "Cipher Files |*.cipher"
            };
            if (open1.ShowDialog() == DialogResult.OK)
            {
                textBox7.Text = open1.FileName;
                button9.Enabled = true;
            }
            else
            {
                textBox7.Text = "";
                button9.Enabled = false;
            }
        }

        private void Button9_Click(object sender, EventArgs e)
        { 
            cipherText = File.ReadAllText(textBox7.Text);
            MessageBox.Show("Cipher Loaded Successfully");
            groupBox5.Enabled = true;
        }

        private void Button7_Click(object sender, EventArgs e)
        { 
            SaveFileDialog save1 = new SaveFileDialog
            {
                Filter = "Image or Text File|*.jpg;*.txt"
            };
            if (save1.ShowDialog() == DialogResult.OK)
            {
                textBox6.Text = save1.FileName;
                button6.Enabled = true;
            }
            else
            {
                textBox6.Text = "";
                button6.Enabled = false;
            }
        }

        private void BtnGetRSAElements_Click(object sender, EventArgs e)
        {
            //65537
            var primes = RSACrypto.RSAHelper.GeneratePrimes(10000);
            var result = RSACrypto.RSAHelper.GetTwoPrimes(primes);

            string Prime1 = primes[(int)result[0]].ToString();
            string Prime2 = primes[(int)result[1]].ToString();
            var N = primes[(int)result[0]] * primes[(int)result[1]];
            p = Convert.ToUInt64(primes[(int)result[0]]);
            q = Convert.ToUInt64(primes[(int)result[1]]);

            textBox2.Text = Prime1;
            textBox3.Text = Prime2;
            textBox8.Text = N.ToString();
            //
            //call the function for calculating e here
            var phi = RSAlgorithm.Phi(p, q);
            EDForm.e = RSAlgorithm.Find_E(phi);
            bool flag = false;
            for (ulong j = 1; !flag; j++)
            {
                if ((EDForm.e * j) % phi == 1)
                {
                    d = j;
                    flag = true;
                    break;
                }
            }
            //Set Public and Private Key
            textBox4.Text = EDForm.e.ToString();
            textBox9.Text = d.ToString();

            if (MakerCheckerMessageMemoEdit.Text.Trim() == "")
                MakerCheckerMessageMemoEdit.Text = "P=" + Prime1 + ", Q=" + Prime2 + ", N=" + N + System.Environment.NewLine + "Public Key=[" + EDForm.e.ToString() + "," + N + "]" + System.Environment.NewLine + "Private Key=[" + d.ToString() + ", " + N + "]";
            else
                MakerCheckerMessageMemoEdit.Text = MakerCheckerMessageMemoEdit.Text + System.Environment.NewLine + "_______________________________________________" + System.Environment.NewLine + "P=" + Prime1 + ", Q=" + Prime2 + ", N=" + N + System.Environment.NewLine + "Public Key=[" + EDForm.e.ToString() + "," + N + "]" + System.Environment.NewLine + "Private Key=[" + d.ToString() + ", " + N + "]";

            //button3.Text = "Reset Details";
            Application.DoEvents();
        }

        private void button14_Click(object sender, EventArgs e)
        {

        }
    }
}

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
        static string stringImage, stringFile = "";
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

        public string Encrypt(string image, ProgressBar progressBar)
        {
            string hexImage = image;
            char[] hexImageArray = hexImage.ToCharArray();
            string cipher = "";
            progressBar.Maximum = hexImageArray.Length;
            for (int i = 0; i < hexImageArray.Length; i++)
            {
                Application.DoEvents();
                progressBar.Value = i;
                //lblPercentage.Text = ((double)i / (double)progressBar.Maximum * 100).ToString();

                int percent = (int)(((double)progressBar.Value / (double)progressBar.Maximum) * 100);
                ///progressBar.Refresh();
                progressBar.CreateGraphics().DrawString(percent.ToString() + "%",
                    new Font("Arial", (float)8.25, FontStyle.Regular),
                    Brushes.Black,
                    new PointF(progressBar.Width / 2 - 10, progressBar.Height / 2 - 7));

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

        public string Decrypt(string cipherText, ProgressBar progressBar)
        {
            char[] cipherArray = cipherText.ToCharArray();
            int i = 0;
            string decryptedCipher = "";
            progressBar.Maximum = cipherArray.Length;
            try
            {
                for (; i < cipherArray.Length; i++)
                {
                    Application.DoEvents();
                    string cipher = "";
                    progressBar.Value = i;
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
            /*if (button3.Text == "Set Details")
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
            }*/
        }

        private void BtnEncryptImage_Click(object sender, EventArgs e)
        {
            n = RSAlgorithm.N(p, q);
            BtnSelectImage.Enabled = false;
            DisableUI();

            //Get running time for encription
            Stopwatch TimerForEncription = new Stopwatch();
            TimerForEncription.Start();
            String en = Encrypt(stringImage, progressBar1);
            TimerForEncription.Stop();
            //End running time calculation

            File.WriteAllText(txtImageCipherPath.Text, en);
            MessageBox.Show("Encryption Done");
            BtnSelectImage.Enabled = true;
            BtnLoadImage.Enabled = true;
            BtnSaveImageCipherAt.Enabled = true;

            BtnDecryptImage.Enabled = true;
            BtnSelectDecrytedImagePath.Enabled = true;
            BtnLoadImageCipher.Enabled = true;
            button10.Enabled = true;
            BtnEncryptImage.Enabled = true;
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

            dataGridView1.Rows[n].Cells[6].Value = txtImageP.Text.ToString();
            dataGridView1.Rows[n].Cells[7].Value = txtImageQ.Text.ToString();
            dataGridView1.Rows[n].Cells[8].Value = txtImageE.Text.ToString();
            dataGridView1.Rows[n].Cells[9].Value = txtImageD.Text.ToString();
            dataGridView1.Rows[n].Cells[10].Value = txtImageN.Text.ToString();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            button10.Enabled = false;
            DisableUI();

            //Get running time for encription
            Stopwatch TimerForEncription = new Stopwatch();
            TimerForEncription.Start();
            string decrypted = Decrypt(cipherText, progressBar2);
            TimerForEncription.Stop();

            pictureBox1.Image = RSAHelper.ConvertByteToImage(RSAHelper.DecodeHex(decrypted));
            MessageBox.Show("Decryption Done");
            pictureBox1.Image.Save(txtDecryptedImagePath.Text, System.Drawing.Imaging.ImageFormat.Jpeg);
            MessageBox.Show("Picture Saved");

            BtnDecryptImage.Enabled = true;
            BtnSelectDecrytedImagePath.Enabled = true;
            BtnLoadImageCipher.Enabled = true;
            button10.Enabled = true;

            BtnSelectImage.Enabled = true;
            BtnLoadImage.Enabled = true;
            BtnSaveImageCipherAt.Enabled = true;
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

            dataGridView1.Rows[n].Cells[6].Value = txtImageP.Text.ToString();
            dataGridView1.Rows[n].Cells[7].Value = txtImageQ.Text.ToString();
            dataGridView1.Rows[n].Cells[8].Value = txtImageE.Text.ToString();
            dataGridView1.Rows[n].Cells[9].Value = txtImageD.Text.ToString();
            dataGridView1.Rows[n].Cells[10].Value = txtImageN.Text.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            /*if (button8.Text == "Set Details")
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
            }*/
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            stringImage = BitConverter.ToString(RSAHelper.ConvertImageToByte(pictureBox1.Image));
            MessageBox.Show("Image Load Successfully");
            groupBox4.Enabled = true;

            ////65537
            //var primes = RSAMath.GeneratePrimes(10000);
            //var result = RSAMath.GetTwoPrimes(primes);

            //string Prime1 = primes[(int)result[0]].ToString();
            //string Prime2 = primes[(int)result[1]].ToString();
            //var N = primes[(int)result[0]] * primes[(int)result[1]];
            //p = Convert.ToUInt64(primes[(int)result[0]]);
            //q = Convert.ToUInt64(primes[(int)result[1]]);

            //textBox2.Text = Prime1;
            //textBox3.Text = Prime2;
            //textBox8.Text = N.ToString();
            ////
            ////call the function for calculating e here
            //var phi = RSAlgorithm.Phi(p, q);
            //EDForm.e = RSAlgorithm.Find_E(phi);
            //bool flag = false;
            //for (ulong j = 1; !flag; j++)
            //{
            //    if (EDForm.e * j % phi == 1)
            //    {
            //        d = j;
            //        flag = true;
            //        break;
            //    }
            //}
            ////Set Public and Private Key
            //textBox4.Text = EDForm.e.ToString();
            //textBox9.Text = d.ToString();

            //if (MakerCheckerMessageMemoEdit.Text.Trim() == "")
            //    MakerCheckerMessageMemoEdit.Text = "P=" + Prime1 + ", Q=" + Prime2 + ", N=" + N + Environment.NewLine + "Public Key=[" + EDForm.e.ToString() + "," + N + "]" + System.Environment.NewLine + "Private Key=[" + d.ToString() + ", " + N + "]";
            //else
            //    MakerCheckerMessageMemoEdit.Text = MakerCheckerMessageMemoEdit.Text + System.Environment.NewLine + "_______________________________________________" + System.Environment.NewLine + "P=" + Prime1 + ", Q=" + Prime2 + ", N=" + N + System.Environment.NewLine + "Public Key=[" + EDForm.e.ToString() + "," + N + "]" + System.Environment.NewLine + "Private Key=[" + d.ToString() + ", " + N + "]";

            ////button3.Text = "Reset Details";
            Application.DoEvents();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveCiper(txtImageCipherPath);
        }

        /// <summary>
        /// Save Cipher File
        /// </summary>
        /// <param name="textbox"></param>
        private void SaveCiper(TextBox textbox)
        {
            SaveFileDialog save1 = new SaveFileDialog
            {
                Filter = "Cipher File|*.cipher"
            };

            if (save1.ShowDialog() == DialogResult.OK)
            {
                textbox.Text = save1.FileName;
                BtnEncryptImage.Enabled = true;
            }
            else
            {
                textbox.Text = "";
                BtnEncryptImage.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OpenFileDialog open1 = new OpenFileDialog();
            openFile.Filter = "Image or Text Files|*.jpg;";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtImageFilePath.Text = openFile.FileName;
                pictureBox1.Image = Image.FromFile(txtImageFilePath.Text);
                BtnLoadImage.Enabled = true;
                FileInfo fi = new FileInfo(txtImageFilePath.Text);

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
                txtImageFilePath.Text = "";
                label9.Text = "File Name: ";
                label10.Text = "Image Resolution: ";
                label11.Text = "Image Size: ";

                tblFileName2.Text = "";
                tblImageResolution2.Text = "";
                tblImageSize2.Text = "";

                BtnLoadImage.Enabled = false;
            }
        }

        private void DisableUI()
        {
            BtnLoadImage.Enabled = false;
            groupBox4.Enabled = false;
            //button4.Enabled = false;
            BtnEncryptImage.Enabled = false;

            BtnLoadImageCipher.Enabled = false;
            groupBox5.Enabled = false;
            BtnSelectDecrytedImagePath.Enabled = false;
            BtnDecryptImage.Enabled = false;
        }

        private void EnableUI()
        {
            BtnSelectImage.Enabled = true;
            BtnLoadImage.Enabled = true;
            groupBox4.Enabled = true;
            //button4.Enabled = true;
            BtnEncryptImage.Enabled = true;
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            OpenCipherFile(txtSrcImageCipher, BtnLoadImageCipher);
        }

        private void OpenCipherFile(TextBox textbox, Button button)
        {
            OpenFileDialog open1 = new OpenFileDialog
            {
                Filter = "Cipher Files |*.cipher"
            };
            if (open1.ShowDialog() == DialogResult.OK)
            {
                textbox.Text = open1.FileName;
                button.Enabled = true;
            }
            else
            {
                textbox.Text = "";
                button.Enabled = false;
            }
        }

        
        /// <summary>
        /// Choose the text file path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            openFile.Filter = "Text Files|*.txt";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                TextFfilePath.Text = openFile.FileName;
                TxtFileContent.Text = ReadFile(TextFfilePath.Text);
                BtnLoadTextToEncr.Enabled = true;
                FileInfo fi = new FileInfo(TextFfilePath.Text);

                lblFileName.Text = "File Name: " + fi.Name;
                txtWordCount.Text = "Word Count" + TxtFileContent.Text.Split(' ').Count().ToString();

                tblFileName2.Text = fi.Name.ToString();
            }
            else
            {
                txtImageFilePath.Text = "";
                label9.Text = "File Name : ";
                label10.Text = "Word Count : ";

                tblFileName2.Text = "";
                tblImageResolution2.Text = "";
                tblImageSize2.Text = "";

                BtnLoadImage.Enabled = false;
            }
        }

        /// <summary>
        /// Reads and returns the content from the selected file
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ReadFile(string text)
        {
            return System.IO.File.ReadAllText(text);
        }

        private void BtnSetDetailsTextDec_Click(object sender, EventArgs e)
        {
            //if (BtnSetDetailsTextDec.Text == "Set Details")
            //{
            //    if (txtImageP.Text == "" || txtImageQ.Text == "" || txtImageE.Text == "")
            //    {
            //        MessageBox.Show("Enter Valid Detail For RSA", "ERROR");
            //    }
            //    else
            //    {
            //        if (RSACrypto.RSAMath.IsPrime(Convert.ToInt32(txtImageP.Text)))
            //        {
            //            p = Convert.ToUInt64(txtImageP.Text);
            //        }
            //        else
            //        {
            //            txtImageP.Text = "";
            //            MessageBox.Show("Enter Prime Number");
            //            return;
            //        }
            //        if (RSACrypto.RSAMath.IsPrime(Convert.ToInt32(txtImageQ.Text)))
            //        {
            //            q = Convert.ToUInt64(txtImageQ.Text);
            //        }
            //        else
            //        {
            //            txtImageQ.Text = "";
            //            MessageBox.Show("Enter Prime Number");
            //            return;
            //        }

            //        EDForm.e = Convert.ToUInt64(txtImageE.Text);

            //        txtImageP.Enabled = false;
            //        txtImageQ.Enabled = false;
            //        txtImageE.Enabled = false;
            //        BtnSaveImageCipherAt.Enabled = true;
            //    }
            //}
            //else
            //{
            //    txtImageP.Text = "";
            //    txtImageQ.Text = "";
            //    txtImageE.Text = "";
            //    txtImageP.Enabled = true;
            //    txtImageQ.Enabled = true;
            //    txtImageE.Enabled = true;
            //    BtnSaveImageCipherAt.Enabled = false;
            //    //button3.Text = "Set Details";
            //}
        }

        private void BtnSetDetailsTextEncry_Click(object sender, EventArgs e)
        {
            //if (BtnSetDetailsTextEncry.Text == "Set Details")
            //{
            //    if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            //    {
            //        MessageBox.Show("Enter Valid Detail For RSA", "ERROR");
            //    }
            //    else
            //    {
            //        if (RSACrypto.RSAMath.IsPrime(Convert.ToInt32(textBox2.Text)))
            //        {
            //            p = Convert.ToUInt64(textBox2.Text);
            //        }
            //        else
            //        {
            //            textBox2.Text = "";
            //            MessageBox.Show("Enter Prime Number");
            //            return;
            //        }
            //        if (RSACrypto.RSAMath.IsPrime(Convert.ToInt32(textBox3.Text)))
            //        {
            //            q = Convert.ToUInt64(textBox3.Text);
            //        }
            //        else
            //        {
            //            textBox3.Text = "";
            //            MessageBox.Show("Enter Prime Number");
            //            return;
            //        }

            //        EDForm.e = Convert.ToUInt64(textBox4.Text);

            //        textBox2.Enabled = false;
            //        textBox3.Enabled = false;
            //        textBox4.Enabled = false;
            //        button4.Enabled = true;
            //        //button3.Text = "ReSet Details";
            //    }
            //}
            //else
            //{
            //    textBox2.Text = "";
            //    textBox3.Text = "";
            //    textBox4.Text = "";
            //    textBox2.Enabled = true;
            //    textBox3.Enabled = true;
            //    textBox4.Enabled = true;
            //    button4.Enabled = false;
            //    //button3.Text = "Set Details";
            //}
        }

        /// <summary>
        /// Choose a path to save the encrypted text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveCipherAt_Click(object sender, EventArgs e)
        {
            SaveCiper(txtSaveCipherAtPath);
            if (txtSaveCipherAtPath.Text != null)
            {
                BtnEncryptText.Enabled = true;
            }
        }

        /// <summary>
        /// Choose the cipher file to decrypt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSrcTextCipher_Click(object sender, EventArgs e)
        {
            OpenCipherFile(txtCipherFileText, BtnLoadCipherText);
        }

        private void BtnLoadCipherText_Click(object sender, EventArgs e)
        {
            cipherText = File.ReadAllText(txtCipherFileText.Text);
            MessageBox.Show("Cipher Loaded Successfully");
            //groupBox5.Enabled = true;
        }

        private void BtnSaveEncTextAt_Click(object sender, EventArgs e)
        {
            SaveDecryptedFile("Text File|*.txt;", txtSaveAtPath, BtnDecryptText);
        }


        /// <summary>
        /// Encrypt the selected text file, which is encoded using the unicode charset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEncryptText_Click(object sender, EventArgs e)
        {
            n = RSAlgorithm.N(p, q);
            BtnSelectImage.Enabled = false;
            DisableUI();

            //Get running time for encription
            Stopwatch TimerForEncription = new Stopwatch();
            TimerForEncription.Start();
            String encrypted = Encrypt(stringFile, progressBarEnc);
            TimerForEncription.Stop();
            //End running time calculation

            File.WriteAllText(txtSaveCipherAtPath.Text, encrypted);
            MessageBox.Show("Encryption Done");
            BtnSelectImage.Enabled = true;
            BtnLoadImage.Enabled = true;
            BtnSaveImageCipherAt.Enabled = true;

            BtnDecryptImage.Enabled = true;
            BtnSelectDecrytedImagePath.Enabled = true;
            BtnLoadImageCipher.Enabled = true;
            button10.Enabled = true;
            BtnEncryptImage.Enabled = true;
            GetRunningTime(TimerForEncription.ElapsedMilliseconds.ToString());
        }

        private void BtnDecryptText_Click(object sender, EventArgs e)
        {
            //button10.Enabled = false;
            DisableUI();

            //Get running time for encription
            Stopwatch TimerForEncription = new Stopwatch();
            TimerForEncription.Start();
            String decrypted = Decrypt(cipherText, progressBarTextDecr);
            TimerForEncription.Stop();

            var result = RSACrypto.RSAHelper.ConvertByteToText(RSACrypto.RSAHelper.DecodeHex(decrypted));
            File.WriteAllText(txtSaveAtPath.Text, result);
            MessageBox.Show("Decryption Done");
            Application.DoEvents();
            /*pictureBox1.Image.Save(txtDecryptedImagePath.Text, System.Drawing.Imaging.ImageFormat.Jpeg);
            MessageBox.Show("Picture Saved");

            BtnDecryptImage.Enabled = true;
            BtnSelectDecrytedImagePath.Enabled = true;
            BtnLoadImageCipher.Enabled = true;
            button10.Enabled = true;

            BtnSelectImage.Enabled = true;
            BtnLoadImage.Enabled = true;
            BtnSaveImageCipherAt.Enabled = true;*/
            GetDecriptionResult(TimerForEncription.ElapsedMilliseconds.ToString());
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            cipherText = File.ReadAllText(txtSrcImageCipher.Text);
            MessageBox.Show("Cipher Loaded Successfully");
            groupBox5.Enabled = true;
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            SaveDecryptedFile("Image File|*.jpg;", txtDecryptedImagePath, BtnDecryptImage);
        }

        private void SaveDecryptedFile(string filter, TextBox textbox, Button button)
        {
            SaveFileDialog save1 = new SaveFileDialog
            {
                Filter = filter
            };
            if (save1.ShowDialog() == DialogResult.OK)
            {
                textbox.Text = save1.FileName;
                button.Enabled = true;
            }
            else
            {
                textbox.Text = "";
                button.Enabled = false;
            }
        }

        private void BtnGetRSAElements_Click(object sender, EventArgs e)
        {
            ulong N;
            if (xtraTabControl1.SelectedTabPage == xtraTabControl1.TabPages[0])
            {
                var result = RSAMath.PopulateKeys(EDForm.e, d, p, q, txtImageP, txtImageQ, txtImageN, txtImageE, txtImageD);
                p = Convert.ToUInt64(result[0]);
                q = Convert.ToUInt64(result[1]);
                N = Convert.ToUInt64(result[2]);
                EDForm.e = Convert.ToUInt64(result[3]);
                d = Convert.ToUInt64(result[4]);
            }
            else
            {
                var result = RSAMath.PopulateKeys(EDForm.e, d, p, q, txtPTextEncr, txtQTextEncr, txtNText, txtETextEncry, txtDText);
                p = Convert.ToUInt64(result[0]);
                q = Convert.ToUInt64(result[1]);
                N = Convert.ToUInt64(result[2]);
                EDForm.e = Convert.ToUInt64(result[3]);
                d = Convert.ToUInt64(result[4]);
            }

            //Set Public and Private Key
            //textBox4.Text = EDForm.e.ToString();
            //textBox9.Text = d.ToString();

            if (MakerCheckerMessageMemoEdit.Text.Trim() == "")
            {
                MakerCheckerMessageMemoEdit.Text = "P=" + p + ", Q=" + q + ", N=" + N + System.Environment.NewLine + "Public Key=[" + EDForm.e.ToString() + "," + N + "]" + System.Environment.NewLine + "Private Key=[" + d.ToString() + ", " + N + "]";
            }
            else
            {
                MakerCheckerMessageMemoEdit.Text = MakerCheckerMessageMemoEdit.Text + System.Environment.NewLine + "_______________________________________________" + System.Environment.NewLine + "P=" + p + ", Q=" + q + ", N=" + N + System.Environment.NewLine + "Public Key=[" + EDForm.e.ToString() + "," + N + "]" + System.Environment.NewLine + "Private Key=[" + d.ToString() + ", " + N + "]";
            }

            Application.DoEvents();
        }

        /// <summary>
        /// Load and Encode the content of the text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoadText_Click(object sender, EventArgs e)
        {
            BtnSaveCipherAt.Enabled = true;
            stringFile = BitConverter.ToString(RSAHelper.ConvertTextToByte(ReadFile(TextFfilePath.Text) + " "));
            MessageBox.Show("Text Loaded Successfully");
            groupBox7.Enabled = true;
        }
    }
}

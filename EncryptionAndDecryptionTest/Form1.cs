using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Edata.CommonLibrary;
using Edata.IPVoyager;

namespace EncryptionAndDecryptionTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonE_Click(object sender, EventArgs e)
        {
            byte[] baKey = EncryptionAndDecryption.CreatedKey();

            byte[] baBuffer = EncryptionAndDecryption.Encryption(textBox1.Text);
            textBox2.Text  = Convert.ToBase64String(baBuffer);            
        }

        private void buttonD_Click(object sender, EventArgs e)
        {
            byte[] baKey = EncryptionAndDecryption.CreatedKey();
            textBox1.Text = EncryptionAndDecryption.Decryption(textBox2.Text);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XmlLoadAndGenerationFlatFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //XmlReader reader = XmlReader.Create("D:/XmlLoadAndGenerationFlatFile/XmlLoadAndGenerationFlatFile/TEST_SPECIFICATION.xml");
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            String path = "D:/XmlLoadAndGenerationFlatFile/XmlLoadAndGenerationFlatFile/TEST_SPECIFICATION.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
           // XPathNavigator navigator = doc.CreateNavigator();
            richTextBox1.Text = Xml.GetChild(doc.ChildNodes).ToString();           
        }   
    }
}
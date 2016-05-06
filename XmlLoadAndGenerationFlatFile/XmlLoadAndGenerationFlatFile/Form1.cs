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
        private TreeNode tNode;
        public Form1()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// This function open xml file and treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open XML Document";
            dlg.Filter = "XML Files (*.xml)|*.xml";
            dlg.FileName = Application.StartupPath + "\\..\\..\\example.xml";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(dlg.FileName);
                    treeView1.Nodes.Clear();
                    treeView1.Nodes.Add(new TreeNode(xDoc.DocumentElement.Name));
                    tNode = new TreeNode();
                    tNode = (TreeNode)treeView1.Nodes[0];
                    addTreeNode(xDoc.DocumentElement, tNode);
                    treeView1.ExpandAll();
                }
                catch (XmlException xExc)
                {
                    MessageBox.Show(xExc.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }

           // XmlTextReader reader = new XmlTextReader(dlg.FileName);
            readerXml(dlg.FileName);
            
}

        void readerXml(string dlg)
        {
            treeView2.Nodes.Clear();
            XElement root = XElement.Load(dlg);
            XDocument doc = XDocument.Load(dlg);
            XmlDocument docs = new XmlDocument();
            docs.Load(dlg);
           
            foreach (XElement el in root.Elements())
            {
                if (el.Attribute("name") != null && el.Attribute("name").Value == root.FirstAttribute.Value)
                {
                    treeView2.Nodes.Add(root.FirstAttribute.Value).ForeColor = Color.Green;
                }

               // reader(el);       
            }
            //foreach (XElement el in doc.Root.Elements())
            //{
            //        //Console.WriteLine("{0}", el.Name.LocalName);
                             
            //    Console.WriteLine("  Elements:");
           
            //    foreach (XElement element in el.Elements())
            //    {
            //        Console.WriteLine("    {0}", element.FirstAttribute.Value);
            //        //foreach (XAttribute attr in el.Attributes())
            //        //Console.WriteLine("    {0}", attr.Value);
            //    }
            //}
            AddNodes(XElement.Load(dlg));
        }

        void AddNodes(XElement parentElement, TreeNode parent = null)
        {
            Queue<XElement> queue = new Queue<XElement>(parentElement.Elements());
            while (queue.Count > 1)
            {
                TreeNode child = parent;
                XElement element = queue.Dequeue();
                if (!element.HasElements)
                {
                    string value = "";
                    //if(element.FirstAttribute.Value!=null)
                    //value =""+ element.FirstAttribute.Value; 
                    element = (XElement)element.NextNode;
                    if (element != null && !element.HasElements) value = element.FirstAttribute.Value;

                    if (parent == null) { value = element.FirstAttribute.Value; treeView2.Nodes.Add(child = new TreeNode(value)); }
                    else parent.Nodes.Add(child = new TreeNode(value));
                    child.Expand();
                    element = queue.Dequeue();
                }
                AddNodes(element, child);
            }
        }

//----------------------------------------------------------------------------------------------------        
     
        /// <summary>
        /// This function is called recursively until all nodes are loaded
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="treeNode"></param>
        private void addTreeNode(XmlNode xmlNode, TreeNode treeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList xNodeList;

            if (xmlNode.HasChildNodes)
            {
                xNodeList = xmlNode.ChildNodes;

                for (int x = 0; x <= xNodeList.Count - 1; x++)
                {
                    xNode = xmlNode.ChildNodes[x];
                    treeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = treeNode.Nodes[x];
                    addTreeNode(xNode, tNode);
                }
            }
            else 
                treeNode.Text = xmlNode.OuterXml.Trim();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            richTextBox1.Clear();
                foreach (var item in treeView1.SelectedNode.Nodes)
                {
                    richTextBox1.Text += item.ToString().Replace("TreeNode:", "") + "\n";
                }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //SaveFileDialog dlg = new SaveFileDialog();
            //dlg.FileName = this.treeView1.Nodes[0].Text + ".xml";
            //dlg.Filter = "XML Files (*.xml)|*.xml";			
        }
    }
}
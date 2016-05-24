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
        XElement root;
        public Form1()
        {
            InitializeComponent();
        }
        

        void readerXml(string dlg)
        {
            treeView2.Nodes.Clear();

            root = XElement.Load(dlg);

            XDocument doc = XDocument.Load(dlg);

            XmlDocument docs = new XmlDocument();

            docs.Load(dlg);


            foreach (XElement el in root.Elements())
            {

                if (el.Attribute("name") != null && el.Attribute("name").Value == root.FirstAttribute.Value)
                {

                    treeView2.Nodes.Add(root.FirstAttribute.Value).ForeColor = Color.Green;
                    AddNodes(root, treeView2.Nodes[0]);
                }
                                  
            }
        }

        void AddNodes(XElement parentElement, TreeNode parent = null)
        {
            Queue<XElement> queue = new Queue<XElement>(parentElement.Elements());

            if (queue.Count > 0)
            {

                TreeNode child = parent;

                XElement element = queue.Dequeue();

                while (queue.Count > 0)
                {

                    string value = "";

                    element = (XElement)element.NextNode;

                    value = element.FirstAttribute.Value;

                    if (parent.Text != value)
                    {

                        if (element != null && !element.HasElements) value = element.FirstAttribute.Value;

                        if (parent == null) { value = element.FirstAttribute.Value; treeView2.Nodes.Add(child = new TreeNode(value)); }

                        else parent.Nodes.Add(child = new TreeNode(value));

                        child.Expand();



                        AddNodes(element, child);

                    }

                    element = queue.Dequeue();

                }

            }

        }

        private TreeNode XmlNode2TreeNode(XmlNode parentNode)
        {
            TreeNode treeNode = new TreeNode();

            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    TreeNode childTreeNode = new TreeNode();

                    if (childNode.ChildNodes.Count > 0)
                        childTreeNode = XmlNode2TreeNode(childNode);

                    childTreeNode.Text = FormatName(childNode);
                    GetAttributes(childNode, childTreeNode);
                    treeNode.Nodes.Add(childTreeNode);
                }
            }

            return treeNode;
        }

        private string FormatName(XmlNode node)
        {
            string fullName = "<" + node.Name;

            if (node.InnerText != "")
                fullName += ">" + node.InnerText + "</" + node.Name + ">";
            else
                fullName += ">";

            return fullName;
        }

        private void GetAttributes(XmlNode node, TreeNode treeNode)
        {
            if (node.Attributes != null)
            {
                ListViewItem[] attributes = new ListViewItem[node.Attributes.Count];

                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    attributes[i] = new ListViewItem(new string[] { node.Attributes[i].Name, node.Attributes[i].Value });
                }

                treeNode.Tag = attributes;
            }
        }
        private void xmlTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            listView1.Items.Clear();

            if (e.Node.Tag != null)
            {
                listView1.Items.AddRange((ListViewItem[])e.Node.Tag);
            }
        }

        private void xmlTreeView2_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            listView1.Items.Clear();
            
            foreach (XElement el in root.Elements())
            {
                if (el.HasAttributes && el.FirstAttribute.Value == e.Node.Text)
                {
                    if (el.Attributes() != null)
                    {
                        ListViewItem[] attributes = new ListViewItem[el.Attributes().Count()];
                        int i = 0;
                        foreach (var item in el.Attributes())
                        {
                            attributes[i] = new ListViewItem(new string[] { item.Name.ToString(), item.Value });
                            i++;
                        }

                        listView1.Items.AddRange(attributes);
                    }
                    break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //SaveFileDialog dlg = new SaveFileDialog();
            //dlg.FileName = this.treeView1.Nodes[0].Text + ".xml";
            //dlg.Filter = "XML Files (*.xml)|*.xml";			
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open XML Document";
            dlg.Filter = "XML Files (*.xml)|*.xml";
            dlg.FileName = Application.StartupPath + "\\..\\..\\TEST_SPECIFICATION.xml";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    treeView1.Nodes.Clear();

                    XmlDocument document = new XmlDocument();
                    document.Load(dlg.FileName);   //load the XMl file

                    XmlNodeList nodeList = document.DocumentElement.ChildNodes;

                    //loop through the top nodes
                    foreach (XmlNode node in document)
                    {
                        TreeNode treeNode = XmlNode2TreeNode(node);
                        GetAttributes(node, treeNode);
                        treeNode.Text = FormatName(node);
                        treeView1.Nodes.Add(treeNode);
                    }

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
            if (dlg.FileName != null)
                readerXml(dlg.FileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void saveFlatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "XML Files (*.xml)|*.xml";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                exportToXml(treeView2, saveFileDialog1.FileName);
            }            
        }

        private void saveXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// /////////////////////////////////
        /// </summary>
        private StreamWriter sr;

        public void exportToXml(TreeView tv, string filename)
        {
            sr = new StreamWriter(filename);
            sr.WriteLine("<" + treeView2.Nodes[0].Text + ">");
            foreach (TreeNode node in tv.Nodes)
            {
                saveNode(node.Nodes);
            }
            //Close the root node
            sr.WriteLine("</" + treeView2.Nodes[0].Text + ">");
            sr.Close();
        }

        private void saveNode(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                if (node.Nodes.Count > 0)
                {
                    sr.WriteLine("<" + node.Text + ">");
                    saveNode(node.Nodes);
                    sr.WriteLine("</" + node.Text + ">");
                }
                else //No child nodes, so we just write the text
                    sr.WriteLine(node.Text);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Meta meta;
        Dictionary<string, List<string>> dict = null;
        //  string filepath;
        string file_name;
        string fullFileName;
        string files_path = @"E:\ANSYSDev\XMLTool\MetadataUI\data";
        string selected_file;
        List<string> level1;
        List<string> level2;

        public Form1()
        {
            InitializeComponent();

            treeView1.AfterCheck += new TreeViewEventHandler(treeView1_AfterCheck);
            treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(DataGridView1_CellValueChanged);
            // Bind the DataGridView to the BindingSource
            // and load the data from the database.
            // dataGridView1.DataSource = bindingSource1;
            //   GetData("select * from Customers");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            populateTreeView();
        }

        private void load_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            var result = fd.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = fd.FileName;
                meta = new Meta(path);
            }
            comboBox1.SelectedIndexChanged +=
            new System.EventHandler(comboBox1_SelectedIndexChanged);

            //  string path = "F:\\ANSYSDev\\XML\\data\\metadataTable.xml";
            //filepath = path;
            //meta = new Meta(path);
            //  textBox1.Text = meta.metaDict.Keys.Count.ToString();

            List<string> metalevel1 = new List<string>(meta.metaDict.Keys);
            comboBox1.DataSource = metalevel1;

        }

        private void comboBox1_SelectedIndexChanged(object sender,
        System.EventArgs e)
        {
            string selected = comboBox1.Text;
            Debug.WriteLine("combox select changed to ===" + selected);

            List<string> metalevel2 = new List<string>(meta.metaDict[selected]);
            comboBox2.DataSource = metalevel2;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //lookup button
        private void button1_Click(object sender, EventArgs e)
        {
            if (meta == null)
            {
                MessageBox.Show("Load a Xml file first.");
                return;
            }
            fullFileName = textBox1.Text;
            Debug.WriteLine(fullFileName);

            //   filename = "aim_boundary_conditions\\structural\\topics\\h_cht_insulated.dita";
            dict = meta.lookup(fullFileName);

            if (dict == null) { MessageBox.Show("not found"); return; }


            level1 = new List<string>(dict.Keys);
            listBox1.DataSource = level1;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = listBox1.SelectedItem.ToString();
            if (dict == null) return;

            level2 = new List<string>(dict[selectedItem]);

            // Debug.WriteLine( level2.ToString());
            listBox2.DataSource = level2;

            //CurrencyManager cm = (CurrencyManager)BindingContext[level2];
            //cm.Refresh();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            string selected1 = comboBox1.Text;
            string selected2 = comboBox2.Text;
            if (meta == null) return;
            if (selected2 == null || selected2.Trim() == "") return;

            meta.fileNode.SetDictObject(selected1, selected2);

            level1 = new List<string>(meta.fileNode.fileMetaDict.Keys);
            listBox1.DataSource = level1;
            CurrencyManager cm1 = (CurrencyManager)BindingContext[level1];
            CurrencyManager cm2 = (CurrencyManager)BindingContext[level2];
            cm1.Refresh();
            cm2.Refresh();

            //update metatable
            meta.addNode(selected1, selected2, fullFileName);
            populateGridView(meta.fileNode.fileMetaDict);
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            string selected1 = listBox1.Text;
            string selected2 = listBox2.Text;
            if (meta == null) return;
            if (selected2 == null || selected2.Trim() == "") return;

            meta.fileNode.removeDictObject(selected1, selected2);

            List<string> level1 = new List<string>(meta.fileNode.fileMetaDict.Keys);
            listBox1.DataSource = level1;

            meta.deleteNode(selected1, selected2, fullFileName);
            populateGridView(meta.fileNode.fileMetaDict);
        }

        private void save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Xml (*.xml)|*.xml";
            var result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                meta.save(saveFileDialog.FileName);
            }
        }

        void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //selectedPaths.Clear();

            //foreach (TreeNode node in treeView1.Nodes)
            //{
            //    findSelectedNodes(node);
            //}

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
         file_name = node.FullPath.Split(Path.DirectorySeparatorChar).Last();
          if (file_name.EndsWith(".dita"))
       {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            dataGridView1.RowCount = 10;
            dataGridView1.ColumnCount = 5;
            dataGridView1.AutoGenerateColumns = false;
            if (meta == null)
            {
                MessageBox.Show("Load a XML file first;");
                return;
            }
                dict = meta.lookup(file_name, false);
                fullFileName = meta.findFullFileName(file_name);
                populateGridView(dict);

                level1 = new List<string>(dict.Keys);
                listBox1.DataSource = level1;

            }
   
        }

        private void populateGridView(Dictionary<string, List<string>> dict)
        {

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            dataGridView1.RowCount = 10;

            int col_num = 1;
            foreach (KeyValuePair<string, List<string>> kvp in meta.metaDict)
            {
                DataGridViewComboBoxCell bc = new DataGridViewComboBoxCell();
                bc.DataSource = kvp.Value;
                DataGridViewColumn cc = new DataGridViewColumn(bc);
                cc.Width = 200;
                dataGridView1.Columns.Add(cc);
                cc.HeaderText = kvp.Key;

                int row_num = 0;
                if (dict.Keys.Contains(kvp.Key))
                {
                    foreach (string metadataValue in dict[kvp.Key])
                    {
                        dataGridView1.Rows[row_num].Cells[col_num].Value = metadataValue;
                        row_num++;
                    }
                }

                col_num++;
            }
            dataGridView1.Columns[0].Visible = false;

        }



        private void populateTreeView()
        {
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(CreateDirectoryNode(new DirectoryInfo(files_path)));

        }

        private TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            TreeNode directoryNode = new TreeNode(directoryInfo.Name);
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                directoryNode.Expand();
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            }

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Name.StartsWith("reusable") || file.Name.StartsWith("u_"))
                {
                    continue;
                }
                else
                {
                    if (file.Name.EndsWith(".dita"))
                    {

                        TreeNode fileNode = new TreeNode(file.Name);

                        //   fileNode.Tag = file.FullName.Replace(path + Path.DirectorySeparatorChar, "");
                        //  fileNode.Tag = file.FullName.Replace(path, "");

                        directoryNode.Nodes.Add(fileNode);
                    }

                }

            }


            //foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            //{
            //    directoryNode.Nodes.Add(CreateDirectoryNode(directory));

            //    if (directory.Name.StartsWith("aim_"))
            //    {
            //        directoryNode.Expand();
            //    }

            //    if (directory.Name.Contains(".svn") || directory.Name.Contains("images"))
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            //    }
            //}

            //foreach (FileInfo file in directoryInfo.GetFiles())
            //{
            //    if (file.Name.StartsWith("reusable") || file.Name.StartsWith("u_"))
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        if (file.Name.EndsWith(".dita"))
            //        {

            //            TreeNode fileNode = new TreeNode(file.Name);

            //            //   fileNode.Tag = file.FullName.Replace(path + Path.DirectorySeparatorChar, "");
            //            fileNode.Tag = file.FullName.Replace(files_path, "");

            //            directoryNode.Nodes.Add(fileNode);
            //        }

            //    }

            //}

            return directoryNode;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellValueChanged(
    object sender, DataGridViewCellEventArgs e)
        {
        }

        private void OK_Click(object sender, EventArgs e)
        {

            if (meta == null) return;
        //    Dictionary<string, List<string>> newDict = new Dictionary<string, List<string>>();

            for (int i = 1; i < dataGridView1.Columns.Count; i++) {
            //    List<string> list = new List<string>();
                string key = dataGridView1.Columns[i].HeaderText;

                if ( key == null || key == string.Empty) continue;

                for (int j = 0; j < dataGridView1.Rows.Count; j++) {
                    string value = (string)dataGridView1.Rows[j].Cells[i].Value;
                    if (value == null || value == string.Empty) {  // delete node
                        //if (meta.fileNode.fileMetaDict.Keys.Contains(key))
                        //{
                        //    List<string> ls = meta.fileNode.fileMetaDict[key];
                        //    if (j < ls.Count)
                        //        Debug.WriteLine(ls[j]);
                        //}
                    } else {
                   //     list.Add(value);

                     

                        if (meta.fileNode.SetDictObject(key, value)) {
                            fullFileName = meta.findFullFileName(file_name);
                            meta.addNode(key, value, fullFileName);
                        
                        };
                    
                    }
                }
               // newDict.Add(key, list);
            }


         //   meta.fileNode.fileMetaDict = newDict;
            level1 = new List<string>(meta.fileNode.fileMetaDict.Keys);
            listBox1.DataSource = level1;
        }



    }
}

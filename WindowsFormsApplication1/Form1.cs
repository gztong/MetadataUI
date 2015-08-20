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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Meta meta;
        Dictionary<string, List<string>> dict = null;
        string filepath;
        string filename;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
          filename =  textBox1.Text;
          Debug.WriteLine(filename);

       //   filename = "aim_boundary_conditions\\structural\\topics\\h_cht_insulated.dita";
          dict =  meta.lookup(filename);

          if (dict == null) { MessageBox.Show("not found"); return; }


          List<string> level1 = new List<string>(dict.Keys);
          listBox1.DataSource = level1;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = listBox1.SelectedItem.ToString();
            if (dict == null) return;

            Debug.WriteLine("dict is not null");
            List<string> level2 = new List<string>(dict[selectedItem]);

            Debug.WriteLine( level2.ToString());
            listBox2.DataSource = level2;

        }

        private void Add_Click(object sender, EventArgs e)
        {
            string selected1 = comboBox1.Text;
            string selected2 = comboBox2.Text;
            if (meta == null) return;
            if (selected2 == null || selected2.Trim() == "") return;

            meta.fileNode.SetDictObject(selected1, selected2);

            List<string> level1 = new List<string>(meta.fileNode.fileMetaDict.Keys);
            listBox1.DataSource = level1;
            

            //update metatable
            meta.addNode(selected1, selected2, filename);

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

            meta.deleteNode(selected1, selected2, filename);

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




    }
}

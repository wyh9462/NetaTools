using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetaTools
{
    public partial class ReplaceFiles : Form
    {
        public ReplaceFiles()
        {
            InitializeComponent();
        }

        private void ReplaceFiles_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
            Main form = new Main();
            form.Show();
        }

        /// <summary>
        /// Clear
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            textBox2.Text = null;
            textBox3.Text = null;
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            List<string> extFilters = textBox1.Text.Split(',').Select(x => x.Replace("*", "").ToLower()).ToList();

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    listBox1.Items.Add(file);
                }
                if (Directory.Exists(file))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(file);
                    var dirinfoList = new List<FileSystemInfo>();
                    var dirinfoList2 = new List<FileSystemInfo>();

                    Service.GetAllDirectoryAndFiles(dirInfo, ref dirinfoList, extFilters);
                    dirinfoList2 = dirinfoList.Where(x => !(x is DirectoryInfo) && !(x is FileInfo && extFilters.Contains(Path.GetExtension(x.FullName.ToLower()))) && !x.FullName.Contains("~$")).ToList();
                    dirinfoList = dirinfoList.Where(x => !(x is DirectoryInfo) && !(x is FileInfo && !extFilters.Contains(Path.GetExtension(x.FullName.ToLower()))) && !x.FullName.Contains("~$")).ToList();

                    listBox1.Items.AddRange(dirinfoList.Select(x => x.FullName).ToArray());
                    listBox2.Items.AddRange(dirinfoList2.Select(x => x.FullName).ToArray());
                }
            }

            Service.DistinctListBox(listBox1.Items);
            Service.DistinctListBox(listBox2.Items);
        }

        private void listBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void listBox2_DragDrop(object sender, DragEventArgs e)
        {
            listBox1_DragDrop(sender, e);
        }

        /// <summary>
        /// Save
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("需输入要被替换的值");
                    return;
                }

                int replaceNum = 0;
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    string item = (string)listBox1.Items[i];
                    Service.DoReplaceFiles(item, textBox2.Text, textBox3.Text);
                    replaceNum++;
                }

                MessageBox.Show($"共处理文件{replaceNum}个");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("需输入要被替换的值");
                    return;
                }

                int renameNum = 0;
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    string item = (string)listBox1.Items[i];
                    if (item.Contains(textBox2.Text))
                    {
                        string newFileName = Service.DoReplaceFileName(item, textBox2.Text, textBox3.Text);
                        listBox1.Items[i] = newFileName;
                        renameNum++;
                    }
                }

                for (int i = 0; i < listBox2.Items.Count; i++)
                {
                    string item = (string)listBox2.Items[i];
                    if (item.Contains(textBox2.Text))
                    {
                        string newFileName = Service.DoReplaceFileName(item, textBox2.Text, textBox3.Text);
                        listBox2.Items[i] = newFileName;
                        renameNum++;
                    }
                }

                MessageBox.Show($"更改文件名{renameNum}个");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}

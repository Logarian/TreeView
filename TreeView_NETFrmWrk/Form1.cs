using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace TreeView_NETFrmWrk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private SqlCommand cmd;
        private SqlConnection con;
        private string constring = @"Data Source=erbd38;Integrated Security=SSPI;Trusted_Connection=yes";

        public string ReadFromFile(string fName)
        {
            string res;
            try
            {
                StreamReader reader = new StreamReader(fName, Encoding.Default);
                res = reader.ReadToEnd();
                reader.Close();
                return res;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Ошибка чтения файла:\n" + ex.Message);
                return "";
            }
        }

        private void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                directoryNode.Nodes.Add(new TreeNode(file.Name.Replace(".sql", "")));
            }

            return directoryNode;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListDirectory(treeView1, Environment.CurrentDirectory + "\\Запросы");
        }

        
        public void disp_data()
        {
            string pth = treeView1.SelectedNode.FullPath + ".sql";

            con = new SqlConnection(constring);
            con.Open();
            cmd = new SqlCommand(ReadFromFile(Environment.CurrentDirectory + "\\" + pth), con);
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ListDirectory(treeView1, Environment.CurrentDirectory + "\\" + "Запросы");
            MessageBox.Show(treeView1.SelectedNode.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            disp_data();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string pth = treeView1.SelectedNode.FullPath + ".sql";

            con = new SqlConnection(constring);
            con.Open();
            cmd = new SqlCommand(ReadFromFile(Environment.CurrentDirectory + "\\" + pth), con);
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            con.Close();

            

            if (dt.Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                //excelApp.Application.Workbooks.Add(Type.Missing);

                //excelApp.Visible = false;
                //excelApp.ScreenUpdating = false;

                Microsoft.Office.Interop.Excel.Workbook ExWorkBook = excelApp.Workbooks.Open(Directory.GetCurrentDirectory() + "\\" + "1.xlsx", Type.Missing, false, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, false);

                #region Рисуем границы вокруг ячеек
                void cellsBorders(int i, int j)
                {
                    Range cell = excelApp.Cells[i, j];
                    Borders border = cell.Borders;
                    border.LineStyle = XlLineStyle.xlContinuous;
                    border.Weight = 2d;
                }
                #endregion

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    excelApp.Cells[1, i + 1] = dt.Columns[i].ToString();
                    //excelApp.Cells[1, i + 1].Font.Size = 14;
                    excelApp.Cells[1, i + 1].Columns.AutoFit();
                    cellsBorders(1, i + 1);
                }

                excelApp.Range[excelApp.Cells[1, 1], excelApp.Cells[1, dt.Columns.Count - 1]].HorizontalAlignment = XlHAlign.xlHAlignCenter;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        excelApp.Cells[i + 2, j + 1] = dt.Rows[i][j].ToString();
                        excelApp.Cells[i + 2, j + 1].Style.WrapText = true;
                        cellsBorders(i + 2, j + 1);
                    }
                }

                excelApp.Range[excelApp.Cells[2, 1], excelApp.Cells[dt.Rows.Count + 1, dt.Columns.Count]].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excelApp.Range[excelApp.Cells[2, 1], excelApp.Cells[dt.Rows.Count + 1, dt.Columns.Count]].VerticalAlignment = XlVAlign.xlVAlignCenter;
                //excelApp.Range[excelApp.Cells[2, dt.Columns.Count - 1], excelApp.Cells[dt.Rows.Count + 1, dt.Columns.Count - 1]].HorizontalAlignment = XlHAlign.xlHAlignLeft;

                ExWorkBook.SaveAs(Environment.CurrentDirectory + "\\" + treeView1.SelectedNode.Text + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();

                //Проверить как ведет себя сохраненный файл без этих строк - ничего не поменялось, но оставлю
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ExWorkBook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                MessageBox.Show("Файл сформирован!");
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            label1.Text = "";
            label1.Text += "Выбран раздел: " + treeView1.SelectedNode.FullPath.Replace("\\", " - ");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string pth = treeView1.SelectedNode.FullPath + ".sql";

            con = new SqlConnection(constring);
            con.Open();
            cmd = new SqlCommand(ReadFromFile(Environment.CurrentDirectory + "\\" + pth), con);
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            con.Close();

            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

            Microsoft.Office.Interop.Excel.Workbook ExWorkBook = excelApp.Workbooks.Open(Directory.GetCurrentDirectory() + "\\" + "1.xlsx", Type.Missing, false, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, false);

            Microsoft.Office.Interop.Excel.Worksheet ExWorkSheet;
            ExWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExWorkBook.Worksheets.get_Item(1);

            object[,] arrHeader = new object[1, dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                arrHeader[0, i] = dt.Columns[i].ToString();
            }

            object[,] arr = new object[dt.Rows.Count, dt.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    arr[i, j] = dr[j];
                }
            }

            Microsoft.Office.Interop.Excel.Range cHeader1 = (Microsoft.Office.Interop.Excel.Range)ExWorkSheet.Cells[1, 1];
            Microsoft.Office.Interop.Excel.Range cHeader2 = (Microsoft.Office.Interop.Excel.Range)ExWorkSheet.Cells[1, dt.Columns.Count];
            Microsoft.Office.Interop.Excel.Range rangeHeader = ExWorkSheet.get_Range(cHeader1, cHeader2);
            rangeHeader.Value = arrHeader;

            Microsoft.Office.Interop.Excel.Range c1 = (Microsoft.Office.Interop.Excel.Range)ExWorkSheet.Cells[2, 1];
            Microsoft.Office.Interop.Excel.Range c2 = (Microsoft.Office.Interop.Excel.Range)ExWorkSheet.Cells[2 + dt.Rows.Count - 1, dt.Columns.Count];
            Microsoft.Office.Interop.Excel.Range range = ExWorkSheet.get_Range(c1, c2);
            range.Value = arr;

            Microsoft.Office.Interop.Excel.Range bordRange = ExWorkSheet.get_Range(cHeader1, c2);
            bordRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            bordRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            bordRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            bordRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

            bordRange.Columns.AutoFit();

            bordRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            bordRange.VerticalAlignment = XlVAlign.xlVAlignCenter;

            ExWorkBook.SaveAs(Environment.CurrentDirectory + "\\" + treeView1.SelectedNode.Text + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            excelApp.Quit();

            //Проверить как ведет себя сохраненный файл без этих строк - ничего не поменялось, но оставлю
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ExWorkSheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ExWorkBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

            MessageBox.Show("Файл сформирован!");
        }
    }
}

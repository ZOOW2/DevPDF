using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace DevExpressTestPDF
{
    public partial class SearchPDF : DevExpress.XtraEditors.XtraForm
    {
        public SearchPDF()
        {
            InitializeComponent();
            searchControl1.TextChanged += SearchControl1_TextChanged;
        }

        private void SearchControl1_TextChanged(object sender, EventArgs e)
        {
            string fileNumber = searchControl1.Text;
            if (!string.IsNullOrWhiteSpace(fileNumber))
            {
                SearchAndDisplayFile(fileNumber);
            }
        }

        // Метод для поиска файла по номеру и вывода данных
        private void SearchAndDisplayFile(string fileNumber)
        {
            string connectionString = "server=localhost;user=root;password=root;database=pdf";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Path FROM info WHERE ID = @ID";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", fileNumber);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string filePath = reader.GetString("Path");

                                if (File.Exists(filePath))
                                {
                                    string fileText = ExtractTextFromPdf(filePath);

                                    memoEdit2.Text = "File Content: " + Environment.NewLine + "-------------------------------------------" + Environment.NewLine;
                                    memoEdit2.AppendText(fileText);
                                }
                                else
                                {
                                    memoEdit2.Text = "File not found.";
                                }
                            }
                        }
                        else
                        {
                            memoEdit2.Text = "No data found for this number.";
                        }
                    }
                }
            }
        }

        private string ExtractTextFromPdf(string filePath)
        {
            StringBuilder text = new StringBuilder();
            using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(filePath))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    string pageText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);

                    text.AppendLine(pageText.Trim());
                }
            }
            return text.ToString();
        }
    }
}

using Head.Common.Generate;
using System.Windows.Forms;
using Common.Logging;
using Head.Common.Csv;
using System.Configuration;

namespace WinGUI
{
    public partial class MainWindow : Form
    {
        static ILog Logger = LogManager.GetCurrentClassLogger();


        public MainWindow()
        {
            Logger.Info("Win GUI Started");

            InitializeComponent();

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["datapath"].ToString()))
                CsvImporter.CsvPath = ConfigurationManager.AppSettings["datapath"].ToString();

            var categories =
                new CategoryCreator()
                    .SetRawPath("Events.csv")
                    .SetOverrideFactory("Resources/events.json")
                    .Create();

            dataGridView1.DataSource = categories;

        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

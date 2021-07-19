using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Projet_2020
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //Initialisation de la fenêtre
            InitializeComponent();
            //Acquisition des ports COM et affichage pour permettre à l'utilistateur de selectionner le port souhaité
            string[] serialPort = SerialPort.GetPortNames();

            foreach(string s in serialPort)
            {
                comboBox1.Items.Add(s);
            }
            //Permet de récuperer les paramètres sauvegardés par l'utilisateur (ou affichage des parametres par défaut si aucun trouvé)
            if(Properties.Settings.Default.portName != "")
            {
                comboBox1.Text = Properties.Settings.Default.portName;
            }

            if (Properties.Settings.Default.baudsRate != 0)
            {
                numericUpDown1.Value = Properties.Settings.Default.baudsRate;
            }

            if (Properties.Settings.Default.tempUnit != "")
            {
                comboBox2.Text = Properties.Settings.Default.tempUnit;
            }
        }

        private void button1_Click(object sender, EventArgs e) //Bouton ouvrir port
        {
            //Ajoute une fonction personnalisée quand l'évenement DataReceived sera déclanché (en cas de données reçues)
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Configuration du port avec les parametres selectionnés par l'utilisateur
            serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = Decimal.ToInt32(numericUpDown1.Value);
            //On ouvre le port
            serialPort1.Open();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e) //Si des données sont reçues
        {
            //On récupere les données dans une variable de type string
            string data = serialPort1.ReadLine();
            //On appel la fonction UpdateUI et on lui donne la variable contenant les données
            UpdateUI(data);
        }

        public void UpdateUI(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateUI), new object[] { value });
                return;
            }
            //On supprime les caractères vides
            string result = value.Replace(" ", "");
            //On récupere les differentes données
            string[] data = result.Split('|');
            string adcValue = Regex.Replace(data[1], @"[^\d]", "");
            //On les affiches
            switch (data[0])
            {
                case "0":

                    circularProgressBar1.Value = int.Parse(adcValue);
                    break;

                case "1":
                    circularProgressBar2.Value = int.Parse(adcValue);
                    break;

                case "2":
                    circularProgressBar3.Value = int.Parse(adcValue);
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e) //Bouton fermer port
        {
            //On ferme le port
            serialPort1.Close();
            circularProgressBar1.Value = 0;
            button2.Enabled = false;
            button1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e) //Bouton enregistrer
        {
            //Sauvegarde les parametres définis par l'utilisateur
            Properties.Settings.Default.portName = comboBox1.Text;
            Properties.Settings.Default.baudsRate = Decimal.ToInt32(numericUpDown1.Value);
            Properties.Settings.Default.tempUnit = comboBox2.Text;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }
    }
}

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

namespace ProgramBackpropagation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int totalKolom;

        public List<List<String>>atribut;
        public List<String> kelas;

        double learningRate;
        int jumlahHiddenLayer;
        int jumlahNeuronHiddenLayer;
        int maksIterasi;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile1 = new OpenFileDialog();
            openfile1.Filter = "File UCI data set (*.data)|*.data";
            if (openfile1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Refresh();

                string[] lines = File.ReadAllLines(openfile1.FileName);

                atribut = new List<List<string>>();
                kelas = new List<String>();

                totalKolom = lines[0].Split(new char[] { ',' }).Length; //cari berapa banyak ada atribut
                foreach (string line in lines)
                {

                    List<String> barisAtribut = new List<String>();
                    string[] col = line.Split(new char[] { ',' });
                    for(int i = 0; i < totalKolom-1; i++)
                    {
                        barisAtribut.Add(col[i]);
                    }

                    atribut.Add(barisAtribut);
                    kelas.Add(col[totalKolom - 1]);
                }
            }
            
            //cetak judul kolom
            for (int i = 0; i < totalKolom-1; i++)
            {
                dataGridView1.Columns.Add("a"+(i+1), "Atribut "+(i+1));
            }
            dataGridView1.Columns.Add("kelas", "Kelas");

            //cetak data atribut ke data grid
            for (int i = 0; i < atribut.Count; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].HeaderCell.Value = (i + 1)+".";
                for (int j = 0; j < atribut[i].Count; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = atribut[i][j];
                }
            }
            for (int j = 0; j < kelas.Count; j++)
            {
                dataGridView1.Rows[j].Cells[dataGridView1.Columns.Count - 1].Value = kelas[j];
            }

            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            jumlahHiddenLayer = Convert.ToInt32(numericUpDown1.Value);
            jumlahNeuronHiddenLayer = Convert.ToInt32(numericUpDown2.Value);
            learningRate = Convert.ToDouble(numericUpDown3.Value);
            maksIterasi = Convert.ToInt32(numericUpDown4.Value);

            training();
        }

        public void training()
        {
            int coba = 0;
            int indexLastHiddenLayer = jumlahHiddenLayer - 1;
            Layer[] hiddenLayer = new Layer[jumlahHiddenLayer];

            MessageBox.Show("JUMLAH DATA: " + atribut.Count.ToString());

            //inisialisasi layer

            //bukan perulangan
            Layer outputLayer = new Layer(kelas.Distinct().Count());

            for(int i = 0; i < jumlahHiddenLayer; i++)
            {
                hiddenLayer[i] = new Layer(jumlahNeuronHiddenLayer, outputLayer.jumlahNeuron);
            }
            //Layer hiddenLayer = new Layer(3, outputLayer.jumlahNeuron);

            //bukan perulangan
            Layer inputLayer = new Layer(atribut[0].Count, hiddenLayer[0].jumlahNeuron);

            do
            {
                for (int i = 0; i < atribut.Count; i++) //banyaknya jumlah data training.
                {
                    #region FEED-FORWARD
                    inputLayer.isiLayerInput(atribut[i]);

                    //perulangan
                    for (int z = 0; z < jumlahHiddenLayer; z++)
                    {
                        if(z==0) //hidden layer pertama
                        {
                            hiddenLayer[z].hitungOutput(inputLayer.outputNeuron, inputLayer.weightNeuron, inputLayer.weightBias);
                        }
                        else
                        {
                            hiddenLayer[z].hitungOutput(hiddenLayer[z - 1].outputNeuron, hiddenLayer[z - 1].weightNeuron, hiddenLayer[z - 1].weightBias);
                        }
                    }
                    //hiddenLayer.hitungOutput(inputLayer.outputNeuron, inputLayer.weightNeuron, inputLayer.weightBias);

                    //bukan perulangan
                    outputLayer.hitungOutput(hiddenLayer[indexLastHiddenLayer].outputNeuron, hiddenLayer[indexLastHiddenLayer].weightNeuron, hiddenLayer[indexLastHiddenLayer].weightBias);
                    #endregion

                    #region BACKPROPAGATE
                    outputLayer.hitungError(kelas[i]);
                    //perulangan
                    for (int z = indexLastHiddenLayer; z > -1; z--)
                    {
                        if(z == indexLastHiddenLayer)
                        {
                            hiddenLayer[z].cariDelta(learningRate, outputLayer.error);
                            hiddenLayer[z].hitungError(outputLayer.error);
                        }
                        else
                        {
                            hiddenLayer[z].cariDelta(learningRate, hiddenLayer[z + 1].error);
                            hiddenLayer[z].hitungError(hiddenLayer[z + 1].error);
                        }
                    }
                    //hiddenLayer.cariDelta(learningRate, outputLayer.error);
                    //hiddenLayer.hitungError(outputLayer.error);

                    //bukan perulangan
                    inputLayer.cariDelta(learningRate, hiddenLayer[0].error);
                    #endregion

                    #region PERBAIKI BOBOT
                    //perulangan
                    for (int z = 0; z < jumlahHiddenLayer; z++)
                    {
                        hiddenLayer[z].ubahWeight();
                    }
                    //hiddenLayer.ubahWeight();

                    //bukan perulangan
                    inputLayer.ubahWeight();
                    #endregion
                }



                coba++;
            } while (coba < 0);
            button3.Enabled = true;
        }
    }
}

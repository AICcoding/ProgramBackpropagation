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
            training();

        }

        public void training()
        {
            int coba = 0;

            MessageBox.Show(atribut.Count.ToString());

            //inisialisasi layer
            Layer inputLayer;
            Layer hiddenLayer = new Layer(3);
            Layer outputLayer = new Layer(3);
            inputLayer = new Layer(atribut[0].Count, hiddenLayer.jumlahNeuron);

            do
            {
                for (int i = 0; i < atribut.Count; i++)
                {
                    inputLayer.isiLayerInput(atribut[i]);
                    MessageBox.Show("Data ke: " + (i+1));

                    /*for (int k = 0; k <inputLayer.jumlahNeuron; k++)
                    {
                        //MessageBox.Show("Data neuron ke: " + (k + 1) + " adalah = " + inputLayer.outputNeuron[k]);
                        for (int j = 0; j < inputLayer.jumlahNeuronAtas; j++)
                        {
                            MessageBox.Show("NEURON: " + k + " - " + j + " = " + inputLayer.weightNeuron[k,j].ToString());
                        }
                    }*/
                    inputLayer.weightNeuron[0, 0] = 0.2;
                    inputLayer.weightNeuron[0, 1] = 0.3;
                    inputLayer.weightNeuron[0, 2] = -0.1;

                    inputLayer.weightNeuron[1, 0] = 0.3;
                    inputLayer.weightNeuron[1, 1] = 0.1;
                    inputLayer.weightNeuron[1, 2] = -0.1;

                    inputLayer.weightBias[0] = -0.3;
                    inputLayer.weightBias[1] = 0.3;
                    inputLayer.weightBias[2] = 0.3;

                    /*for (int k = 0; k < inputLayer.jumlahNeuron; k++)
                    {
                        //MessageBox.Show("Data neuron ke: " + (k + 1) + " adalah = " + inputLayer.outputNeuron[k]);
                        for (int j = 0; j < inputLayer.jumlahNeuronAtas; j++)
                        {
                            MessageBox.Show("NEURON: " + k + " - " + j + " = " + inputLayer.weightNeuron[k,j].ToString());
                            MessageBox.Show("BIAS: " + j + " = " + inputLayer.weightBias[j].ToString());
                        }
                    }*/

                    //hiddenLayer.inputNeuron = inputLayer.outputNeuron;
                    hiddenLayer.hitungOutput(inputLayer.outputNeuron, inputLayer.weightNeuron, inputLayer.weightBias);

                    /*for (int k = 0; k < inputLayer.jumlahNeuron; k++)
                    {
                        //MessageBox.Show("Data neuron ke: " + (k + 1) + " adalah = " + inputLayer.outputNeuron[k]);
                        for (int j = 0; j < inputLayer.jumlahNeuronAtas; j++)
                        {
                            MessageBox.Show("NEURON: " + k + " - " + j + " = " + inputLayer.weightNeuron[k,j].ToString());
                            MessageBox.Show("BIAS: " + j + " = " + inputLayer.weightBias[j].ToString());
                        }
                    }*/

                    /*double a = 0.3 + 1*(0.3) + 1*(0.1);
                    double b = 1.0 / (1.0 + Math.Exp(-a));

                    MessageBox.Show("Hasil a: " + a + "\n" + "Hasil b: " + b);*/

                    /*for (int z = 0; z < outputLayer.jumlahNeuron; z++)
                    {
                        double temp = 0;

                        for (int j = 0; j < inputLayer.outputNeuron.Length; j++)
                        {
                            //Hitung net
                            temp += (inputLayer.inputNeuron[z] * inputLayer.weightNeuron[z, j]);
                        }
                        temp += inputLayer.weightBias[z];
                        //Langsung aktivasi dengan fungsi Sigmoid
                        a = temp;
                        b = 1.0 / (1.0 + Math.Exp(-a));

                        MessageBox.Show()
                    }*/

                    for (int k = 0; k < hiddenLayer.jumlahNeuron; k++)
                    {
                        MessageBox.Show("Data neuron ke: " + (k + 1) + " adalah = " + hiddenLayer.outputNeuron[k].ToString());
                        //MessageBox.Show("Data neuron ke: " + (k + 1) + " adalah = " + hiddenLayer.inputNeuron[k]);
                        /*for (int j = 0; j < inputLayer.jumlahNeuronAtas; j++)
                        {
                            MessageBox.Show("NEURON: " + k + " - " + j + " = " + inputLayer.weightNeuron[k,j].ToString());
                            MessageBox.Show("BIAS: " + j + " = " + inputLayer.weightBias[j].ToString());
                        }*/
                    }

                }



                coba++;
            } while (coba < 0);
        }
    }
}

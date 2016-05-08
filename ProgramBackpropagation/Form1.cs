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

        public List<List<String>> atribut, atribut_testing;
        public List<String> kelas, kelas_testing;

        Layer[] hiddenLayer;
        Layer outputLayer;
        Layer inputLayer;

        double learningRate;
        int jumlahHiddenLayer;
        int jumlahNeuronHiddenLayer;
        int maksIterasi;

        List<List<int>> target;
        List<string> nama_kelas;

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

        private void buat_target()
        {
            target = new List<List<int>>();
            nama_kelas = new List<string>();

            List<int> tmp;
            nama_kelas = kelas.Distinct().ToList<String>();

            for (int i = 0; i < kelas.Count; i++)
            {
                tmp = new List<int>();
                for (int j = 0; j < nama_kelas.Count; j++)
                {
                    tmp.Add(0);
                }

                for (int j = 0; j < nama_kelas.Count; j++)
                {
                    if (kelas[i] == nama_kelas[j])
                    {
                        tmp[j] = 1;
                        target.Add(tmp);
                        break;
                    }
                }          
            }
        }

        private bool hasil_testing_data(List<String> atribut_testing, String kelas_testing)
        {
            Layer[] hiddenLayer_tmp;
            Layer outputLayer_tmp;

            hiddenLayer_tmp = new Layer[jumlahHiddenLayer];
            outputLayer_tmp = new Layer(kelas.Distinct().Count());
            hiddenLayer_tmp = hiddenLayer;
            outputLayer_tmp = outputLayer;

            int indexLastHiddenLayer = jumlahHiddenLayer - 1;

            //isi 1 data testing
            List<String> tes = new List<String>();
            for (int i = 0; i < atribut_testing.Count; i++)
            {
                tes.Add(atribut_testing[i].ToString());
            }
            Layer layestes = new Layer(atribut[0].Count, hiddenLayer_tmp[0].jumlahNeuron);

            #region FEED-FORWARD
            layestes.isiLayerInput(tes);

            //perulangan
            for (int z = 0; z < jumlahHiddenLayer; z++)
            {
                if (z == 0) //hidden layer pertama
                {
                    hiddenLayer_tmp[z].hitungOutput(layestes.outputNeuron, inputLayer.weightNeuron, inputLayer.weightBias);
                }
                else
                {
                    hiddenLayer_tmp[z].hitungOutput(hiddenLayer_tmp[z - 1].outputNeuron, hiddenLayer_tmp[z - 1].weightNeuron, hiddenLayer_tmp[z - 1].weightBias);
                }
            }

            //bukan perulangan
            outputLayer_tmp.hitungOutput(hiddenLayer_tmp[indexLastHiddenLayer].outputNeuron, hiddenLayer_tmp[indexLastHiddenLayer].weightNeuron, hiddenLayer_tmp[indexLastHiddenLayer].weightBias);
            #endregion

            int[] hasil = new int[outputLayer_tmp.jumlahNeuron];
            int index_angka_1 = -1;

            for (int k = 0; k < outputLayer_tmp.jumlahNeuron; k++)
            {
                hasil[k] = (int)Math.Round(outputLayer_tmp.outputNeuron[k]);
                if (hasil[k] == 1)
                {
                    index_angka_1 = k;
                }
            }
            if (index_angka_1 != -1)
            {
                //nama kelas sama
                if (nama_kelas[index_angka_1] == kelas_testing)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            jumlahHiddenLayer = Convert.ToInt32(numericUpDown1.Value);
            jumlahNeuronHiddenLayer = Convert.ToInt32(numericUpDown2.Value);
            learningRate = Convert.ToDouble(numericUpDown3.Value);
            maksIterasi = Convert.ToInt32(numericUpDown4.Value);

            training();
            button3.Enabled = true;
        }

        public void training()
        {
            double penambahan_loading = 100F / maksIterasi;
            int penambahan_1_persen = (int)(1F / penambahan_loading);

            int coba = 0;
            int indexLastHiddenLayer = jumlahHiddenLayer - 1;
            hiddenLayer = new Layer[jumlahHiddenLayer];

            //MessageBox.Show("JUMLAH DATA: " + atribut.Count.ToString());

            //inisialisasi layer

            //bukan perulangan
            outputLayer = new Layer(kelas.Distinct().Count());

            buat_target();

            for (int i = indexLastHiddenLayer; i > -1; i--)
            {
                if (i != indexLastHiddenLayer)
                {
                    hiddenLayer[i] = new Layer(jumlahNeuronHiddenLayer, hiddenLayer[i + 1].jumlahNeuron);
                }
                else
                {
                    hiddenLayer[i] = new Layer(jumlahNeuronHiddenLayer, outputLayer.jumlahNeuron);
                }
            }
            //Layer hiddenLayer = new Layer(3, outputLayer.jumlahNeuron);

            //bukan perulangan
            inputLayer = new Layer(atribut[0].Count, hiddenLayer[0].jumlahNeuron);

            do
            {
                if (coba % penambahan_1_persen==0)
                {
                    progressBar1.Value += 1;
                }
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
                    //outputLayer.hitungError(kelas[i]);
                    outputLayer.hitungError(target[i]);

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
            } while (coba < maksIterasi);
            MessageBox.Show("Selesai dalam " + coba + " iterasi");


            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int indexLastHiddenLayer = jumlahHiddenLayer - 1;


            //data tes Iris-virginica
            List<String> tes = new List<string>();
            tes.Add(numericUpDown5.Value.ToString());
            tes.Add(numericUpDown6.Value.ToString());
            tes.Add(numericUpDown7.Value.ToString());
            tes.Add(numericUpDown8.Value.ToString());

            Layer layestes = new Layer(atribut[0].Count, hiddenLayer[0].jumlahNeuron);

            #region FEED-FORWARD
            layestes.isiLayerInput(tes);

            //perulangan
            for (int z = 0; z < jumlahHiddenLayer; z++)
            {
                if (z == 0) //hidden layer pertama
                {
                    hiddenLayer[z].hitungOutput(layestes.outputNeuron, inputLayer.weightNeuron, inputLayer.weightBias);
                }
                else
                {
                    hiddenLayer[z].hitungOutput(hiddenLayer[z - 1].outputNeuron, hiddenLayer[z - 1].weightNeuron, hiddenLayer[z - 1].weightBias);
                }
            }

            //bukan perulangan
            outputLayer.hitungOutput(hiddenLayer[indexLastHiddenLayer].outputNeuron, hiddenLayer[indexLastHiddenLayer].weightNeuron, hiddenLayer[indexLastHiddenLayer].weightBias);
            #endregion

            int[] hasil = new int[outputLayer.jumlahNeuron];
            int index_angka_1 = -1;

            for (int k = 0; k < outputLayer.jumlahNeuron; k++)
            {
                hasil[k] = (int)Math.Round(outputLayer.outputNeuron[k]);
                if (hasil[k] == 1)
                {
                    index_angka_1 = k;
                }
            }
            if (index_angka_1 != -1)
            {
                MessageBox.Show("Pola " + nama_kelas[index_angka_1]);
            }
            else if (index_angka_1 == -1)
            {
                MessageBox.Show("tidak berhasil mengenali pola");
            }          
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openfile1 = new OpenFileDialog();
            openfile1.Filter = "File UCI data set (*.data)|*.data";
            if (openfile1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Refresh();

                string[] lines = File.ReadAllLines(openfile1.FileName);

                atribut_testing = new List<List<string>>();
                kelas_testing = new List<String>();

                totalKolom = lines[0].Split(new char[] { ',' }).Length; //cari berapa banyak ada atribut
                foreach (string line in lines)
                {

                    List<String> barisAtribut = new List<String>();
                    string[] col = line.Split(new char[] { ',' });
                    for (int i = 0; i < totalKolom - 1; i++)
                    {
                        barisAtribut.Add(col[i]);
                    }

                    atribut_testing.Add(barisAtribut);
                    kelas_testing.Add(col[totalKolom - 1]);
                }
            }

            //cetak judul kolom
            for (int i = 0; i < totalKolom - 1; i++)
            {
                dataGridView1.Columns.Add("a" + (i + 1), "Atribut " + (i + 1));
            }
            dataGridView1.Columns.Add("kelas", "Kelas");

            //cetak data atribut ke data grid
            for (int i = 0; i < atribut_testing.Count; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].HeaderCell.Value = (i + 1) + ".";
                for (int j = 0; j < atribut_testing[i].Count; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = atribut_testing[i][j];
                }
            }
            for (int j = 0; j < kelas_testing.Count; j++)
            {
                dataGridView1.Rows[j].Cells[dataGridView1.Columns.Count - 1].Value = kelas_testing[j];
            }

            button5.Enabled = true;
            MessageBox.Show("Berhasil load data");

        }

        private void button5_Click(object sender, EventArgs e)
        {
            int jumlah_benar, jumlah_salah;
            double akurasi;
            jumlah_benar = 0;
            jumlah_salah = 0;
            for (int i = 0; i < atribut_testing.Count; i++)
            {
                if (hasil_testing_data(atribut_testing[i], kelas_testing[i]) == true)
                {
                    jumlah_benar += 1;
                }
                else
                {
                    jumlah_salah += 1;
                }
            }

            akurasi = (double)jumlah_benar / (double)atribut_testing.Count * 100;
            MessageBox.Show("Total data = " + atribut_testing.Count + "\nJumlah benar = " + jumlah_benar + "\nJumlah salah = " + jumlah_salah + "\ntingkat akurasi = " + akurasi+" %");

        }
    }
}

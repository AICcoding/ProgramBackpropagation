using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgramBackpropagation
{
    class Layer
    {
        private Random r = new Random();

        int tipe; //1: Hidden Layer, 2: Output Layer.

        public int jumlahInput; //neuron bawah
        public int jumlahNeuron; //neuron ini
        public int jumlahWeight; //neuron atas x 2
        public int jumlahNeuronAtas;

        public double[] inputNeuron; //[Urutan Neuron Bawah]
        public double[] outputNeuron; //[Urutan Neuron Ini]
        public double[,] weightNeuron; //[Urutan Neuron Ini][Urutan Neuron Atas]
        public double[] weightBias; //[Urutan Neuron Atas] - 

        public double[,] deltaWeightNeuron; //[Urutan Neuron Ini][Urutan Neuron Atas]
        public double[] deltaWeightBias; //[Urutan Neuron Atas] - 

        public double[] error; //[Urutan Neuron ini] - Error dari neuron(s) di layer ini

        public void inisialisasi()
        {
            weightNeuron = new double[jumlahNeuron, jumlahNeuronAtas];
            weightBias = new double[jumlahNeuronAtas];

            error = new double[jumlahNeuron];

            deltaWeightNeuron = new double[jumlahNeuron, jumlahNeuronAtas];
            deltaWeightBias = new double[jumlahNeuronAtas];

            outputNeuron = new double[jumlahNeuron];
        }

        public void isiLayerInput(List<String> atribut) //khusus untuk input layer
        {
            for (int i = 0; i < atribut.Count; i++)
            {
                this.outputNeuron[i] = Convert.ToDouble(atribut[i]);
            }
        }

        public void randomWeight()
        {
            for (int i = 0; i < jumlahNeuron; i++)
            {
                for (int j = 0; j < jumlahNeuronAtas; j++)
                {
                    weightNeuron[i, j] = r.Next(-10, 10) / 10F;
                    weightBias[j] = r.Next(-10, 10) / 10F;
                }
            }
        }

        #region Feed-Forward

        public void hitungOutput(double[] inputNeuronBawah, double[,] weightNeuronBawah, double[] weightBias) //semua parameter ini berasal dari data layer bawah.
        {
            for(int i = 0; i < jumlahNeuron; i++)
            {
                double temp = 0;

                for (int j = 0; j < inputNeuronBawah.Length; j++)
                {
                    //Hitung net
                    temp += (inputNeuronBawah[j] * weightNeuronBawah[j, i]);
                }
                temp += weightBias[i];

                //Langsung aktivasi dengan fungsi Sigmoid
                outputNeuron[i] = hitungSigmoid(temp);
            }
        }

        public double hitungSigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
        #endregion

        #region Backpropagation
        public void hitungError(String kelas) //overload method untuk output layer
        {
            double t = Convert.ToDouble(kelas);

            for (int i = 0; i < jumlahNeuron; i++)
            {
                //hitung δ = (tk – yk) yk (1 – yk) //khusus untuk layer output
                error[i] = (t - this.outputNeuron[i]) * hitungDerivatifSigmoid(this.outputNeuron[i]);
            }
        }

        public void hitungError(List<int> target) //overload method untuk output layer
        {
            double t;

            for (int i = 0; i < jumlahNeuron; i++)
            {
                t = target[i];
                //hitung δ = (tk – yk) yk (1 – yk) //khusus untuk layer output
                error[i] = (t - this.outputNeuron[i]) * hitungDerivatifSigmoid(this.outputNeuron[i]);
            }
        }

        public void hitungError(double[] errorNeuronAtas) //overload method untuk hidden layer
        {
            for (int i = 0; i < jumlahNeuron; i++)
            {
                for (int j = 0; j < jumlahNeuronAtas; j++)
                {
                    //hitung δ pada hidden layer.
                    error[i] += (errorNeuronAtas[j] * weightNeuron[i,j]);
                }
                //langsung hitung dengan derivatifnya
                error[i] = error[i] * hitungDerivatifSigmoid(this.outputNeuron[i]);
            }
        }

        public void cariDelta(double learningRate, double[] error) //error dari neuron atas. -- ini ingat ya untuk neuron bawahnya lho.
        {
            for (int i = 0; i < jumlahNeuron; i++)
            {
                for (int j = 0; j < jumlahNeuronAtas; j++)
                {
                    deltaWeightNeuron[i,j] = learningRate * error[j] * outputNeuron[i];
                    deltaWeightBias[j] = learningRate * error[j];
                }
            }
        }

        public double hitungDerivatifSigmoid(double x)
        {
            return x * (1 - x);
        }
        
        #endregion

        public void ubahWeight()
        {
            for (int i = 0; i < jumlahNeuron; i++)
            {
                for (int j = 0; j < jumlahNeuronAtas; j++)
                {
                    weightNeuron[i,j] += deltaWeightNeuron[i,j];
                    weightBias[j] += deltaWeightBias[j];
                }
            }
        }

        public Layer(int jumlahNeuron)
        {
            this.jumlahNeuron = jumlahNeuron;
            inisialisasi();
            randomWeight();
        }

        public Layer(int jumlahNeuron, int jumlahNeuronAtas)
        {
            this.jumlahNeuron = jumlahNeuron;
            this.jumlahNeuronAtas = jumlahNeuronAtas;
            inisialisasi();
            randomWeight();
        }


    }
}

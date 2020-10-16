using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

using ZedGraph;


namespace Особые_точки_РЭГ_нарезка
{
    public partial class Form1 : Form
    {
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        delegate void SetTextCallback(int nomer);

        const int time_numerical = 60;
        const int number_point_line = 1000;
        int nomer_period = 0;
        int scet_period = 0;

        UseZedgraph usergraph;

        public Form1()
        {
            InitializeComponent();
        }

        void Clear_list_zed()
        {
            ZedGraph.MasterPane masterPane = zedGraph1.MasterPane;

            masterPane.PaneList.Clear();
            GraphPane pane = new GraphPane();
            pane.CurveList.Clear();

            // Добавим новый график в MasterPane
            masterPane.Add(pane);

            // Установим масштаб по умолчанию для оси X
            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MaxAuto = true;
            // Установим масштаб по умолчанию для оси Y
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;

            using (Graphics g = CreateGraphics())
            {               
                masterPane.SetLayout(g, PaneLayout.SingleColumn);
            }

            zedGraph1.AxisChange();
            zedGraph1.Invalidate();

        }//Очистить график - функция

     
        public void Maker_graph_one_period()
        {            
            richTextBox2.Clear();
            
            int reg = System.Convert.ToInt32(this.textBox1.Text);
            int ekg = 1;

            int B2 = System.Convert.ToInt32(textBox4.Text);
            int B3 = System.Convert.ToInt32(textBox6.Text);
            int B4 = System.Convert.ToInt32(textBox8.Text);

            Initial_data init_data = new Initial_data("Исправляемый цикл.txt", reg, ekg);
            long[,] row = init_data.get_row1();

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.clearAll();//Очищаем полотно
            usergraph.makeGraph_on_canal_only_one_graph();
            
            usergraph.makeGraph_osob_point_without_EKG_one_period(row, B2, B3, B4);
            usergraph.resetGraph();
            usergraph.install_pane("t, мc", "R, Ом", " ");//Устанавливаем оси и заглавие
            usergraph.resetGraph();//Обновляем
           
            for (int q = 1; q < init_data.get_b(); q++)
            {               
                richTextBox2.AppendText(System.Convert.ToString(q) + "\t" + System.Convert.ToString(row[q, 1]) + "\n");
            }

            StreamWriter rw2 = new StreamWriter("Исправляемый цикл - точки.txt");
            rw2.WriteLine(row[B2, 0] + "\t" + row[B2, 1]);
            rw2.WriteLine(row[B3, 0] + "\t" + row[B3, 1]);
            rw2.WriteLine(row[B4, 0] + "\t" + row[B4, 1]);

            rw2.Close();
            
        }
               
        private void button1_Click(object sender, EventArgs e)//Очистить график
        {
            usergraph.clearAll();

            richTextBox2.Clear();
        }

        private void button2_Click(object sender, EventArgs e)//Сохранить график
        {
            usergraph.saveGraph();
        }

        private void button4_Click(object sender, EventArgs e)//Открыть файл
        {    
            string adres = "q";
            string adres2 = "q";
            string datapath = "w";

            int da5 = 0;

            StringBuilder buffer2 = new StringBuilder();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            OpenFileDialog qqq = new OpenFileDialog();
            qqq.Filter = "Файлы txt|*.txt";

            if (qqq.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {                
                adres = qqq.FileName;
                buffer2.Insert(0, qqq.FileName);

                da5 = buffer2.Length;
                buffer2.Remove(da5 - 8, 8);
                adres2 = buffer2.ToString();
                datapath = Path.Combine(Application.StartupPath);
         
                System.IO.File.Copy(Path.Combine(qqq.InitialDirectory, qqq.FileName), Path.Combine(datapath, "test.txt"), true);
                try
                {
                    System.IO.File.Copy(adres2 + "Информация о пациенте.txt", Path.Combine(datapath, "Информация о пациенте.txt"), true);
                }
                catch
                {    }
            }

            ///////////////////////////////////////////////////////////

            int reg = Convert.ToInt32(this.textBox1.Text);
            int ekg = 1;
            
            try
            {
                Initial_data init_data = new Initial_data("test.txt", reg, ekg);
                init_data.row1_shift_time_0();//Сдвигаем время к 0
                init_data.row1_smothing();// Сглаживаем полученные данные
                init_data.row1_write_in_file("test3.txt");

                usergraph = new UseZedgraph(zedGraph1);
                usergraph.clearAll();//Очищаем полотно
                usergraph.makeGraph_4kanal(init_data.get_row1(), init_data.get_b());//Строим график
                usergraph.install_pane("t, мc", "R, Ом", "Каналы");//Устанавливаем оси и загавие
                usergraph.resetGraph();//Обновляем
              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Выбран неправильный файл");
            }
            
        }

        private void button6_Click(object sender, EventArgs e)//Обновить данные
        {
            Clear_list_zed();

            int reg = System.Convert.ToInt32(this.textBox1.Text);
            int ekg = 1;
            try
            {
                Initial_data init_data = new Initial_data("test3.txt", reg, ekg);
              
                usergraph = new UseZedgraph(zedGraph1);
                usergraph.clearAll();//Очищаем полотно
                usergraph.makeGraph_4kanal(init_data.get_row1(), init_data.get_b());//Строим график
                usergraph.install_pane("t, мc", "R, Ом", "Каналы");//Устанавливаем оси и загавие
                usergraph.resetGraph();//Обновляем
             }
            catch (Exception ex)
            {
                MessageBox.Show("Выбран неправильный файл");
            }
        }

        private void button5_Click(object sender, EventArgs e)//Рассчитать особые точки
        {
            StringBuilder buffer = new StringBuilder();
            richTextBox2.Clear();

            int reg = System.Convert.ToInt32(this.textBox1.Text);                   
            int ekg = 1;
            
            Initial_data init_data = new Initial_data("test3.txt", reg, ekg);
            init_data.row1_shift_time_0();//Сдвигаем время к 0
            init_data.row1_smothing();// Сглаживаем полученные данные
            init_data.row2_calculate();
            init_data.row3_average_kanal_reg();
            init_data.row4_smoothing_ekg();

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.clearAll();//Очищаем полотно
            usergraph.makeGraph_on_chosen_canal();

            //Разделяем 
            Initial_processing.Divided_by_periods_data divided_row = new Initial_processing.Divided_by_periods_data(init_data, this.comboBox2.Text);
            divided_row.return_data_in_period();
         
            Special_point osob_point = new Special_point(divided_row, init_data);

            long[,] osob = null;
            
            osob_point.return_osob_point(this.comboBox2.Text);                                  
            osob = osob_point.get_spec_point();

            int arre = osob.Length;
            int ew = arre / 15;//счетчик найденных максимумов

            /////////////////////////
            /////////////////////////
            // новое
            //ЭКГ мах -     0
            //ЭКГ мах -х -  1
            // В1, В5 -     2
            // В1x, В5x -   3
            // В2 -         4
            // В2x -        5
            // В3 -         6
            // В3x -        7
            // В4 -         8  
            // В4x -        9
            //osob_10  -    Изначальная высота

            ////////////////////////

            long[,] osob_x = new long[5, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[5, ew];

            for (int i = 0; i < ew - 1; i++)
            {
                osob_x[0, i] = osob[1, i];
                osob_y[0, i] = osob[0, i];

                osob_x[1, i] = osob[3, i];
                osob_y[1, i] = osob[2, i];

                osob_x[2, i] = osob[5, i];
                osob_y[2, i] = osob[4, i] + osob[10, i];

                osob_x[3, i] = osob[7, i];
                osob_y[3, i] = osob[6, i] + osob[10, i];

                osob_x[4, i] = osob[9, i];
                osob_y[4, i] = osob[8, i] + osob[10, i];
            }

            textBox3.Text = Convert.ToString(ew - 4);
            usergraph.makeGraph_osob_point(osob_x, osob_y, ew);
            usergraph.install_pane("t, мc", "R, Ом", " ");//Устанавливаем оси и заглавие
            usergraph.resetGraph();//Обновляем
            StreamReader scetch = new StreamReader("счетчик.txt");
                String ssssq = scetch.ReadLine();
                textBox10.Text = ssssq;
                scet_period = System.Convert.ToInt32(ssssq);
                scetch.Close();            
        }

        private void button7_Click(object sender, EventArgs e)//Влево <
        {
            button12.Enabled = false;
            button13.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
            button16.Enabled = false;
            button17.Enabled = false;

            nomer_period = System.Convert.ToInt32(textBox2.Text);

            if (nomer_period > 0)
            {
                nomer_period--;
            }
            textBox2.Text = System.Convert.ToString(nomer_period);

            Boolean povtor = false;

            richTextBox2.Clear();

            int reg = System.Convert.ToInt32(this.textBox1.Text);
            int ekg = 1;
            
            Initial_data init_data = new Initial_data("test3.txt", reg, ekg);
            init_data.row1_shift_time_0();//Сдвигаем время к 0
            init_data.row1_smothing();// Сглаживаем полученные данные
            init_data.row2_calculate();
            init_data.row3_average_kanal_reg();
            init_data.row4_smoothing_ekg();

            long[,] row_1 = init_data.get_row1();

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.clearAll();//Очищаем полотно
        
            Initial_processing.Divided_by_periods_data divided_row = new Initial_processing.Divided_by_periods_data(init_data, this.comboBox2.Text);
            Special_point osob_point = new Special_point(divided_row, init_data);

            long[,] osob = null;
            osob_point.return_osob_point(this.comboBox2.Text);
            osob = osob_point.get_spec_point();

            int arre = osob.Length;
            int ew = arre / 15;//счетчик найденных максимумов

            /////////////////////////
            /////////////////////////
            // новое
            //ЭКГ мах -     0
            //ЭКГ мах -х -  1
            // В1, В5 -     2
            // В1x, В5x -   3
            // В2 -         4
            // В2x -        5
            // В3 -         6
            // В3x -        7
            // В4 -         8  
            // В4x -        9
            //osob_10  -    Изначальная высота

            ////////////////////////

            long[,] osob_x = new long[5, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[5, ew];

            for (int i = 0; i < ew - 1; i++)
            {
                osob_x[0, i] = osob[1, i];
                osob_y[0, i] = osob[0, i];

                osob_x[1, i] = osob[3, i];
                osob_y[1, i] = osob[2, i];

                osob_x[2, i] = osob[5, i];
                osob_y[2, i] = osob[4, i] + osob[10, i];

                osob_x[3, i] = osob[7, i];
                osob_y[3, i] = osob[6, i] + osob[10, i];

                osob_x[4, i] = osob[9, i];
                osob_y[4, i] = osob[8, i] + osob[10, i];

            }
         
            int scah_max = 1;
            int B2 = 0;
            int B3 = 0;
            int B4 = 0;

            for (int q = 0; q < init_data.get_b(); q++)// считаем производную
            {
                if (row_1[q, 0] >= osob_x[1, nomer_period] && row_1[q, 0] < osob_x[1, nomer_period + 1])
                {               
                    if (osob_x[2, nomer_period] > row_1[q, 0])
                    {
                        B2 = scah_max;
                    }
                    if (osob_x[3, nomer_period] > row_1[q, 0])
                    {
                        B3 = scah_max;
                    }
                    if (osob_x[4, nomer_period] > row_1[q, 0])
                    {
                        B4 = scah_max;
                    }

                    scah_max++;
                    povtor = true;
                } 
            }

            textBox5.Text = System.Convert.ToString(scah_max);
            textBox7.Text = System.Convert.ToString(scah_max);
            textBox9.Text = System.Convert.ToString(scah_max);

            textBox4.Text = System.Convert.ToString(B2);
            textBox6.Text = System.Convert.ToString(B3);
            textBox8.Text = System.Convert.ToString(B4);

            // Заполним массив точек для кривой f1-3(x)

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.clearAll();//Очищаем полотно
            usergraph.makeGraph_on_chosen_canal_only_one_graph(reg, osob_x[1, nomer_period], osob_x[1, nomer_period+1]);

            if (povtor == true)
            {
                usergraph.makeGraph_osob_point_without_EKG_one_period(osob_x, osob_y, B2, B3, B4, nomer_period);
                usergraph.resetGraph();
                usergraph.install_pane("t, мc", "R, Ом", " ");//Устанавливаем оси и заглавие
                usergraph.resetGraph();//Обновляем

            }
          
            StreamWriter scetch = new StreamWriter("счетчик.txt", true);
            scetch.WriteLine(scet_period);
            scetch.Close();          

        }

        private void button8_Click(object sender, EventArgs e)// Вправо >
        {
            button12.Enabled = false;
            button13.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
            button16.Enabled = false;
            button17.Enabled = false;

            nomer_period = System.Convert.ToInt32(textBox2.Text);

            if (nomer_period < System.Convert.ToInt32(textBox3.Text))
            {
                nomer_period++;
            }
            textBox2.Text = System.Convert.ToString(nomer_period);

            Boolean povtor = false;

            StringBuilder buffer = new StringBuilder();

            richTextBox2.Clear();


            int reg = System.Convert.ToInt32(this.textBox1.Text);
            int ekg = 1;

            Initial_data init_data = new Initial_data("test3.txt", reg, ekg);
            init_data.row1_shift_time_0();//Сдвигаем время к 0
            init_data.row1_smothing();// Сглаживаем полученные данные
            init_data.row2_calculate();
            init_data.row3_average_kanal_reg();
            init_data.row4_smoothing_ekg();

            long[,] row_1 = init_data.get_row1();

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.clearAll();//Очищаем полотно

            Initial_processing.Divided_by_periods_data divided_row = new Initial_processing.Divided_by_periods_data(init_data, this.comboBox2.Text);
            Special_point osob_point = new Special_point(divided_row, init_data);

            long[,] osob = null;
            osob_point.return_osob_point(this.comboBox2.Text);
            osob = osob_point.get_spec_point();

            int arre = osob.Length;
            int ew = arre / 15;//счетчик найденных максимумов

            /////////////////////////
            /////////////////////////
            // новое
            //ЭКГ мах -     0
            //ЭКГ мах -х -  1
            // В1, В5 -     2
            // В1x, В5x -   3
            // В2 -         4
            // В2x -        5
            // В3 -         6
            // В3x -        7
            // В4 -         8  
            // В4x -        9
            //osob_10  -    Изначальная высота

            ////////////////////////

            long[,] osob_x = new long[5, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[5, ew];

            for (int i = 0; i < ew - 1; i++)
            {
                osob_x[0, i] = osob[1, i];
                osob_y[0, i] = osob[0, i];

                osob_x[1, i] = osob[3, i];
                osob_y[1, i] = osob[2, i];

                osob_x[2, i] = osob[5, i];
                osob_y[2, i] = osob[4, i] + osob[10, i];

                osob_x[3, i] = osob[7, i];
                osob_y[3, i] = osob[6, i] + osob[10, i];

                osob_x[4, i] = osob[9, i];
                osob_y[4, i] = osob[8, i] + osob[10, i];
            }

            int scah_max = 1;
            int B2 = 0;
            int B3 = 0;
            int B4 = 0;

            for (int q = 0; q < init_data.get_b(); q++)// считаем производную
            {
                if (row_1[q, 0] >= osob_x[1, nomer_period] && row_1[q, 0] < osob_x[1, nomer_period + 1])
                {
                    if (osob_x[2, nomer_period] > row_1[q, 0])
                    {
                        B2 = scah_max;
                    }
                    if (osob_x[3, nomer_period] > row_1[q, 0])
                    {
                        B3 = scah_max;
                    }
                    if (osob_x[4, nomer_period] > row_1[q, 0])
                    {
                        B4 = scah_max;
                    }
                    scah_max++;
                    povtor = true;
                }
            }

            textBox5.Text = System.Convert.ToString(scah_max);
            textBox7.Text = System.Convert.ToString(scah_max);
            textBox9.Text = System.Convert.ToString(scah_max);

            textBox4.Text = System.Convert.ToString(B2);
            textBox6.Text = System.Convert.ToString(B3);
            textBox8.Text = System.Convert.ToString(B4);

            // Заполним массив точек для кривой f1-3(x)

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.clearAll();//Очищаем полотно
            usergraph.makeGraph_on_chosen_canal_only_one_graph(reg, osob_x[1, nomer_period], osob_x[1, nomer_period + 1]);

            if (povtor == true)
            {
                usergraph.makeGraph_osob_point_without_EKG_one_period(osob_x, osob_y, B2, B3, B4, nomer_period);
                usergraph.resetGraph();
                usergraph.install_pane("t, мc", "R, Ом", " ");//Устанавливаем оси и заглавие
                usergraph.resetGraph();//Обновляем

            }
         

            StreamWriter scetch = new StreamWriter("счетчик.txt", true);
            scetch.WriteLine(scet_period);
            scetch.Close();
        }

        private void button10_Click(object sender, EventArgs e)// Сохранить данные
        {
            string adres = "q";
            string datapath = "w";

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                adres = fbd.SelectedPath;
                datapath = Path.Combine(Application.StartupPath);
               
                FileInfo f1 = new FileInfo(Path.Combine(Application.StartupPath, "Данные обработанные.txt"));
                f1.CopyTo(Path.Combine(adres, "Данные обработанные.txt"), true);

                FileInfo f2 = new FileInfo(Path.Combine(Application.StartupPath, "Особые точки.txt"));
                f2.CopyTo(Path.Combine(adres, "Особые точки.txt"), true);
                                               
            }
        }

        private void button3_Click(object sender, EventArgs e)//Сохранить 1 период
        {
            nomer_period = System.Convert.ToInt32(textBox2.Text);
            Boolean povtor = false;
           
            richTextBox2.Clear();

            long n2_x = 0;
            long n3_x = 0;
            long n4_x = 0;

            long n2_y = 0;
            long n3_y = 0;
            long n4_y = 0;

            int B2 = System.Convert.ToInt32(textBox4.Text);
            int B3 = System.Convert.ToInt32(textBox6.Text);
            int B4 = System.Convert.ToInt32(textBox8.Text);

            int reg = System.Convert.ToInt32(this.textBox1.Text);
            int ekg = 1;

            Initial_data init_data = new Initial_data("test3.txt", reg, ekg);
            init_data.row1_shift_time_0();//Сдвигаем время к 0
            init_data.row1_smothing();// Сглаживаем полученные данные
            init_data.row2_calculate();
            init_data.row3_average_kanal_reg();
            init_data.row4_smoothing_ekg();

            long[,] row_1 = init_data.get_row1();
            int b = init_data.get_b();

            Initial_processing.Divided_by_periods_data divided_row = new Initial_processing.Divided_by_periods_data(init_data, this.comboBox2.Text);
            Special_point osob_point = new Special_point(divided_row, init_data);

            long[,] osob = null;
            osob_point.return_osob_point(this.comboBox2.Text);
            osob = osob_point.get_spec_point();

            int arre = osob.Length;
            int ew = arre / 15;//счетчик найденных максимумов

            /////////////////////////
            /////////////////////////
            // новое
            //ЭКГ мах -     0
            //ЭКГ мах -х -  1
            // В1, В5 -     2
            // В1x, В5x -   3
            // В2 -         4
            // В2x -        5
            // В3 -         6
            // В3x -        7
            // В4 -         8  
            // В4x -        9
            //osob_10  -    Изначальная высота

            ////////////////////////

            long[,] osob_x = new long[5, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[5, ew];

            for (int i = 0; i < ew - 1; i++)
            {
                osob_x[0, i] = osob[1, i];
                osob_y[0, i] = osob[0, i];

                osob_x[1, i] = osob[3, i];
                osob_y[1, i] = osob[2, i];

                osob_x[2, i] = osob[5, i];
                osob_y[2, i] = osob[4, i] + osob[10, i];

                osob_x[3, i] = osob[7, i];
                osob_y[3, i] = osob[6, i] + osob[10, i];

                osob_x[4, i] = osob[9, i];
                osob_y[4, i] = osob[8, i] + osob[10, i];

            }

            StreamWriter rw = new StreamWriter("Данные обработанные.txt", true);
            StreamWriter rw2 = new StreamWriter("Особые точки.txt", true);

            int nub_dop = 0;

            for (int q = 3; q < b; q++)// Выбираем отрезок
            {
                if (row_1[q, 0] >= osob_x[1, nomer_period] && row_1[q, 0] < osob_x[1, nomer_period + 1])
                {                   
                    rw.Write(row_1[q, reg]+"\t");                   
                    povtor = true;

                    if (nub_dop==B2) {
                        n2_x = B2;
                        n2_y = row_1[q, reg];
                    }
                    if (nub_dop == B3)
                    {
                        n3_x = B3;
                        n3_y = row_1[q, reg];
                    }
                    if (nub_dop == B4)
                    {
                        n4_x = B4;
                        n4_y = row_1[q, reg];
                    }
                    nub_dop++;
                } 
            }
            
            //Дополняем отрезок нулями до равной длины
            for (int i = nub_dop; i < number_point_line; i++) {
                
                if (i == number_point_line - 1)
                {
                    rw2.Write(0);
                }
                else {
                 rw.Write(0+"\t");
                }
            }

            rw.WriteLine();
                      
            for (int i = 0; i < number_point_line; i++)
            {
              
                if (i == n2_x)
                {
                    rw2.Write(n2_y+"\t");
                }
                else if (i == n3_x)
                {
                    rw2.Write(n3_y + "\t");
                }
                else if (i == n4_x)
                {
                    rw2.Write(n4_y + "\t");
                }
                else if (i == number_point_line-1)
                {
                    rw2.Write(0);
                }else  {
              rw2.Write(0 + "\t");
                }
            }
            rw2.WriteLine();


            rw.Close();
            rw2.Close();

            scet_period++;
            textBox10.Text = System.Convert.ToString(scet_period);

            StreamWriter scetch = new StreamWriter("счетчик.txt");
            scetch.WriteLine(scet_period);
            scetch.Close();
        }

        private void button12_Click(object sender, EventArgs e)//Точка В2 влево
        {
            int Shifter = System.Convert.ToInt32(textBox4.Text);
            Shifter--;
            textBox4.Text = System.Convert.ToString(Shifter);
            Maker_graph_one_period();
        }

        private void button13_Click(object sender, EventArgs e)//Точка В2 вправо
        {
            int Shifter = System.Convert.ToInt32(textBox4.Text);
            Shifter++;
            textBox4.Text = System.Convert.ToString(Shifter);
            Maker_graph_one_period();
        }

        private void button14_Click(object sender, EventArgs e)//Точка В3 влево
        {
            int Shifter = System.Convert.ToInt32(textBox6.Text);
            Shifter--;
            textBox6.Text = System.Convert.ToString(Shifter);
            Maker_graph_one_period();
        }

        private void button15_Click(object sender, EventArgs e)//Точка В3 вправо
        {
            int Shifter = System.Convert.ToInt32(textBox6.Text);
            Shifter++;
            textBox6.Text = System.Convert.ToString(Shifter);
            Maker_graph_one_period();
        }

        private void button16_Click(object sender, EventArgs e)//Точка В4 влево
        {
            int Shifter = System.Convert.ToInt32(textBox8.Text);
            Shifter--;
            textBox8.Text = System.Convert.ToString(Shifter);
            Maker_graph_one_period();
        }

        private void button17_Click(object sender, EventArgs e)//Точка В4 вправо
        {
            int Shifter = System.Convert.ToInt32(textBox8.Text);
            Shifter++;
            textBox8.Text = System.Convert.ToString(Shifter);
            Maker_graph_one_period();
        }

        private void button11_Click(object sender, EventArgs e)//Редактировать особые точки
        {
            button12.Enabled = true;
            button13.Enabled = true;
            button14.Enabled = true;
            button15.Enabled = true;
            button16.Enabled = true;
            button17.Enabled = true;

            nomer_period = System.Convert.ToInt32(textBox2.Text);
            Boolean povtor = false;

            richTextBox2.Clear();

            int reg = System.Convert.ToInt32(this.textBox1.Text);
            int ekg = 1;

            Initial_data init_data = new Initial_data("test3.txt", reg, ekg);
            init_data.row1_shift_time_0();//Сдвигаем время к 0
            init_data.row1_smothing();// Сглаживаем полученные данные
            init_data.row2_calculate();
            init_data.row3_average_kanal_reg();
            init_data.row4_smoothing_ekg();

            long[,] row_1 = init_data.get_row1();
            int b = init_data.get_b();

            Initial_processing.Divided_by_periods_data divided_row = new Initial_processing.Divided_by_periods_data(init_data, this.comboBox2.Text);
            Special_point osob_point = new Special_point(divided_row, init_data);

            long[,] osob = null;
            osob_point.return_osob_point(this.comboBox2.Text);
            osob = osob_point.get_spec_point();

            int arre = osob.Length;
            int ew = arre / 15;//счетчик найденных максимумов

            /////////////////////////
            /////////////////////////
            // новое
            //ЭКГ мах -     0
            //ЭКГ мах -х -  1
            // В1, В5 -     2
            // В1x, В5x -   3
            // В2 -         4
            // В2x -        5
            // В3 -         6
            // В3x -        7
            // В4 -         8  
            // В4x -        9
            //osob_10  -    Изначальная высота

            ////////////////////////

            long[,] osob_x = new long[5, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[5, ew];

            for (int i = 0; i < ew - 1; i++)
            {
                osob_x[0, i] = osob[1, i];
                osob_y[0, i] = osob[0, i];

                osob_x[1, i] = osob[3, i];
                osob_y[1, i] = osob[2, i];

                osob_x[2, i] = osob[5, i];
                osob_y[2, i] = osob[4, i] + osob[10, i];

                osob_x[3, i] = osob[7, i];
                osob_y[3, i] = osob[6, i] + osob[10, i];

                osob_x[4, i] = osob[9, i];
                osob_y[4, i] = osob[8, i] + osob[10, i];

            }

            // Файлы для редактирования
            StreamWriter rw = new StreamWriter("Исправляемый цикл.txt");
            StreamWriter rw2 = new StreamWriter("Исправляемый цикл - точки.txt");

            GraphPane pane = zedGraph1.GraphPane;
            pane.CurveList.Clear();

            // Создадим список точек для кривой f2(x)
            PointPairList f2_list = new PointPairList();
            PointPairList f4_list = new PointPairList();
            int scah_max = 1;
            int B2 = 0;
            int B3 = 0;
            int B4 = 0;

            for (int q = 3; q < b; q++)// считаем производную
            {
                if (row_1[q, 0] >= osob_x[1, nomer_period] && row_1[q, 0] < osob_x[1, nomer_period + 1])
                {
                    f2_list.Add(row_1[q, 0] / 1000, 570);
                    f4_list.Add(row_1[q, 0] / 1000, row_1[q, reg]);
                    rw.WriteLine(row_1[q, 0]+"\t"+ row_1[q, reg]);
                    richTextBox2.AppendText(System.Convert.ToString(scah_max) + "\t" + System.Convert.ToString(row_1[q, reg]) + "\n");
                                       
                    if (osob_x[2, nomer_period]> row_1[q, 0])
                    {
                        B2 = scah_max;
                    }
                    if (osob_x[3, nomer_period] > row_1[q, 0])
                    {
                        B3 = scah_max;
                    }
                    if (osob_x[4, nomer_period] > row_1[q, 0])
                    {
                        B4 = scah_max;
                    }

                    scah_max++;
                    povtor = true;
                    
                } //rw.WriteLine(row1[q, 0] + "\t" + row1[q, reg]);
            }
            // Заполним массив точек для кривой f1-3(x)
            textBox5.Text = System.Convert.ToString(scah_max);
            textBox7.Text = System.Convert.ToString(scah_max);
            textBox9.Text = System.Convert.ToString(scah_max);

            textBox4.Text = System.Convert.ToString(B2);
            textBox6.Text = System.Convert.ToString(B3);
            textBox8.Text = System.Convert.ToString(B4);

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.clearAll();//Очищаем полотно
            usergraph.makeGraph_on_chosen_canal_only_one_graph(reg, osob_x[1, nomer_period], osob_x[1, nomer_period + 1]);

            if (povtor == true)
            {
                usergraph.makeGraph_osob_point_without_EKG_one_period(osob_x, osob_y, B2, B3, B4, nomer_period);
                usergraph.resetGraph();
                usergraph.install_pane("t, мc", "R, Ом", " ");//Устанавливаем оси и заглавие
                usergraph.resetGraph();//Обновляем

            }
          
            rw.Close();
            rw2.Close();
        }

        private void button19_Click(object sender, EventArgs e)//Очистить файл
        {
            StreamWriter rw = new StreamWriter("Данные обработанные.txt");
            StreamWriter rw2 = new StreamWriter("Особые точки.txt");
            rw.Close();
            rw2.Close();

            scet_period = 0;
            textBox10.Text = System.Convert.ToString(scet_period);
            StreamWriter scetch = new StreamWriter("счетчик.txt");
            scetch.WriteLine(scet_period);
            scetch.Close();
        }
    }
}

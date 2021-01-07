using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Особые_точки_РЭГ_нарезка.Initial_processing
{
    class Divided_by_periods_data
    {
        String combobox_3;

        private long[][] period;
        Initial_data init_data;

        /// <summary>
        /// Получить данные ФПГ нарезанные по периодам пульсового цикла
        /// </summary>
        /// <returns></returns>
        public long[][] Get_Period()
        {
            return period;
        }

        /// <summary>
        /// Получить полное число элементов
        /// </summary>
        /// <returns></returns>
        public int Get_Period_Number_Element()
        {
            return period.Length;
        }

        /// <summary>
        /// Установить значения для данных ФПГ
        /// </summary>
        /// <param name="value"></param>
        public void Set_Period(long[][] value)
        {
            period = value;
        }

        /// <summary>
        /// Получить один элемент
        /// </summary>
        /// <param name="x"> номер столбца - периода</param>
        /// <param name="y"> номер строки - элемента периода</param>
        /// <returns></returns>
        public long Get_One_Element(int x, int y)
        {
            return period[x][y];
        }

        /// <summary>
        /// Получить один период пульсового цикла
        /// </summary>
        /// <param name="x">номер столбца - периода</param>
        /// <returns></returns>
        public long[] Get_One_Period_Array(int x)
        {
            return period[x];
        }
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="init_data2">объект Initial_data</param>
        /// <param name="text">Уровень чувствительности, задаваемый ComboBox. Передается текст в Combobox,е</param>
        public Divided_by_periods_data(Initial_data init_data2, String text)
        {
            this.init_data = init_data2;
            this.combobox_3 = text;
        }


        /// <summary>
        /// Заполнить массив данными. Метод обязательный к выполнению
        /// </summary>
        public void Calculate_Data_In_Period()
        {
            int b = init_data.Get_Number_Strings();

            long[,] row1 = init_data.Get_Row1();
            long[] row3 = init_data.Get_Row3();

            int reg = init_data.REG;


            int Shift_03 = 100;
            int b0 = 0; //второй счетчик строк
            int ew = 0;//счетчик найденных максимумов
            int est = 0;
            int maxim = 0;// счетчик массива
            long[] max1_y = new long[2000]; // счетчик максимума
            long[] max1_x = new long[2000];
            long[] max1_coor = new long[2000];

            for (int u = 0; u < 2000; u++)
            {
                max1_x[u] = 1;
                max1_y[u] = 1;
            }

            // while (ew<2)
            while (b0 < b)/////////////поиск опорных точек
            {
                for (int t = 0; t < 200; t++)
                {
                    b0++;

                    if ((row3[t + 1 + est]) > max1_y[maxim])
                    {
                        max1_y[maxim] = (row3[t + 1 + est]);
                        max1_x[maxim] = row1[t + 1 + est, 0];
                        max1_coor[maxim] = t + 1 + est;
                    }
                }

                if (max1_y[maxim] > System.Convert.ToInt64(combobox_3) * 10)////////////////////!!!!!!
                {
                    ew++;// счетчик пиков производной
                    maxim++;
                }
                est = est + 200;
            }

            long[] osob_x = new long[ew + 1];// список особых точек для вывода на график
            long[] osob_y = new long[ew + 1];
            long[] osob_coor = new long[ew + 1];
            
            long[] period_all = new long[ew];
            period = new long[ew][];

            if (ew == 0)
            {
                System.Windows.Forms.MessageBox.Show("Выбран не тот канал");
                period = new long[1][];
                period[0] = new long[0];

            }
            else
            {

                for (int u = 1; u < ew; u++)
                {
                    period_all[u - 1] = max1_coor[u] - max1_coor[u - 1];
                }

                osob_x[0] = 0;
                osob_y[0] = 512;// Данные с соответствующего канала (№4)
                osob_coor[0] = 0;
                period[0] = new long[0];


                for (int w = 1; w < ew; w++)//перебираем пики
                {
                    //////////////////////////////ищем начало подъема--2

                    osob_x[w] = row1[max1_coor[w], 0];
                    osob_y[w] = row1[max1_coor[w], reg];// Данные с соответствующего канала (№4)
                    osob_coor[w] = max1_coor[w];


                    for (long i = max1_coor[w]; i > max1_coor[w] - Shift_03; i--)//2
                    {
                        if (row1[i, reg] < osob_y[w])
                        {
                            osob_x[w] = row1[i, 0];
                            osob_y[w] = row1[i, reg];// Данные с соответствующего канала (№4)
                            osob_coor[w] = i;

                        }
                    }
                }


                for (int i = 0; i < ew; i++)
                {
                    try
                    {
                        period[i] = new long[osob_coor[i + 1] - osob_coor[i]];
                        for (int j = 0; j < (osob_coor[i + 1] - osob_coor[i]); j++)
                        {

                            period[i][j] = row1[j + osob_coor[i], reg];
                        }
                    }
                    catch (Exception e)
                    {
                        period[i] = new long[0];
                    }

                }


            }///////Конец else
        } 

        /// <summary>
        /// Перевести массив с нарезанными периодами в одномерный массив.
        /// </summary>
        /// <returns></returns>
        public long[] Calculate_Period_In_Data()
        {
            int length_massiv = 0;
            for (int i = 0; i < period.Length; i++)
            {
                length_massiv = length_massiv + period[i].Length;
            }

            long[] period_all = new long[length_massiv];
            long k = 0;

            for (int i = 0; i < period.Length; i++)
            {
                for (int j = 0; j < period[i].Length; j++)
                {
                    period_all[k] = period[i][j];
                    k++;
                }
            }

            return period_all;

        }

        /// <summary>
        /// Удалить периоды нулевой длительности из всего массива
        /// </summary>
        public void Delete_Zero_In_Period()
        {
            for (int i = 1; i < period.Length; i++)
            {

                if (period[i].Length == 0)
                {
                    for (int j = i; j < period.Length - 1; i++)
                    {
                        period[j] = period[j + 1];
                    }
                }
            }
            int s = 1;

            for (int i = 1; i < period.Length; i++)
            {
                s++;

                if (period[i].Length == 0)
                {
                    break;
                }
            }

            long[][] period_new = new long[s][];

            for (int i = 0; i < period_new.Length; i++)
            {
                period_new[i] = period[i];
            }

            Set_Period(period_new);



        }

        /// <summary>
        /// Удалить нули в конце каждого периода - пульсового цикла
        /// </summary>
        public void Delete_Zero_At_End()
        {
            long[][] period_new = new long[period.Length][];

            for (int i = 0; i < period.Length; i++)
            {
                int k = period[i].Length;
                for (int j = period[i].Length - 1; j >= 0; j--)
                {
                    if (period[i][j] == 0)
                    {
                        k--;
                    }
                }

                period_new[i] = new long[k];

                for (int l = 0; l < k; l++)
                {
                    period_new[i][l] = period[i][l];
                }
            }
            Set_Period(period_new);
        }

        /// <summary>
        /// Вернуть полное число элементов периода
        /// </summary>
        /// <returns></returns>
        public long Return_Period_In_Data_Length()
        {
            int length_massiv = 0;
            for (int i = 0; i < period.Length; i++)
            {
                length_massiv = length_massiv + period[i].Length;
            }

            long[] period_all = new long[length_massiv];
            long k = 0;

            for (int i = 0; i < period.Length; i++)
            {
                for (int j = 0; j < period[i].Length; j++)
                {
                    period_all[k] = period[i][j];
                    k++;
                }
            }

            return k;
        }


        /// <summary>
        /// Дополняем все массивы периода нулями до одинаковой длины - 1000 
        /// </summary>
        /// <returns></returns>
        public long[,] Return_Periods_1000()
        {

            long[,] period1000 = new long[period.Length, 1000];

            for (int i = 0; i < period.Length; i++)
            {
                for (int j = 0; j < period[i].Length; j++)
                {
                    period1000[i, j] = period[i][j];
                }
            }

            return period1000;
        }

        /// <summary>
        /// Возвращает число элементов от первого элемента до элемента с координатами i, j 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public long Return_Length_to_Zero(int i, int j)
        {
            long a = System.Convert.ToInt64(j);

            for (int k = 0; k < i; k++)
            {
                a = a + period[k].Length;
            }

            return a;
        }

        /// <summary>
        /// Вернуть полное число элементов периода переданного из вне
        /// </summary>
        /// <returns></returns>
        public long Return_Period_In_Data_Length(long[][] period)
        {

            int length_massiv = 0;
            for (int i = 0; i < period.Length; i++)
            {
                length_massiv = length_massiv + period[i].Length;
            }

            long[] period_all = new long[length_massiv];
            long k = 0;

            for (int i = 0; i < period.Length; i++)
            {
                for (int j = 0; j < period[i].Length; j++)
                {
                    period_all[k] = period[i][j];
                    k++;
                }
            }

            return k;
        }


    }
}

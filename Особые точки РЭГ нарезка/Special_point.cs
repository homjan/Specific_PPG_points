using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Особые_точки_РЭГ_нарезка
{
    class Special_point
    {
        Initial_processing.Divided_by_periods_data Period_job;
        Initial_data initial_data;
        private int mass = 10;
        private long[,] spec_point;

        private void set_spec_point(long[,] value)
        {
            spec_point = value;
        }

        const int shift_B1 = 2;

        /// <summary>
        /// Получить массив специальных точек
        /// </summary>
        /// <returns></returns>
        public long[,] get_spec_point()
        {
            return spec_point;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="per"></param>
        /// <param name="init"></param>
        public Special_point(Initial_processing.Divided_by_periods_data per, Initial_data init)
        {
            this.Period_job = per;
            this.initial_data = init;
        }

        /// <summary>
        /// Получить особые точки РЭГ и ЭКГ, используя стандартный алгоритм
        /// </summary>
        /// <param name="combobox3"></param>
        public void return_point_EKG(String combobox3)
        {

            int b = initial_data.get_b();
            long[,] row1 = initial_data.get_row1();
            long[,] row2 = initial_data.get_row2();
            long[] row3 = initial_data.get_row3();
            long[] row4 = initial_data.get_row4();

            int reg = initial_data.REG;

            /////////////////////////

            int b0 = 0; //второй счетчик строк
            int ew = 0;//счетчик найденных максимумов
            int est = 0;
            int maxim = 0;// счетчик массива
            long[] max1_y = new long[2000]; // счетчик максимума
            long[] max1_x = new long[2000];
            long[] max1_coor = new long[2000];

            for (int u = 0; u < 1000; u++)
            {
                max1_x[u] = 1;
                max1_y[u] = 1;
            }
            // while (ew<2)
            int N_propusk = 0;

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

                if (max1_y[maxim] > System.Convert.ToInt64(combobox3) * mass)////////////////////!!!!!!
                {
                    ew++;// счетчик пиков производной
                    maxim++;
                    N_propusk = 0;
                }
                est = est + 200;
                N_propusk++;


                int period = 0;
                double period2 = 0;
                for (int u = 1; u < ew; u++)
                {
                    period2 = period2 + (max1_coor[u] - max1_coor[u - 1]);
                }
                period = System.Convert.ToInt32(Math.Round(period2 / (ew - 1)));

                double Left_shift = 0.1 * period;
                double Right_shift = 0.75 * period;

                double Shift_03n = 300;

                if (period < 400)
                {
                    Shift_03n = 0.65 * period;
                }

                int Left_Border = System.Convert.ToInt32(Math.Round(Left_shift));
                int Right_Border = System.Convert.ToInt32(Math.Round(Right_shift));

                int Shift_03 = System.Convert.ToInt32(Math.Round(Shift_03n));

                /////////////////////////////////////////////////////

                long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
                long[,] osob_y = new long[14, ew];
                long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)

                long[] EKG_max = new long[ew];
                long[] EKG_max_x = new long[ew];

                for (int w = 0; w < ew; w++) //н.у.
                {
                    for (int i = 0; i < 14; i++)
                    {
                        osob_x[i, w] = 1;
                        osob_y[i, w] = 512;
                    }
                    EKG_max[w] = 512;
                    EKG_max_x[w] = 0;
                }

                for (int w = 2; w < ew - 1; w++)//перебираем пики
                {
                    ///////////////////////////// Ищем максимум ЭКГ--1

                    for (long i = max1_coor[w]; i > max1_coor[w] - Shift_03; i--)//2
                    {
                        if (row4[i] > EKG_max[w])
                        {
                            EKG_max[w] = row4[i];
                            EKG_max_x[w] = row1[i, 0];
                        }
                    }
                }

                for (int w = 1; w < ew - 1; w++)//перебираем пики
                {
                    spec_point[0, w] = EKG_max[w];
                    spec_point[1, w] = EKG_max_x[w];
                }
            }
        }

        /// <summary>
        /// Удалить нули из периодов
        /// </summary>
        public void delete_zero_in_data()
        {
            int arre = spec_point.Length;
            int ew = arre / 15;

            for (int i = 1; i < ew; i++)
            {

                if (spec_point[2, i] == 0 && spec_point[3, i] == 0 && spec_point[4, i] == 0 && spec_point[5, i] == 0
                   )
                {
                    for (int j = i; j < ew - 1; j++)
                    {
                        for (int k = 0; k < 15; k++)
                        {
                            spec_point[k, j] = spec_point[k, j + 1];
                        }
                    }
                }
            }
            int s = 1;

            for (int i = 1; i < ew; i++)
            {
                s++;
                if (spec_point[2, i] == 0 && spec_point[3, i] == 0 && spec_point[4, i] == 0 && spec_point[5, i] == 0
                    )
                {
                    break;
                }
            }

            long[,] period_new = new long[15, s];

            for (int i = 0; i < s; i++)
            {
                for (int k = 0; k < 15; k++)
                {
                    period_new[k, i] = spec_point[k, i];
                }
            }

            set_spec_point(period_new);

        }


        public void return_osob_point(String combobox3)
        {
            int b = initial_data.get_b();

            long[,] row1 = initial_data.get_row1();

            long[,] row2 = initial_data.get_row2();
            long[] row3 = initial_data.get_row3();
            long[] row4 = initial_data.get_row4();

            int reg = initial_data.REG;

            /////////////////////////

            int b0 = 0; //второй счетчик строк
            int ew = 0;//счетчик найденных максимумов
            int est = 0;
            int maxim = 0;// счетчик массива
            long[] max1_y = new long[1000]; // счетчик максимума
            long[] max1_x = new long[1000];
            long[] max1_coor = new long[1000];

            for (int u = 0; u < 1000; u++)
            {
                max1_x[u] = 1;
                max1_y[u] = 1;
            }
            // while (ew<2)
            while (b0 < b)/////////////поиск опорных точек
                          // for (int est = 0; est < 120; est = est + 40)
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

                if (max1_y[maxim] > System.Convert.ToInt64(combobox3) * 10)////////////////////!!!!!!
                {
                    ew++;// счетчик пиков производной
                    maxim++;
                }
                est = est + 200;
            }
                        
            ////////////////////////////////////////////////////////////////
            int period = 0;
            double period2 = 0;
            for (int u = 1; u < ew; u++)
            {
                period2 = period2 + (max1_coor[u] - max1_coor[u - 1]);
            }
            period = System.Convert.ToInt32(Math.Round(period2 / (ew - 1)));

            double Left_shift = 0.1 * period;
            double Right_shift = 0.75 * period;

            double Shift_005n = 30;//разные сдвиги
            double Shift_01n = 45;
            double Shift_015n = 100;
            double Shift_02n = 120;
            double Shift_025n = 150;
            double Shift_03n = 180;
            double Shift_04n = 240;
            double Shift_05n = 300;
            double Shift_065n = 400;
            double Shift_075n = 450;


            if (period < 400)

            {
                Shift_005n = 0.05 * period;//разные сдвиги
                Shift_01n = 0.1 * period;
                Shift_015n = 0.15 * period;
                Shift_02n = 0.2 * period;
                Shift_025n = 0.25 * period;
                Shift_03n = 0.3 * period;
                Shift_04n = 0.4 * period;
                Shift_05n = 0.5 * period;
                Shift_065n = 0.65 * period;
                Shift_075n = 0.75 * period;
            }

            int Left_Border = System.Convert.ToInt32(Math.Round(Left_shift));
            int Right_Border = System.Convert.ToInt32(Math.Round(Right_shift));

            int Shift_005 = System.Convert.ToInt32(Math.Round(Shift_005n));
            int Shift_01 = System.Convert.ToInt32(Math.Round(Shift_01n));
            int Shift_015 = System.Convert.ToInt32(Math.Round(Shift_015n));
            int Shift_02 = System.Convert.ToInt32(Math.Round(Shift_02n));
            int Shift_025 = System.Convert.ToInt32(Math.Round(Shift_025n));
            int Shift_03 = System.Convert.ToInt32(Math.Round(Shift_03n));
            int Shift_04 = System.Convert.ToInt32(Math.Round(Shift_04n));
            int Shift_05 = System.Convert.ToInt32(Math.Round(Shift_05n));
            int Shift_065 = System.Convert.ToInt32(Math.Round(Shift_065n));
            int Shift_075 = System.Convert.ToInt32(Math.Round(Shift_075n));

            /////////////////////////////////////////////////////

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)
            long[] schet_sum = new long[15];// список особых точек
            double[] schet_sum_final = new double[15];

            double[,] time_dif = new double[3, ew];
            double[] time_dif_sum = new double[3];
            double[] time_dif_sum_final = new double[3];

            long[] EKG_max = new long[ew];
            long[] EKG_max_x = new long[ew];

            long[] Begin_coor = new long[ew];
            long[] x_min_1 = new long[ew];
            long[] y_min_1 = new long[ew];

            long[] x_min_2 = new long[ew];
            long[] y_min_2 = new long[ew];

            long[] max_1_1 = new long[ew];
            long[] coor_max_1_1 = new long[ew];

            long[] local_min = new long[ew];
            long[] coor_local_min = new long[ew];
            long[] nomer_local_min = new long[ew];

            long[] local_max = new long[ew];
            long[] coor_local_max = new long[ew];

            long[] local_min_diff = new long[ew];
            long[] coor_local_min_diff = new long[ew];
            long[] nomer_local_min_diff = new long[ew];

            long[] global_max = new long[ew];
            long[] coor_global_max = new long[ew];

            long[] chetv_ot_end = new long[ew];
            long[] coor_chetv_ot_end = new long[ew];

            bool B3_diff = false;
            long Diff_3_max = 0;
            long Diff_3_max_coor = 0;

            for (int w = 0; w < ew; w++) //н.у.
            {
                for (int i = 0; i < 14; i++)
                {
                    osob_x[i, w] = 1;
                    osob_y[i, w] = 512;
                }
                x_min_1[w] = 0;//3
                y_min_1[w] = 512;
                x_min_2[w] = 0;
                y_min_2[w] = 512;

                max_1_1[w] = 1;//7
                coor_max_1_1[w] = 0;

                local_min[w] = 512;//10
                coor_local_min[w] = 0;

                local_max[w] = 512;//11
                coor_local_max[w] = 0;

                local_min_diff[w] = 512;//13
                coor_local_min_diff[w] = 0;

                global_max[w] = 512;
                coor_global_max[w] = 0;

                EKG_max[w] = 512;
                EKG_max_x[w] = 0;
            }

            for (int w = 2; w < ew - 2; w++)//перебираем пики
            {

                //////////////////////////////ищем начало подъема--2

                osob_x[1, w] = row1[max1_coor[w], 0];
                osob_y[1, w] = row1[max1_coor[w], reg];// Данные с соответствующего канала (№4)

                for (long i = max1_coor[w]; i > max1_coor[w] - Shift_03; i--)//2
                {

                    if (row1[i, reg] < osob_y[1, w])
                    {
                        osob_x[1, w] = row1[i + shift_B1, 0];
                        osob_y[1, w] = row1[i + shift_B1, reg];// Данные с соответствующего канала (№4)
                        Begin_coor[w] = i + shift_B1;
                        y_min_2[w] = row1[i + shift_B1, reg];
                        x_min_2[w] = row1[i + shift_B1, 0];
                    }
                }

               
                ////////////////////////////////////  // Ищем 1 максимум--7

                for (long i = max1_coor[w]; i < max1_coor[w] + Shift_03; i++)
                {
                    if (row1[i, reg] > max_1_1[w])
                    {
                        max_1_1[w] = row1[i, reg];
                        coor_max_1_1[w] = row1[i, 0];
                    }
                }

                //Ищем локальный минимум производной---12

                for (long i = max1_coor[w] + Shift_015; i < max1_coor[w] + Shift_025; i++)
                {
                    if (row3[i] < local_min_diff[w])
                    {
                        local_min_diff[w] = row3[i + 1];
                        coor_local_min_diff[w] = row1[i + 1, 0];
                        nomer_local_min_diff[w] = i + 1;
                    }
                }
                
                //ищем первый локальный минимум---10

                for (long i = nomer_local_min_diff[w]; i < nomer_local_min_diff[w] + Shift_03; i++)
                {
                    if (row3[i + 1] > 0)
                    {
                        local_min[w] = row1[i, reg];
                        coor_local_min[w] = row1[i, 0];
                        nomer_local_min[w] = i;
                        B3_diff = true;
                        break;
                    }
                }

                if (B3_diff == false)
                {
                    Diff_3_max = row3[nomer_local_min_diff[w]];
                    Diff_3_max_coor = nomer_local_min_diff[w];

                    for (long i = nomer_local_min_diff[w]; i < nomer_local_min_diff[w] + Shift_03; i++)
                    {
                        if (Diff_3_max < row3[i])
                        {
                            Diff_3_max = row3[i];
                            Diff_3_max_coor = i;
                        }
                    }

                    local_min[w] = row1[Diff_3_max_coor, reg];
                    coor_local_min[w] = row1[Diff_3_max_coor, 0];
                    nomer_local_min[w] = Diff_3_max_coor;

                }

                B3_diff = false;
                // ищем 2 локальный максимум  ---11

                local_max[w] = local_min[w];
                coor_local_max[w] = coor_local_min[w];

                for (long i = nomer_local_min[w] + Shift_005 + 10; i < nomer_local_min[w] + Shift_025; i++)///////////////
                {
                    if (i == nomer_local_min[w] + 10)
                    {
                        local_max[w] = row1[i + 1, reg];
                        coor_local_max[w] = row1[i + 1, 0];
                    }

                    if (row1[i + 1, reg] > local_max[w])
                    {
                        local_max[w] = row1[i + 1, reg];
                        coor_local_max[w] = row1[i + 1, 0];
                    }
                }

                //Ищем глобальный максимум ---14

                for (long i = max1_coor[w] - Shift_01; i < max1_coor[w] + Shift_075; i++)
                {
                    if (row1[i, reg] > global_max[w])
                    {
                        global_max[w] = row1[i, reg];
                        coor_global_max[w] = row1[i, 0];
                    }
                }

                //Ищем амплитуда за 0,25 до конца
                chetv_ot_end[w] = row1[max1_coor[w] + Shift_065, reg];
                coor_chetv_ot_end[w] = row1[max1_coor[w] + Shift_065, 0];


                if (w < ew - 1)
                {
                    time_dif[0, w] = max1_x[w + 1] - max1_x[w];
                }
                time_dif[1, w] = coor_global_max[w] - max1_x[w];
                time_dif[2, w] = max1_x[w] - osob_x[1, w];

                schet[2, w] = osob_y[1, w];// минимум В1
                schet[3, w] = osob_x[1, w];// положение минимума В1 - начала отсчета

                schet[4, w] = max_1_1[w] - y_min_2[w];// максимум В2
                schet[5, w] = coor_max_1_1[w];// положение максимума В2 - начала отсчета - EKG_max_x[w]

                schet[6, w] = local_min[w] - y_min_2[w];// минимум В3
                schet[7, w] = coor_local_min[w];// положение минимума В3 - начала отсчета- EKG_max_x[w]

                schet[8, w] = local_max[w] - y_min_2[w];// максимум В4
                schet[9, w] = coor_local_max[w];// положение максимума В4 - начала отсчета - EKG_max_x[w]

                schet[10, w] = y_min_2[w];
            }
            
            spec_point = schet;
        }

      /// <summary>
      /// Учесть дыхательный сдвиг
      /// </summary>
      /// <param name="sche">массив с обыми точками</param>
      /// <param name="ew">Число особых точек</param>
      /// <returns></returns>
        public long[,] shift_osob_point(long[,] sche, long ew)
        {
            long[,] schet = sche;
            long test = 0;
            long test2 = 0;
            for (int w = 0; w < ew; w++)
            {
                if (w > 2)
                {
                    test = schet[8, w - 1] - Shift_BX(schet[3, w - 1], schet[3, w], schet[9, w - 1], schet[2, w - 1], schet[2, w]);// максимум В4
                    test2 = schet[8, w - 1];


                    schet[4, w - 1] = schet[4, w - 1] - Shift_BX(schet[3, w - 1], schet[3, w], schet[5, w - 1], schet[2, w - 1], schet[2, w]);// максимум В2

                    schet[6, w - 1] = schet[6, w - 1] - Shift_BX(schet[3, w - 1], schet[3, w], schet[7, w - 1], schet[2, w - 1], schet[2, w]); // минимум В3

                    schet[8, w - 1] = schet[8, w - 1] - Shift_BX(schet[3, w - 1], schet[3, w], schet[9, w - 1], schet[2, w - 1], schet[2, w]);// максимум В4


                }
            }

            return schet;
        }

        /// <summary>
        /// Рассчитать дыхательтный сдвиг
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="x3_b"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public long Shift_BX(long x1, long x2, long x3_b, long y1, long y2)
        {
            double shift_y = 0;

            if (x2 == x1)
            {
                shift_y = 0;
            }
            else
            {
                shift_y = (System.Convert.ToDouble(x3_b - x1) / System.Convert.ToDouble(x2 - x1)) * System.Convert.ToDouble(y2 - y1);
            }

            long shift_2 = System.Convert.ToInt64(shift_y);

            return shift_2;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Особые_точки_РЭГ_нарезка
{
    class Initial_data
    {
        const int numerical = 60;

        private long[,] row1;
        private long[,] row2;
        private long[] row3;
        private long[] row4;

        private long[][] row_divided;

        private int b;

        public int REG;
        public int EKG;

        const int potok2 = 12;

        String name_file;

       Reader_data re_data;

      
        /// <summary>
        /// открывает файл c именем "name_File" и заполняет содержимым 2-мерный массив
        /// </summary>
        /// <param name="name_File">Имя файла</param>
        /// <param name="reg">номер канала РЭГ (Или ФПГ)</param>
        /// <param name="ekg">Номер канала с ЭКГ</param>
        public Initial_data(String name_File, int reg, int ekg) //
        {
            this.name_file = name_File;
            this.REG = reg;
            this.EKG = ekg;

            re_data = new Reader_data(name_file);

            row1 = re_data.Return_Read_Array();
            b = re_data.Return_Read_String();

            row2 = new long[b + 200, potok2];
            row3 = new long[b + 200];
            row4 = new long[b + 200];

            Average_Canal_REG_Row3();

        }

        /// <summary>
        /// открывает файл c именем ss и заполняет содержимым 2-мерный массив
        /// </summary>
        /// <param name="ss">Имя файла</param>
        /// <param name="reg">номер канала РЭГ (Или ФПГ)</param>
        /// <param name="ekg">Номер канала с ЭКГ</param>
        /// <param name="div">Отметка о необходимости предварительно нарезать периоды</param>
        public Initial_data(String ss, int reg, int ekg, bool div) //
        {
            this.name_file = ss;
            this.REG = reg;
            this.EKG = ekg;

            re_data = new Reader_data(name_file);

            row_divided = re_data.Return_Read_Array_Divided_Data();

        }


        /// <summary>
        /// сдвинуть к нулю первый столбец (c временем)
        /// </summary>
        public void Shift_Row1_To_Time_0() 
        {
            for (int j = 3; j < b; j++)
            {
                row1[j, 0] = row1[j, 0] - row1[2, 0];
            }
            row1[2, 0] = 0;
            row1[1, 0] = 0;
            row1[0, 0] = 0;

        }

        /// <summary>
        /// Сгладить по семи точкам все данные
        /// </summary>
        public void Smoothe_Row1() 
        {
            long[,] rw11 = row1;

            for (int d = 1; d < potok2; d++)
            {
                for (int q = 4; q < b - 4; q++)
                {
                    row1[q, d] = (rw11[q + 3, d] + rw11[q + 2, d] + rw11[q + 1, d] + rw11[q, d] + rw11[q - 1, d] + rw11[q - 2, d] + rw11[q - 3, d]) / 7;
                }
            }

        }

        /// <summary>
        ///  Рассчитать и усилить производную
        /// </summary>
        public void Calculate_Derivative_Row2()
        {
            for (int d = 1; d <= potok2; d++)
            {
                for (int q = 3; q < b - 3; q++)
                {
                    row2[q, d - 1] = 1000000 * (row1[q + 1, d] - row1[q - 1, d]) / (row1[q + 1, 0] - row1[q - 1, 0]);
                }
            }
        }

        /// <summary>
        /// Усреднить производную
        /// </summary>
        public void Average_Canal_REG_Row3()
        {
            for (int q = 4; q < b - 4; q++)
            {
                row3[q] = (row2[q + 3, REG - 1] + row2[q + 2, REG - 1] + row2[q + 1, REG - 1] + row2[q, REG - 1] + row2[q - 1, REG - 1] + row2[q - 2, REG - 1] + row2[q - 3, REG - 1]) / 7;
            }
        }

        /// <summary>
        /// Сгладить ЭКГ, чтобы убрать наводку 50 Гц
        /// </summary>
        public void Smoothing_Ekg_Row4()
        {
            for (int q = 3; q < b - 8; q++)
            {
                row4[q] = (row1[q, EKG] + row1[q + 7, EKG]) / 2;
            }
        }

        /// <summary>
        /// Записать данные (необработанные) в файл
        /// </summary>
        /// <param name="name_file">Имя файла</param>
        public void Write_In_File_Row1(String name_file)
        {
            StreamWriter rw2 = new StreamWriter(name_file);
            for (int j = 3; j < b; j++)
            {
                rw2.Write(System.Convert.ToString(row1[j, 0]));

                for (int z = 0; z < potok2; z++)
                {
                    rw2.Write(System.Convert.ToString("\t"));
                    rw2.Write(System.Convert.ToString(row1[j, z + 1]));
                }

                rw2.WriteLine();
            }

            rw2.Close();
        }


        //Геттеры
        /// <summary>
        /// Получить исходные данные
        /// </summary>
        /// <returns></returns>
        public long[,] Get_Row1()
        {
            return row1;
        }

        /// <summary>
        /// Получить производную
        /// </summary>
        /// <returns></returns>
        public long[,] Get_Row2()
        {
            return row2;
        }

        /// <summary>
        /// Получить усредненную производную канала РЭГ
        /// </summary>
        /// <returns></returns>
        public long[] Get_Row3()
        {
            return row3;
        }

        /// <summary>
        /// Получить сглаженные данные канала ЭКГ
        /// </summary>
        /// <returns></returns>
        public long[] Get_Row4()
        {
            return row4;
        }

        /// <summary>
        /// Получить число строк в последовательности
        /// </summary>
        /// <returns></returns>
        public int Get_Number_Strings()
        {
            return b;
        }

        /// <summary>
        /// Получить разделенную на периоды последовательность
        /// </summary>
        /// <returns></returns>
        public long[][] Get_Row_Divided()
        {
            return row_divided;
        }
        /// <summary>
        /// Получить один элемент последовательности
        /// </summary>
        /// <param name="b"> номер элемента</param>
        /// <param name="kanal">номер канала</param>
        /// <returns></returns>
        public long Get_Row1_X_Y(int b, int kanal)
        {
            return row1[b, kanal];
        }

        /// <summary>
        /// Задать исходные данные
        /// </summary>
        /// <param name="row_1_new"></param>
        public void Set_Row1(long[,] row_1_new)
        {
            row1 = row_1_new;
        }
        /// <summary>
        /// Задать число строк
        /// </summary>
        /// <param name="bx"></param>
        public void Set_Number_Strings(int bx)
        {
            b = bx;
        }







    }
}

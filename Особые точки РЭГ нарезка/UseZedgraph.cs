using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZedGraph;

namespace Особые_точки_РЭГ_нарезка
{
    class UseZedgraph
    {
        const int potok2 = 13; //Общее Число потоков (работае только 4 + время)
        long shift_grafh = 200;
        long shift_grafh_ekg = 200;
        long maximum = 0;
        long minimum = 1024;

        private GraphPane pane;
        ZedGraphControl zedgraph; 
        Initial_data initdata;
       
        public UseZedgraph(ZedGraphControl zedd)
        {
            this.zedgraph = zedd;
            pane = zedd.GraphPane;

        }

        public UseZedgraph(ZedGraphControl zedd, Initial_data data)
        {
            this.zedgraph = zedd;
            pane = zedd.GraphPane;
            this.initdata = data;
        }

       
        /// <summary>
        /// Очистить график
        /// </summary>
        public void ClearGraph()
        {
            pane.CurveList.Clear();
        }

        /// <summary>
        /// Построить график с 4 каналов
        /// </summary>
        /// <param name="xxx">массив данных (обычно row1)</param>
        /// <param name="b"> число строк</param>
        public void MakeGraph_4_Canal(long[,] xxx, int b)
        {
            PointPairList f1_list = new PointPairList();
            PointPairList f2_list = new PointPairList();
            PointPairList f3_list = new PointPairList();
            PointPairList f4_list = new PointPairList();

            for (int i = 3; i < b; i++)
            {
                f1_list.Add(xxx[i, 0] / 1000, xxx[i, 1]);
                f2_list.Add(xxx[i, 0] / 1000, xxx[i, 2]);
                f3_list.Add(xxx[i, 0] / 1000, xxx[i, 3]);
                f4_list.Add(xxx[i, 0] / 1000, xxx[i, 4]);
            }

            LineItem myCurve1 = pane.AddCurve("Канал1", f1_list, Color.Blue, SymbolType.None);
            LineItem myCurve2 = pane.AddCurve("Канал2", f2_list, Color.Red, SymbolType.None);
            LineItem myCurve3 = pane.AddCurve("Канал3", f3_list, Color.Green, SymbolType.None);
            LineItem myCurve4 = pane.AddCurve("Канал4", f4_list, Color.Black, SymbolType.None);

        }

        /// <summary>
        /// Установить подписи осей и заглавие
        /// </summary>
        /// <param name="Xaxis">ось х</param>
        /// <param name="Yaxis">ось y</param>
        /// <param name="Title_text">Заглавие</param>
        public void Install_Pane(String Xaxis, String Yaxis, String Title_text)
        {
            pane.Title.Text = Title_text;
            pane.XAxis.Title.Text = Xaxis;
            pane.YAxis.Title.Text = Yaxis;
        }


        /// <summary>
        /// Построить график от выбранных каналов ЭКГ и РЭГ + производная РЭГ
        /// </summary>
        public void MakeGraph_On_Chosen_Canal()
        {

            long[,] row_1 = initdata.Get_Row1();
            long[] row_3 = initdata.Get_Row3();
            long[] row_4 = initdata.Get_Row4();
            int b = initdata.Get_Number_Strings();


            for (int y = 100; y < b - 10; y++)
            {
                if (maximum < row_1[y, initdata.REG])
                {
                    maximum = row_1[y, initdata.REG];
                }

                if (minimum > row_1[y, initdata.REG])
                {
                    minimum = row_1[y, initdata.REG];
                }

            }

            if ((maximum - minimum) < 200)
            {
                shift_grafh = 200;
                shift_grafh_ekg = 200;
            }
            else if ((maximum - minimum) > 500)
            {
                shift_grafh = -500;
                shift_grafh_ekg = 400;
            }
            else if ((maximum - minimum) > 1000)
            {
                shift_grafh = -5500;
                shift_grafh_ekg = 5500;
            }
            else
            {
                shift_grafh_ekg = 200;
                shift_grafh = -300;
            }


            // Создадим список точек для кривой f1(x)
            PointPairList f1_list = new PointPairList();
            PointPairList f2_list = new PointPairList();
            PointPairList f3_list = new PointPairList();
            PointPairList f4_list = new PointPairList();
            PointPairList f5_list_diff = new PointPairList();

            // Заполним массив точек для кривой f1-3(x)
            for (int y = 3; y < b - 10; y++)
            {
                f1_list.Add(row_1[y, 0] / 1000, row_4[y] + (shift_grafh_ekg));
                f2_list.Add(row_1[y, 0] / 1000, 570);
                f3_list.Add(row_1[y, 0] / 1000, shift_grafh);

                f4_list.Add(row_1[y, 0] / 1000, row_1[y, initdata.REG]);
                f5_list_diff.Add(row_1[y, 0] / 1000, row_3[y] / 10 + shift_grafh);
            }

            pane.XAxis.Title.Text = "t, мc";
            pane.YAxis.Title.Text = "R, Ом";
            pane.Title.Text = "Данные";

            LineItem f1_curve = pane.AddCurve("ЭКГ", f1_list, Color.Blue, SymbolType.None);
            LineItem f2_curve = pane.AddCurve("", f2_list, Color.Black, SymbolType.None);
            LineItem f3_curve = pane.AddCurve("", f3_list, Color.Black, SymbolType.None);
            LineItem f4_curve = pane.AddCurve(" РЭГ", f4_list, Color.Red, SymbolType.None);
            LineItem f5_curve_diff = pane.AddCurve("Производная РЭГ", f5_list_diff, Color.Green, SymbolType.None);


        }

        /// <summary>
        /// Построить график только с первого канала
        /// </summary>
        public void MakeGraph_On_Canal_Only_One_Graph()
        {
            long[,] row_1 = initdata.Get_Row1();
            int b = initdata.Get_Number_Strings();

            PointPairList f2_list = new PointPairList();
            PointPairList f4_list = new PointPairList();
                      
            for (int q = 1; q < b; q++)// считаем производную
            {
                f2_list.Add(row_1[q, 0] / 1000, 570);
                f4_list.Add(row_1[q, 0] / 1000, row_1[q, 1]);               
            }

            pane.XAxis.Title.Text = "t, мc";
            pane.YAxis.Title.Text = "R, Ом";
            pane.Title.Text = "Данные";

            LineItem f2_curve = pane.AddCurve("", f2_list, Color.Black, SymbolType.None);
            LineItem f4_curve = pane.AddCurve(" РЭГ", f4_list, Color.Red, SymbolType.None);
        }

        /// <summary>
        /// Построить график с выбранного канала в определенных временных границах
        /// </summary>
        /// <param name="number_kanal">номер канала</param>
        /// <param name="left_border">Левая граница</param>
        /// <param name="right_border">Правая граница</param>
        public void MakeGraph_On_Chosen_Canal_Only_One_Graph(int number_kanal, long left_border, long right_border)
        {
            long[,] row_1 = initdata.Get_Row1();
            int b = initdata.Get_Number_Strings();
         
            PointPairList f2_list = new PointPairList();
            PointPairList f4_list = new PointPairList();

            for (int i = 0; i < b; i++)// считаем производную
            {
                if (row_1[i, 0] >= left_border && row_1[i, 0] < right_border) {

                f2_list.Add(row_1[i, 0] / 1000, 570);
                f4_list.Add(row_1[i, 0] / 1000, row_1[i, number_kanal]);
                }

                if (row_1[i, 0] > right_border) {
                    break;
                }                   
            }

            pane.XAxis.Title.Text = "t, мc";
            pane.YAxis.Title.Text = "R, Ом";
            pane.Title.Text = "Данные";

            LineItem f2_curve = pane.AddCurve("", f2_list, Color.Black, SymbolType.None);
            LineItem f4_curve = pane.AddCurve(" РЭГ", f4_list, Color.Red, SymbolType.None);
        }

        /// <summary>
        /// Построить график с особыми точками
        /// </summary>
        /// <param name="osob_x"></param>
        /// <param name="osob_y"></param>
        /// <param name="ew"></param>
        public void MakeGraph_Special_Point(long[,] osob_x, long[,] osob_y, int ew)
        {

            // Выводим точки на экран
            PointPairList list5 = new PointPairList();
            PointPairList list6 = new PointPairList();
            PointPairList list7 = new PointPairList();
            PointPairList list8 = new PointPairList();
            PointPairList list9 = new PointPairList();

            for (int w = 2; w < ew - 2; w++)
            {               
                list5.Add(osob_x[1, w] / 1000, osob_y[1, w]);
                list6.Add(osob_x[2, w] / 1000, osob_y[2, w]);
                list7.Add(osob_x[3, w] / 1000, osob_y[3, w]);
                list8.Add(osob_x[4, w] / 1000, osob_y[4, w]);

                list9.Add(osob_x[0, w] / 1000, osob_y[0, w] + (shift_grafh_ekg));


            }
            LineItem myCurve5 = pane.AddCurve("B1", list5, Color.Blue, SymbolType.Diamond);
            LineItem myCurve6 = pane.AddCurve("B2", list6, Color.Black, SymbolType.Diamond);
            LineItem myCurve7 = pane.AddCurve("B3", list7, Color.DarkRed, SymbolType.Diamond);
            LineItem myCurve8 = pane.AddCurve("B4", list8, Color.Green, SymbolType.Diamond);
            LineItem myCurve9 = pane.AddCurve("ЭКГ", list9, Color.Brown, SymbolType.Diamond);

            // !!!
            // У кривой линия будет невидимой
            myCurve5.Line.IsVisible = false;
            myCurve6.Line.IsVisible = false;
            myCurve7.Line.IsVisible = false;
            myCurve8.Line.IsVisible = false;
            myCurve9.Line.IsVisible = false;

            // !!!
            // Цвет заполнения отметок (ромбиков) - голубой
            myCurve5.Symbol.Fill.Color = Color.Blue;
            myCurve6.Symbol.Fill.Color = Color.Black;
            myCurve7.Symbol.Fill.Color = Color.DarkRed;
            myCurve8.Symbol.Fill.Color = Color.Green;
            myCurve9.Symbol.Fill.Color = Color.Brown;

            // !!!
            // Тип заполнения - сплошная заливка
            myCurve5.Symbol.Fill.Type = FillType.Solid;
            myCurve6.Symbol.Fill.Type = FillType.Solid;
            myCurve7.Symbol.Fill.Type = FillType.Solid;
            myCurve8.Symbol.Fill.Type = FillType.Solid;
            myCurve9.Symbol.Fill.Type = FillType.Solid;

            // !!!
            // Размер ромбиков
            myCurve5.Symbol.Size = 8;
            myCurve6.Symbol.Size = 8;
            myCurve7.Symbol.Size = 8;
            myCurve8.Symbol.Size = 8;
            myCurve9.Symbol.Size = 8;

            pane.YAxis.MajorGrid.IsZeroLine = false;

        }

        /// <summary>
        /// Построить график с особыми точками без экг
        /// </summary>
        /// <param name="osob_x"></param>
        /// <param name="B2"></param>
        /// <param name="B3"></param>
        /// <param name="B4"></param>
        public void MakeGraph_Special_Point_Without_EKG_One_Period(long[,] osob_x, int B2, int B3, int B4)
        {

            // Выводим точки на экран
            PointPairList list5 = new PointPairList();
            PointPairList list6 = new PointPairList();
            PointPairList list7 = new PointPairList();
            PointPairList list8 = new PointPairList();


            list5.Add(osob_x[1, 0] / 1000, osob_x[1, 1]);
            list6.Add(osob_x[B2, 0] / 1000, osob_x[B2, 1]);
            list7.Add(osob_x[B3, 0] / 1000, osob_x[B3, 1]);
            list8.Add(osob_x[B4, 0] / 1000, osob_x[B4, 1]);


            LineItem myCurve5 = pane.AddCurve("B1", list5, Color.Blue, SymbolType.Diamond);
            LineItem myCurve6 = pane.AddCurve("B2", list6, Color.Black, SymbolType.Diamond);
            LineItem myCurve7 = pane.AddCurve("B3", list7, Color.DarkRed, SymbolType.Diamond);
            LineItem myCurve8 = pane.AddCurve("B4", list8, Color.Green, SymbolType.Diamond);

            // !!!
            // У кривой линия будет невидимой
            myCurve5.Line.IsVisible = false;
            myCurve6.Line.IsVisible = false;
            myCurve7.Line.IsVisible = false;
            myCurve8.Line.IsVisible = false;

            // !!!
            // Цвет заполнения отметок (ромбиков) - голубой
            myCurve5.Symbol.Fill.Color = Color.Blue;
            myCurve6.Symbol.Fill.Color = Color.Black;
            myCurve7.Symbol.Fill.Color = Color.DarkRed;
            myCurve8.Symbol.Fill.Color = Color.Green;

            // !!!
            // Тип заполнения - сплошная заливка
            myCurve5.Symbol.Fill.Type = FillType.Solid;
            myCurve6.Symbol.Fill.Type = FillType.Solid;
            myCurve7.Symbol.Fill.Type = FillType.Solid;
            myCurve8.Symbol.Fill.Type = FillType.Solid;

            // !!!
            // Размер ромбиков
            myCurve5.Symbol.Size = 12;
            myCurve6.Symbol.Size = 12;
            myCurve7.Symbol.Size = 12;
            myCurve8.Symbol.Size = 12;

            pane.YAxis.MajorGrid.IsZeroLine = false;

        }

        /// <summary>
        /// Построить график  особых точек только одного периода
        /// </summary>
        /// <param name="osob_x">Массив с положением точек по оси х</param>
        /// <param name="osob_y">Массив с положением точек по оси y</param>
        /// <param name="B2"></param>
        /// <param name="B3"></param>
        /// <param name="B4"></param>
        /// <param name="number"> номер периода</param>
        public void MakeGraph_Special_Point_Without_EKG_One_Period(long[,] osob_x, long[,] osob_y, int B2, int B3, int B4, int number)
        {

            // Выводим точки на экран
            PointPairList list5 = new PointPairList();
            PointPairList list6 = new PointPairList();
            PointPairList list7 = new PointPairList();
            PointPairList list8 = new PointPairList();


            list5.Add(osob_x[1, number] / 1000, osob_y[1, number]);
            list6.Add(osob_x[2, number] / 1000, osob_y[2, number]);
            list7.Add(osob_x[3, number] / 1000, osob_y[3, number]);
            list8.Add(osob_x[4, number] / 1000, osob_y[4, number]);


            LineItem myCurve5 = pane.AddCurve("B1", list5, Color.Blue, SymbolType.Diamond);
            LineItem myCurve6 = pane.AddCurve("B2", list6, Color.Black, SymbolType.Diamond);
            LineItem myCurve7 = pane.AddCurve("B3", list7, Color.DarkRed, SymbolType.Diamond);
            LineItem myCurve8 = pane.AddCurve("B4", list8, Color.Green, SymbolType.Diamond);

            // !!!
            // У кривой линия будет невидимой
            myCurve5.Line.IsVisible = false;
            myCurve6.Line.IsVisible = false;
            myCurve7.Line.IsVisible = false;
            myCurve8.Line.IsVisible = false;

            // !!!
            // Цвет заполнения отметок (ромбиков) - голубой
            myCurve5.Symbol.Fill.Color = Color.Blue;
            myCurve6.Symbol.Fill.Color = Color.Black;
            myCurve7.Symbol.Fill.Color = Color.DarkRed;
            myCurve8.Symbol.Fill.Color = Color.Green;

            // !!!
            // Тип заполнения - сплошная заливка
            myCurve5.Symbol.Fill.Type = FillType.Solid;
            myCurve6.Symbol.Fill.Type = FillType.Solid;
            myCurve7.Symbol.Fill.Type = FillType.Solid;
            myCurve8.Symbol.Fill.Type = FillType.Solid;

            // !!!
            // Размер ромбиков
            myCurve5.Symbol.Size = 12;
            myCurve6.Symbol.Size = 12;
            myCurve7.Symbol.Size = 12;
            myCurve8.Symbol.Size = 12;

            pane.YAxis.MajorGrid.IsZeroLine = false;

        }


        /// <summary>
        /// Обновить график
        /// </summary>
        public void ResetGraph()
        {
            zedgraph.AxisChange();
            zedgraph.Invalidate();
        }

        /// <summary>
        /// Сохранить график
        /// </summary>
        public void SaveGraph()
        {
            zedgraph.SaveAsBitmap();
        }

        /// <summary>
        /// Очистить все
        /// </summary>
        public void ClearAll()
        {
            pane.CurveList.Clear();

            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MaxAuto = true;

            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;

            zedgraph.AxisChange();
            zedgraph.Invalidate();

        }

        /// <summary>
        /// Установить определенный интервал
        /// </summary>
        /// <param name="value_min"></param>
        /// <param name="value_max"></param>
        public void Shift_Axis(double value_min, double value_max)
        {
            // Устанавливаем интересующий нас интервал по оси Y
            pane.XAxis.Scale.Min = value_min;
            pane.XAxis.Scale.Max = value_max;

            pane.YAxis.Scale.Min = 0;
            pane.YAxis.Scale.Max = 1250;
          
            zedgraph.AxisChange();
            zedgraph.Invalidate();

        }


    }
}

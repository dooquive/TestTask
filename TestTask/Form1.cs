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
using System.Globalization;

namespace TestTask
{
    public partial class TestTask : Form
    {
        //Диалог открытия файла
        OpenFileDialog LoadDlg;

        // Массив id пользователей
        string[] UsersIds;
        // Массив count
        int[] UsersCount;
        // Массив стран
        string[] UsersCountries;
        // Общее количество строк в считываемом файле
        int NumLines;

        // Индекс записи в массиве
        int Ind;

        public TestTask()
        {
            InitializeComponent();

            // Диалог открытия файла
            LoadDlg = new OpenFileDialog();
            // Установка начальных значений и фильтров
            string p = Path.GetDirectoryName(Application.ExecutablePath).Replace("\\TestTask\\bin\\Debug", string.Empty);
            string CombinedPath = p + "\\Files";
            LoadDlg.InitialDirectory = CombinedPath;
            LoadDlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            LoadDlg.FilterIndex = 2;
            LoadDlg.RestoreDirectory = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Имя файла
            string FName = null;
            if (LoadDlg.ShowDialog() == DialogResult.OK)
            {
                FName = LoadDlg.FileName;
                // Получение пути к файлу по имени
                string FPath = Path.GetFullPath(FName);
                // Поток для чтения из файла
                FileStream f = new FileStream(FPath, FileMode.Open, FileAccess.Read);
        
                NumLines = File.ReadAllLines(FPath).Length;
                // Выделение памяти под массивы
                UsersIds       = new string[NumLines];
                UsersCount     = new int[NumLines];
                UsersCountries = new string[NumLines];

                // Текущая считываемая строка
                string CurrentStr;
                // Массив подстрок CurrentStr
                string[] Str;
                // Вспомогательная переменная для проверки формата поля count
                int Num;
                Ind = 0;

                // Чтение данных из потока
                StreamReader rd = new StreamReader(f);

                while (rd.EndOfStream != true)
                {
                    CurrentStr = rd.ReadLine();
                    Str = CurrentStr.Split(';');

                    if (Str.Length == 3)
                    {
                        if (int.TryParse(Str[1], out Num) && Num >=0)
                        {
                            if (Str[2].All(ch => Char.IsLetter(ch)))
                            {
                                UsersIds[Ind]       = Str[0];
                                UsersCount[Ind]     = Num;
                                UsersCountries[Ind] = Str[2];

                                // Добавление считанной строки в таблицу (объект DataGridView)
                                dgvSourceTable.Rows.Add(UsersIds[Ind], UsersCount[Ind], UsersCountries[Ind]);

                                Ind++;
                            } else { continue; }
                        } else { continue; }
                    } else { continue; }
                }

                rd.Close();
            }
        }

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int SumCount;
            int NumUniqIds;
            string[] TempIds;
            string temp;

            for (int i = 0; i < NumLines; i++)
            {
                if(UsersCountries[i] != null)
                {
                    temp = UsersIds[i];
                    // Сумма по count для фиксированной страны
                    SumCount = UsersCount[i];

                    for (int j = i + 1; j < NumLines; j++)
                    {
                        // Проверка совпадения стран (без учёта регистра)
                        if (string.Compare(UsersCountries[i], UsersCountries[j], true) == 0) 
                        {
                            // Подчёт суммы по count
                            SumCount += UsersCount[j];

                            temp += "," + UsersIds[j];

                            // "Обнуление" одинаковых стран в общем массиве
                            UsersCountries[j] = null;
                        }
                    }

                    // Массив всех id для страны
                    TempIds = temp.Split(',');
                    // Количество уникальных id
                    NumUniqIds = 0;
                    // Подсчёт уникальных id для страны
                    for (int k = 0; k < TempIds.Length; k++)
                    {
                        // Вспомогательная переменная, если не найдено ни одного совпадения c k-м id, то DifferentIds == k
                        int DifferentIds = 0;
                        while (DifferentIds < k && TempIds[DifferentIds] != TempIds[k])
                        {
                            DifferentIds++;
                        }
                        NumUniqIds += DifferentIds == k ? 1 : 0;
                    }

                } else { continue; }

                // Добавление строки в итоговую таблицу (объект DataGridView)
                dgvResultTable.Rows.Add(UsersCountries[i], SumCount, NumUniqIds);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Очистка таблиц
            dgvSourceTable.Rows.Clear();
            dgvResultTable.Rows.Clear();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}

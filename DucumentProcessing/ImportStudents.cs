using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DucumentProcessing
{
    public static class ImportStudents
    {
        //Import students from a Excel file
        public static List<string> Import(string filePath)
        {
            var list = new List<string>();
            var workbook = new Workbook();

            workbook.LoadFromFile(filePath);

            Worksheet sheet = workbook.Worksheets[0]; // get the first worksheet

            for (int row = 1; row <= sheet.LastRow; row++)
            {
                list.Add(sheet.Range[row, 1].Text); // read the value in the first column of the current row
            }

            return list;
        }
    }
}

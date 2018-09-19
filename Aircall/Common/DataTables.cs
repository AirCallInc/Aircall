using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aircall.Common
{
    public class DataTables
    {
        private const int TOTAL_ROWS = 995;
        private static readonly List<DataItem> _data = CreateData();

        public class DataItem
        {
            public string Name { get; set; }
            public string Age { get; set; }
            public string DoB { get; set; }
        }

        public class DataTableData
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<DataItem> data { get; set; }
        }

        // here we simulate data from a database table.
        // !!!!DO NOT DO THIS IN REAL APPLICATION !!!!
        private static List<DataItem> CreateData()
        {
            Random rnd = new Random();
            List<DataItem> list = new List<DataItem>();
            for (int i = 1; i <= TOTAL_ROWS; i++)
            {
                DataItem item = new DataItem();
                item.Name = "Name_" + i.ToString().PadLeft(5, '0');
                DateTime dob = new DateTime(1900 + rnd.Next(1, 100), rnd.Next(1, 13), rnd.Next(1, 28));
                item.Age = ((DateTime.Now - dob).Days / 365).ToString();
                item.DoB = dob.ToShortDateString();
                list.Add(item);
            }
            return list;
        }

        private int SortString(string s1, string s2, string sortDirection)
        {
            return sortDirection == "asc" ? s1.CompareTo(s2) : s2.CompareTo(s1);
        }

        private int SortInteger(string s1, string s2, string sortDirection)
        {
            int i1 = int.Parse(s1);
            int i2 = int.Parse(s2);
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }

        private int SortDateTime(string s1, string s2, string sortDirection)
        {
            DateTime d1 = DateTime.Parse(s1);
            DateTime d2 = DateTime.Parse(s2);
            return sortDirection == "asc" ? d1.CompareTo(d2) : d2.CompareTo(d1);
        }

        // here we simulate SQL search, sorting and paging operations
        // !!!! DO NOT DO THIS IN REAL APPLICATION !!!!
        private List<DataItem> FilterData(ref int recordFiltered, int start, int length, string search, int sortColumn, string sortDirection)
        {
            List<DataItem> list = new List<DataItem>();
            if (search == null)
            {
                list = _data;
            }
            else
            {
                // simulate search
                foreach (DataItem dataItem in _data)
                {
                    if (dataItem.Name.ToUpper().Contains(search.ToUpper()) ||
                        dataItem.Age.ToString().Contains(search.ToUpper()) ||
                        dataItem.DoB.ToString().Contains(search.ToUpper()))
                    {
                        list.Add(dataItem);
                    }
                }
            }

            // simulate sort
            if (sortColumn == 0)
            {// sort Name
                list.Sort((x, y) => SortString(x.Name, y.Name, sortDirection));
            }
            else if (sortColumn == 1)
            {// sort Age
                list.Sort((x, y) => SortInteger(x.Age, y.Age, sortDirection));
            }
            else if (sortColumn == 2)
            {   // sort DoB
                list.Sort((x, y) => SortDateTime(x.DoB, y.DoB, sortDirection));
            }

            recordFiltered = list.Count;

            // get just one page of data
            list = list.GetRange(start, Math.Min(length, list.Count - start));

            return list;
        }
    }
}
using System.Collections.Generic;

namespace Libraries
{
    public class DataList<T> : List<T>
    {
        public DataList(List<T> source, int index, int size, int total)
        {
            Page = new Pagging
            {
                Index = index,
                Size = size,
                Total = total
            };

            AddRange(source);
        }

        public Pagging Page { get; }

        public string Type { get; set; }
        public int Id { get; set; }

        public int GetNo(T item)
        {
            var location = IndexOf(item) + 1;
            var startIndex = (Page.Index - 1) * Page.Size;
            location = location + startIndex;
            return location;
        }
        public List<SelectItem> Types { get; set; }
    }

    public class Pagging
    {
        public int Index { get; set; } 
        public int Size { get; set; }
        public int Total { get; set; }   
    }
}

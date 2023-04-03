using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    [Flags]
    public enum Genre
    {
        Fantasy = 1,
        Adventure = 2,
        Romance = 4,
        Contemporary = 8,
        Dystopian = 16,
        Mystery = 32,
        Horror = 64,
        Thriller = 128,
        Scientific = 256,
        Historicalfiction = 512,
        ScienceFiction = 1024,
        Childrens = 2048,
        Business = 4096,
        Medical = 8192
    }

    public enum ItemTypes { Book, Jurnal }
}

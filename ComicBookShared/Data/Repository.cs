using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComicBookShared.Models;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class Repository
    {

        private Context _context = null;

        // this will allow our web app controllers to handle it's lifetime
        public Repository(Context context)
        {
            _context = context;
        }

        public IList<ComicBook> GetComicBooks()
        {
            var comicBooks = _context.ComicBooks
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();// Forces Excution of the query, makes it clear that the db query will happen here in this method
        }

        public IList<ComicBook> GetComicBook()
        {
            var ComicBook = _context.ComicBooks
                   .Include(cb => cb.Series)
                   .Include(cb => cb.Artists.Select(a => a.Artist))
                   .Include(cb => cb.Artists.Select(a => a.Role))
                   .Where(cb => cb.Id == id)
                   .SingleOrDefault();
        }

    }
}

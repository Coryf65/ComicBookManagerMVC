using ComicBookShared.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class ComicBookArtistsRepository : BaseRepository<ComicBookArtist>
    {

        //private Context _context = null;

        public ComicBookArtistsRepository(Context context) : base(context)
        {
            //_context = context;
        }       

        public override ComicBookArtist Get(int id, bool includeRelatedEntities = true)
        {
            var comicBookArtists = Context.ComicBookArtists.AsQueryable();

            if (includeRelatedEntities)
            {
                comicBookArtists = comicBookArtists
                    .Include(cba => cba.Artist)
                    .Include(cba => cba.Role)
                    .Include(cba => cba.ComicBook.Series);
            }

            return comicBookArtists
                
                .Where(cba => cba.Id == id)
                .SingleOrDefault();
        }

        /// <summary>
        /// Not using yet so it will throw
        /// </summary>
        public override IList<ComicBookArtist> GetList()
        {
            throw new NotImplementedException();
        }
    }
}

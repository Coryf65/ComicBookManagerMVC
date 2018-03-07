using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    /// <summary>
    /// Entity Type Focused Repository
    /// for the Artist Entity Type
    /// </summary>
   public class ArtistsRepository : BaseRepository<Artist> //Inherited from the Base Class Repository and used an Artist Entity Type for the generic Parameter
   {
      public ArtistsRepository(Context context): base(context) //Constructor
      {

      }

       public override Artist Get(int id, bool includeRelatedEntities = true) //Get Method
       {
                var artist = Context.Artists.AsQueryable(); // get a reference to an IQueryable Object

                if (includeRelatedEntities)
                {
                    artist = artist
                        .Include(s => s.ComicBooks.Select(a => a.ComicBook.Series))
                        .Include(s => s.ComicBooks.Select(a => a.Role));
                }

                return artist
                    .Where(cb => cb.Id == id)
                    .SingleOrDefault();
       }

       public override IList<Artist> GetList()
       {
          return Context.Artists
                .OrderBy(a => a.Name)
                .ToList();
       }

       public bool ArtistHasName(int artistId, string name)
       {
          return Context.Artists
                .Any(a => a.Id != artistId && a.Name == name);
       }
   }
}

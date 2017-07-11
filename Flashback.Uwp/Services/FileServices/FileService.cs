using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashback.Model;
using Template10.Services.FileService;

namespace FlashbackUwp.Services.FileServices
{
    /// <summary>
    /// Skälet till denna är för att att spara state via onnavigatedFrom osv har begränsning till 8kb för state där.
    /// Spara ner vymodelleras data här. Används för bläddring bakåt i forumlistorna så man slipper göra riktiga anrop
    /// </summary>
    public class FileService
    {
        private const string CACHEFILE = "fbCache.json";
        private const string EXTRAFORUMSFILE = "extraForums.json";
        
        public async Task AddToCacheList(ForumList newForumlist)
        {
            FlashbackCacheList<Dictionary<string,ForumList>> fbCache;

            var dataToAdd = new Dictionary<string, ForumList> {
            {
                newForumlist.Id, newForumlist                
            }};

            var exists = await FileHelper.FileExistsAsync(CACHEFILE);

            if(!exists)
            {
                fbCache = new FlashbackCacheList<Dictionary<string, ForumList>>();
            }
            else
            {
                fbCache = await FileHelper.ReadFileAsync<FlashbackCacheList<Dictionary<string, ForumList>>>(CACHEFILE);
            }
            
            fbCache.Add(dataToAdd);
            
            await FileHelper.WriteFileAsync(CACHEFILE, fbCache);
        }

        public async Task ResetCache()
        {           
            await FileHelper.DeleteFileAsync(CACHEFILE);
        }

        public async Task<ForumList> TryGetFromCache(string key)
        {            
            var cacheExists = await FileHelper.FileExistsAsync(CACHEFILE);

            if (!cacheExists)
            {                
                return null;
            }                
            else
            {
                FlashbackCacheList<Dictionary<string, ForumList>> data;
                
                try
                {                   
                    data = await FileHelper.ReadFileAsync<FlashbackCacheList<Dictionary<string, ForumList>>>(CACHEFILE);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                var item = data?.FirstOrDefault(x => x.ContainsKey(key));

                return item?[key];
            }            
        }

        public async Task SaveExtraForums(List<FbItem> forumList)
        {
            if(forumList == null)
                forumList = new List<FbItem>();
                    
            await FileHelper.WriteFileAsync(EXTRAFORUMSFILE, forumList);
        }

        public async Task<List<FbItem>> GetExtraForums()
        {
            var fileExists = await FileHelper.FileExistsAsync(EXTRAFORUMSFILE);

            if (!fileExists)
            {
                return new List<FbItem>();
            }
            else
            {
                return await FileHelper.ReadFileAsync<List<FbItem>>(EXTRAFORUMSFILE);
            }
        }


    }
}

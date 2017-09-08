using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using Windows.Storage.Streams;
using Template10.Services.SerializationService;

namespace FlashbackUwp.Services.SecurityServices
{
    /// <summary>
    /// Används för att spara ner cookies och få dom krypterade till att bara kunna öppnas av LOCAL=user
    /// </summary>
    public class EncryptionService
    {
        private readonly ISerializationService _serializationService;
        private const string COOKIEFILE = "cookies.txt";
        public EncryptionService()
        {
            _serializationService = SerializationService.Json;
        }

        public async Task<string> Unprotect(IBuffer buffProtected)
        {            
            var provider = new DataProtectionProvider();
            
            var buffUnprotected = await provider.UnprotectAsync(buffProtected);

            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffUnprotected);
        }

        private async Task<IBuffer> Protect(string dataToBeStored)
        {            
            var provider = new DataProtectionProvider("LOCAL=user");

            var encoding = BinaryStringEncoding.Utf8;
            var buffMsg = CryptographicBuffer.ConvertStringToBinary(dataToBeStored, encoding);

            return await provider.ProtectAsync(buffMsg);
        }

        public async Task<bool> WriteCookieData(CookieContainer container)
        {
            try
            {
                var cookies = container.GetCookies(new Uri("https://www.flashback.org/")).Cast<Cookie>().ToList();
                var dataToBeSecured = _serializationService.Serialize(cookies);
                var buffer = await Protect(dataToBeSecured);

                var storageFolder = ApplicationData.Current.LocalFolder;
                var sampleFile = await storageFolder.CreateFileAsync(COOKIEFILE, CreationCollisionOption.ReplaceExisting);

                var stream = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);

                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    using (var dataWriter = new DataWriter(outputStream))
                    {
                        dataWriter.WriteBuffer(buffer);

                        await dataWriter.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }

                stream.Dispose();

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Fel vid skrivande av cookies: " + e.Message);
                return false;               
            }
            
        }

        public async Task<List<Cookie>> GetCookieData()
        {
            try
            {
                var storageFolder = ApplicationData.Current.LocalFolder;
                IStorageItem cookieFileCheck = await storageFolder.TryGetItemAsync(COOKIEFILE);

                if (cookieFileCheck == null)
                    return new List<Cookie>();

                var cookieFile = (StorageFile)cookieFileCheck;

                var stream = await cookieFile.OpenAsync(FileAccessMode.Read);
                ulong size = stream.Size;

                if(size == 0)
                    return new List<Cookie>();

                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    using (var dataReader = new DataReader(inputStream))
                    {
                        uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                        var buff = dataReader.ReadBuffer(numBytesLoaded);
                        var cookieData = await Unprotect(buff);

                        if(!string.IsNullOrWhiteSpace(cookieData))
                            return _serializationService.Deserialize<List<Cookie>>(cookieData);
                    }
                }
            }
            catch (Exception e)
            {
                // nåt sket sig, vi nollar isf alla cookies
                Debug.WriteLine("Det sket sig vid hämtning av sparade cookies: " + e.Message);
                return new List<Cookie>();               
            }
            
            return new List<Cookie>();
        }
    }
}

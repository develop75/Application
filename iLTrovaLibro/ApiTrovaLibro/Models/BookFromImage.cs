using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using TrovaLibroLib.Dto;

namespace ApiTrovaLibro.Models
{
    public class BookFromImage
    {
        #region Google Classes
        public class GoogleBookVolumeInfo
        {
            public string title { get; set; }
            public List<string> authors { get; set; }
            public string publisher { get; set; }
            public string publishedDate { get; set; }
            public string description { get; set; }
            public ImageLinks imageLinks { get; set; }
        }

        public class ImageLinks
        {
            public string thumbnail { get; set; }
        }

        public class GoogleBookItem
        {
            public GoogleBookVolumeInfo volumeInfo { get; set; }
        }

        public class GoogleBookResponse
        {
            public List<GoogleBookItem> items { get; set; }
        }
        #endregion Google Classes

        /// <summary>
        /// Processa un'immagine di copertina, estrae l'ISBN, recupera i dati del libro da Google Books e restituisce un BookDto completo.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BookDto> GetBookDtoFromImage(string imagePath)
        {
            // 1. Leggo ISBN da foto
            var isbn = ExtractIsbnFromImage(imagePath);
            if (isbn == null)
                throw new Exception("Impossibile leggere il codice ISBN dalla foto.");

            // 2. Ottengo i dati del libro da Google Books
            var info = await GetBookInfoFromGoogle(isbn);
            if (info == null)
                throw new Exception("Nessun libro trovato per questo ISBN.");

            // 3. Converto in BookDto della tua app
            return ConvertToBookDto(info, isbn);
        }
        /// <summary>
        /// Estrae il codice ISBN da un'immagine di copertina usando ZXing.Net
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public string? ExtractIsbnFromImage(string imagePath)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(imagePath);

            // ✔️ Convertiamo in byte[]
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            int bufferSize = bitmapData.Stride * bitmapData.Height;
            byte[] buffer = new byte[bufferSize];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, buffer, 0, bufferSize);
            bitmap.UnlockBits(bitmapData);

            // ✔️ Creiamo la luminance source
            var luminanceSource = new RGBLuminanceSource(buffer, bitmap.Width, bitmap.Height, RGBLuminanceSource.BitmapFormat.ARGB32);

            // ✔️ Binarizzazione
            var binarizer = new HybridBinarizer(luminanceSource);
            var binaryBitmap = new BinaryBitmap(binarizer);

            // ✔️ Reader
            var reader = new MultiFormatReader();

            var result = reader.decode(binaryBitmap);

            return result?.Text;
        }
        /// <summary>
        /// Recupera le informazioni del libro da Google Books API dato un codice ISBN.
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public async Task<GoogleBookVolumeInfo?> GetBookInfoFromGoogle(string isbn)
        {
            using var client = new HttpClient();

            var json = await client.GetStringAsync(
                $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}"
            );

            var result = JsonSerializer.Deserialize<GoogleBookResponse>(json);

            return result?.items?.FirstOrDefault()?.volumeInfo;
        }
        /// <summary>
        /// Converte le informazioni ottenute da Google Books in un BookDto utilizzabile dalla tua applicazione.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public BookDto ConvertToBookDto(GoogleBookVolumeInfo info, string isbn)
        {
            return new BookDto
            {
                Title = info.title,
                Author = info.authors?.FirstOrDefault() ?? "Sconosciuto",
                Publisher = info.publisher ?? "Sconosciuto",
                Description = info.description ?? "",
                Isbn = isbn,
                Cover = info.imageLinks?.thumbnail ?? "",

                // Campi gestiti dal sistema o dall'utente
                CategoryId = 0,
                CategorySubId = 0,
                TargetId = 0,
                IsNew = false,
                CoverPrice = null,
                Price = 0,
                PriceOld = 0,
                ShippingPrice = 0,
                IsShippingAvailable = false,
                IsSelling = false,
                IsActive = true,
                CreationDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
        }


    }
}

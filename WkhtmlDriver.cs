using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using IronPdf;

namespace Screenshots
{
    public class WkhtmlDriver
    {
        public static byte[] Capture(string switches, ImageFormat format)
        {
            // switches:
            //     "-q"  - silent output, only errors - no progress messages
            //     " -"  - switch output to stdout
            //     "- -" - switch input to stdin and output to stdout
            switches = "-q " + switches + " -";

            string location;
            string wkhtmlPath = WkhtmltopdfConfiguration.Path;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                location = Path.Combine(wkhtmlPath, "Windows", "wkhtmltopdf.exe");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                location = Path.Combine(wkhtmlPath, "Mac", "wkhtmltopdf");
            }
            else
            {
                location = Path.Combine(wkhtmlPath, "Linux", "wkhtmltopdf");
            }

            if (!File.Exists(location))
            {
                throw new Exception("wkhtmltopdf not found, searched for " + location);
            }

            var proc = new Process();
            try
            {
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = location,
                    Arguments = switches,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                };

                proc.Start();
            }
            catch (Exception)
            {
                throw;
            }

            using (var stream = new MemoryStream())
            {
                using (var sOut = proc.StandardOutput.BaseStream)
                {
                    byte[] buffer = new byte[4096];
                    int read;

                    while ((read = sOut.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, read);
                    }
                }

                string error = proc.StandardError.ReadToEnd();
                if (stream.Length == 0)
                {
                    throw new Exception(error);
                }

                proc.WaitForExit();

                return ToImage(stream.ToArray(), format);
            }
        }

        private static byte[] ToImage(byte[] pdf, ImageFormat format)
        {
            var document = new PdfDocument(pdf);           
            var bitmap = AppendVertical(document.ToBitmap());
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, format);
                return stream.ToArray();
            }
        }

        public static Bitmap AppendVertical(Bitmap[] bitmaps)
        {
            Bitmap finalBitmap = null;
            try
            {
                int width = 0;
                int height = 0;

                foreach (var bitmap in bitmaps)
                {
                    width = bitmap.Width > width ? bitmap.Width : width;
                    height += bitmap.Height;
                }

                finalBitmap = new Bitmap(width, height);

                using (var graphics = Graphics.FromImage(finalBitmap))
                {
                    // set background color
                    graphics.Clear(System.Drawing.Color.Black);

                    int offset = 0;
                    foreach (var image in bitmaps)
                    {
                        graphics.DrawImage(image, new Rectangle(0, offset, image.Width, image.Height));
                        offset += image.Height;
                    }
                }

                return finalBitmap;
            }
            catch (Exception)
            {
                if (finalBitmap != null)
                {
                    finalBitmap.Dispose();
                }
                throw;
            }
            finally
            {
                foreach (var bitmap in bitmaps)
                {
                    bitmap.Dispose();
                }
            }
        }
    }
}

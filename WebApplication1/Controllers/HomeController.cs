using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public class PixelInfo
        {
            public Color color;
            public int x = -1;
            public int y = -1;
            public string name;
        }

        public class ColorComparison
        {
            public Color systemColor;
            public double dR;
            public double dG;
            public double dB;
            public double distance;

        }

        private List<Color> GetAllColors()
        {
            List<Color> allColors = new List<Color>();

            foreach (PropertyInfo property in typeof(Color).GetProperties())
            {
                if (property.PropertyType == typeof(Color))
                {
                    allColors.Add((Color)property.GetValue(null));
                }
            }

            return allColors;
        }


        private Color GetFromARGB(List<Color> systemColors, Color target)
        {

            double Diff = 0;

            List<ColorComparison> comparisons = new List<ColorComparison>();
            ColorComparison bestMatch = null;
            
            for (int c = 0; c < systemColors.Count; c++)
            {
                var color = systemColors[c];

                ColorComparison cp = new ColorComparison();
                cp.systemColor = color;

                cp.dR = Math.Abs(target.R - color.R);
                cp.dG = Math.Abs(target.G - color.G);
                cp.dB = Math.Abs(target.B - color.B);
                cp.distance = cp.dR + cp.dB + cp.dG;

                comparisons.Add(cp);

                if (bestMatch == null ||cp.distance < bestMatch.distance)
                {
                    bestMatch = cp;
                }
            }

            return bestMatch.systemColor;

        }


        public void ScanImage()
        {
            var path = Server.MapPath("~/all_you_need_is_love_schulz.jpg");
            FileStream originalFile = System.IO.File.Open(path, FileMode.Open);
            Image fileImage = System.Drawing.Image.FromStream(originalFile);

            List<PixelInfo> colors = new List<PixelInfo>();

            List<Color> systemColors = GetAllColors();

            using (Bitmap bmp = new Bitmap(fileImage))
            {

                for (int y = 0; y < bmp.Height; y++)
                {
                    if (y % 2 == 0)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {

                            if (x % 2 == 0)
                            {
                                var pixel = bmp.GetPixel(x, y);
                                PixelInfo pixi = new PixelInfo();
                                pixi.x = x;
                                pixi.y = y;

                                pixi.name = GetFromARGB(systemColors, pixel).Name;

                                pixi.color = pixel;

                                colors.Add(pixi);
                            }

                        }
                    }
                }
            }
        }
    }
}
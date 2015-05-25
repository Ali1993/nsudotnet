using System;
using System.IO;
using System.Linq;

namespace Eshankulov.Nsudotnet.LinesCounter
{
    class LinesCounter
    {
        public static int CountLines(string dir, string[] typeOfFiles)
        {
            
            string[] paths = Directory.GetDirectories(dir);
            string[] files = Directory.GetFiles(dir);
            bool forComments = false;

            int count = paths.Sum(directory => CountLines(directory, typeOfFiles));

            foreach (string types in typeOfFiles)
            {
                foreach (string f in files.Where(f => f.EndsWith(types)))
                {
                    using (StreamReader reader = new StreamReader(f))
                    {
                        string str = null;
                        while ((str = reader.ReadLine()) != null)
                        {
                            str = str.Trim();

                 if (forComments)
                {
                    if (!str.Contains("*/"))  
                    {                                
                      continue;
                    }   
                         else
                        {
                          forComments = false;
                          continue;
                         }
                }
                
                 else
               {
                     if (str.StartsWith("//"))  
                     {
                       continue;
                     }
                   
                     if (str.Contains("/*"))
                     {
                        forComments = true;
                     }   
              }
                            count++;
                        }
                    }
                }
            }
            return count;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            string curdirect = Directory.GetCurrentDirectory();
            int res = LinesCounter.CountLines(curdirect, args);
            Console.WriteLine(res);
            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace ListDecrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            if (args[0].EndsWith(".csv"))
                WriteList(args[0]);
            else
                WriteCSV(args[0]);
        }

        static void WriteList(string csvPath)
        {
            ChaListData chaListData = new ChaListData();

            using (StreamReader reader = new StreamReader(csvPath, Encoding.UTF8))
            {
                chaListData.categoryNo = int.Parse(reader.ReadLine().Trim());
                chaListData.distributionNo = int.Parse(reader.ReadLine().Trim());
                chaListData.filePath = reader.ReadLine().Trim();

                chaListData.lstKey = reader.ReadLine().Trim().Split(',').ToList();

                int i = 0;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (!line.Contains(','))
                        break;

                    chaListData.dictList.Add(i++, line.Split(',').ToList());
                }

                string listPath = Path.ChangeExtension(csvPath, "txt");

                File.WriteAllBytes(listPath, MessagePackSerializer.Serialize(chaListData));
            }
        }

        static void WriteCSV(string listPath)
        {
            ChaListData chaListData = MessagePackSerializer.Deserialize<ChaListData>(File.ReadAllBytes(listPath));

            string csvPath = Path.ChangeExtension(listPath, "csv");

            using (StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8))
            {
                writer.WriteLine(chaListData.categoryNo);
                writer.WriteLine(chaListData.distributionNo);
                writer.WriteLine(chaListData.filePath);

                writer.WriteLine(chaListData.lstKey.Aggregate((a, b) => $"{a},{b}"));

                foreach (var entry in chaListData.dictList.OrderBy(x => x.Key).Select(x => x.Value))
                {
                    writer.WriteLine(entry.Aggregate((a, b) => $"{a},{b}"));
                }
            }
        }
    }
}

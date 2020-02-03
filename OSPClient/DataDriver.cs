using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSPClient
{
    public class DataDriver
    {
        // for standard data
        static string rootPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName; //+ "/data";
        // for test data
        //static string rootPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/data-test";

        static string photoFilename = Path.Combine(rootPath, @"Photo");
        static string tagFilename = Path.Combine(rootPath, @"Tag");
        static string gpsFilename = Path.Combine(rootPath, @"GPS");
        static int cPhoto = 191737;
        static int cGPS = 3485450;

        /***
         * int rate: generating rate of photo stream, rates of tag and gps streams are correspondingly decided.
         * int randSpan: time span for timestamp randomization.
         ***/
        public static async Task Run(IAsyncStream<string> photoStream, IAsyncStream<string> tagStream, IAsyncStream<string> gpsStream, long rate, int randSpan)
        {
            StreamReader photoFile = new StreamReader(photoFilename);
            StreamReader tagFile = new StreamReader(tagFilename);
            StreamReader gpsFile = new StreamReader(gpsFilename);
            long ratePhoto = rate / 100, rateGPS = rate * cGPS / cPhoto / 100;
            PhotoStreamProducer psp = new PhotoStreamProducer(photoFile, tagFile, photoStream, tagStream, (int)ratePhoto, randSpan);
            GPSStreamProducer gsp = new GPSStreamProducer(gpsFile, gpsStream, (int)rateGPS, randSpan);
            var task1 = psp.Start();
            var task2 = gsp.Start();
            await Task.WhenAll(task1, task2);
        }
        public static long getCurrentTimestamp()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalMilliseconds;
        }
    }
    public class PhotoStreamProducer
    {
        StreamReader photoFile, tagFile;
        IAsyncStream<string> photoStream, tagStream;
        int rate, randSpan;
        bool endOfFile;
        IDictionary<int, ISet<int>> tags;
        public PhotoStreamProducer(StreamReader photoFile, StreamReader tagFile, IAsyncStream<string> photoStream, IAsyncStream<string> tagStream, int rate, int randSpan)
        {
            this.photoFile = photoFile;
            this.tagFile = tagFile;
            this.photoStream = photoStream;
            this.tagStream = tagStream;
            this.rate = rate;
            this.randSpan = randSpan;
            this.endOfFile = false;
            string line;
            tags = new Dictionary<int, ISet<int>>();
            while ((line = tagFile.ReadLine()) != null)
            {
                string[] arr = line.Split(" ");
                int pid, uid;
                if (!Int32.TryParse(arr[0], out pid) || !Int32.TryParse(arr[1], out uid)) continue;
                if (!tags.ContainsKey(pid))
                {
                    tags.Add(pid, new HashSet<int>());
                }
                tags[pid].Add(uid);
            }
        }

        public async Task Start()
        {
            while (!endOfFile)
            {
                await Run();
                Thread.Sleep(10);
            }
        }

        public async Task Run()
        {
            string line;
            Random random = new Random();
            int count = rate / 2 + random.Next(rate + 1);
            for (int i = 0; i < count; ++i)
            {
                line = photoFile.ReadLine();
                if (line == null) break;
                long ts = DataDriver.getCurrentTimestamp() + random.Next(2 * randSpan + 1) - randSpan;
                line = line + " " + ts;
                await photoStream.OnNextAsync(line);
                int pid;
                if (!Int32.TryParse(line.Split(" ")[0], out pid)) continue;
                if (tags.ContainsKey(pid))
                {
                    foreach (int uid in tags[pid])
                    {
                        string tagLine = pid + " " + uid + " " + ts;
                        await tagStream.OnNextAsync(tagLine);
                    }
                }
            }
        }

    }
    public class GPSStreamProducer
    {
        StreamReader gpsFile;
        IAsyncStream<string> gpsStream;
        int rate, randSpan;
        bool endOfFile;
        public GPSStreamProducer(StreamReader gpsFile, IAsyncStream<string> gpsStream, int rate, int randSpan)
        {
            this.gpsFile = gpsFile;
            this.gpsStream = gpsStream;
            this.rate = rate;
            this.randSpan = randSpan;
            this.endOfFile = false;
        }

        public async Task Start()
        {
            while (!endOfFile)
            {
                await Run();
                Thread.Sleep(10);
            }
        }

        public async Task Run()
        {
            string line;
            Random random = new Random();
            int count = rate / 2 + random.Next(rate + 1);
            for (int i = 0; i < count; ++i)
            {
                line = gpsFile.ReadLine();
                if (line == null) break;
                long ts = DataDriver.getCurrentTimestamp() + random.Next(2 * randSpan + 1) - randSpan;
                line = line + " " + ts;
                await gpsStream.OnNextAsync(line);
            }
        }
    }
}

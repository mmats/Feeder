using RssImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssImporterTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //RssFeed feed;
            
            //feed = new RssFeed("https://www.heise.de/rss/heise-atom.xml", 7);
            //feed.Refresh();
            //feed.MarkAllItemsAsRead();
            //Print(feed.Items);

            //feed = new RssFeed("https://rss.golem.de/rss.php?feed=ATOM1.0", 7);
            //feed.Refresh();
            //Print(feed.Items);

            Feeds feeds = new Feeds();
            feeds.Add("https://www.heise.de/rss/heise-atom.xml");
            feeds.Add("https://rss.golem.de/rss.php?feed=ATOM1.0");
            
            foreach(RssFeed f in feeds.FeedList)
            {
                f.Refresh();
                Print(f.Items);
            }

            Console.ReadLine();
        }

        static void Print(List<FeedItem> list)
        {
            foreach (RssImporter.FeedItem item in list)
            {
                if (item.IsRead)
                    Console.Write("[R]");
                else
                    Console.Write("[ ]");

                Console.WriteLine($" - {item.PublishDate} - {item.Url} - {item.Title}");
            }
        }
    }
}

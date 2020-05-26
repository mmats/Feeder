using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.ServiceModel.Syndication;
using System.Xml;

namespace RssImporter
{
    public class FeedItem
    {
        public string Title { get; internal set; }
        public DateTimeOffset PublishDate { get; internal set; }
        public string PreviewText { get; internal set; }
        public string Url { get; internal set; }
        public bool IsRead { get; internal set; } = false;
    }

    public class RssFeed
    {
        #region Fields and properties
        public List<FeedItem> Items { get; internal set; }

        private string feedUrl = String.Empty;
        public string FeedUrl
        {
            get => feedUrl;
            internal set
            {
                if (value != feedUrl)
                {
                    feedUrl = value;
                    Items?.Clear();
                }
            }
        }
        public int RetentionPeriodInDays { get; internal set; }
        #endregion

        #region Constructor
        public RssFeed(string url)
            : this(url, 2)
        {

        }

        public RssFeed(string url, int retentionPeriod)
        {
            FeedUrl = url;
            RetentionPeriodInDays = retentionPeriod;

            Items = new List<FeedItem>();
        }
        #endregion

        #region Methods
        public void Refresh()
        {
            Atom10FeedFormatter formatter = new Atom10FeedFormatter();
            using (XmlReader reader = XmlReader.Create(FeedUrl))
            {
                formatter.ReadFrom(reader);
            }

            foreach (SyndicationItem item in formatter.Feed.Items)
            {
                FeedItem newItem = new FeedItem()
                {
                    Title = item.Title.Text,
                    PublishDate = item.PublishDate,
                    PreviewText = ((TextSyndicationContent)item.Content)?.Text != null ? ((TextSyndicationContent)item.Content).Text : String.Empty,
                    Url = item.Id
                };

                if (Items.Count == 0)
                    Items.Add(newItem);
                else if (FindItem(item.Id) == null)
                    Items.Insert(0, newItem);
            }

            Clean();
        }

        public void Clean()
        {
            Items.RemoveAll(item => item.PublishDate + new TimeSpan(RetentionPeriodInDays, 0, 0, 0) < DateTimeOffset.Now);
        }

        public FeedItem FindItem(string url)
        {
            return Items.Find(x => x.Url.Equals(url));
        }

        public void MarkAllItemsAsRead()
        {
            foreach (FeedItem item in Items)
                item.IsRead = true;
        }

        public void MarkAllItemsAsUnread()
        {
            foreach (FeedItem item in Items)
                item.IsRead = false;
        }
        #endregion
    }

    public class Feeds
    {
        #region Fields and properties
        public List<RssFeed> FeedList { get; internal set; }
        #endregion

        #region Constructor
        public Feeds()
        {
            FeedList = new List<RssFeed>();
        }

        public void Add(string url)
        {
            FeedList.Add(new RssFeed(url));
        }

        public void Remove(string url)
        {
            FeedList.Remove(Find(url));
        }

        public RssFeed Find(string url)
        {
            return FeedList.Find(x => x.FeedUrl.Equals(url));
        }

        #endregion
    }
}

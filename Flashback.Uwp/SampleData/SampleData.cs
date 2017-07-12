using System.Collections.ObjectModel;
using Flashback.Model;

namespace FlashbackUwp.SampleData
{
    /// <summary>
    /// används enbart för att mata xaml-designern med data och för att minska junket av detta i vymodellerna
    /// </summary>
    public static class SampleData
    {
        public static ForumList GetDefaultForumList()
        {
            var forumList = new ForumList
            {
                Id = "/f202",
                Title = "Hockey",
                ShowNavigation = true,
                CurrentPage = 1,
                MaxPages = 378,
                Items = new ObservableCollection<FbItem>
                    {
                        new FbItem
                        {
                            Name = "Bästa back 2017?",
                            ShowPostCount = true,
                            PostCount = 1234,
                            Type = FbItemType.Thread,
                            ShowForumColor = false,
                            Description = "Senaste inlägg av XXXX 2017-05-01"
                        },
                        new FbItem
                        {
                            Name = "Bästa back 2017?",
                            ShowPostCount = true,
                            PostCount = 367,
                            Type = FbItemType.Thread,
                            ShowForumColor = false,
                             Description = "Senaste inlägg av XXXX 2017-05-01"
                        },
                        new FbItem()
                        {
                            Name = "Bästa back 2017?",
                            ShowPostCount = true,
                            PostCount = 12,
                            Type = FbItemType.Thread,
                            ShowForumColor = false,
                            Description = "Senaste inlägg av XXXX 2017-05-01"
                        },
                        new FbItem()
                        {
                            Name = "Bästa back 2017?",
                            ShowPostCount = true,
                            PostCount = 4,
                            Type = FbItemType.Thread,
                            ShowForumColor = false,
                            Description = "Senaste inlägg av XXXX 2017-05-01"
                        },
                        new FbItem()
                        {
                            Name = "Ekonomi",
                            ShowForumColor = true,
                            ShowPostCount = false,
                            Type = FbItemType.Forum
                        }
                    }
            };

            return forumList;
        }


        public static ObservableCollection<FbItem> GetDefaultAktuellaÄmnen()
        {
            var list = new ObservableCollection<FbItem>()
            {
                new FbItem()
                {
                    Description = "Sport - hockey",
                    Name = "Den stora tråden om SHL 2017",
                    Id = "/t1231221",
                    ShowPostCount = true,
                    ShowForumColor = false,
                    PostCount = 2682
                }
            };

            return list;
        }

        public static ForumThread GetDefaultForumThread()
        {
            var thread = new ForumThread
            {
                Id = "t123123",
                CurrentPage = 5,
                MaxPages = 78,
                ShowNavigation = true,
                ParentId = "/f3432",
                Title = "Den stora tråden om allsvenskan 2017"
            };

            return thread;
        }

        public static ObservableCollection<FbItem> GetDefaultNewTopics()
        {
            return new ObservableCollection<FbItem>();
        }

        public static ObservableCollection<FbItem> GetDefaultNewPosts()
        {
            return new ObservableCollection<FbItem>();
        }

        public static ObservableCollection<FbItem> GetDefaultMyStartedPosts()
        {
            var list = new ObservableCollection<FbItem>()
            {
                new FbItem()
                {
                    Description = "Sport - hockey",
                    Name = "Den stora tråden om SHL 2017",
                    Id = "/t1231221",
                    ShowPostCount = true,
                    ShowForumColor = false,
                    PostCount = 2682
                }
            };

            return list;
        }

        public static ObservableCollection<FbItem> GetDefaultExtraForums()
        {
            var list = new ObservableCollection<FbItem>()
            {
                new FbItem()
                {
                    Description = "AAAA",
                    Name = "AAAA",
                    Id = "/f1",
                    Type = FbItemType.Forum                    
                },
                new FbItem()
                {
                    Description = "BBBB",
                    Name = "BBBBB",
                    Id = "/f2",
                    Type = FbItemType.Forum
                }
            };

            return list;
        }

        public static ObservableCollection<FbItem> GetDefaultSearchResult()
        {
            return new ObservableCollection<FbItem>
            {
                new FbItem
                {
                    Name = "varör är beatles bäst?",
                    ShowPostCount = true,
                    PostCount = 1234,
                    Type = FbItemType.Thread,
                    ShowForumColor = false,
                    Description = "Senaste inlägg av XXXX 2017-05-01"
                },
                new FbItem
                {
                    Name = "paul eller lennon?",
                    ShowPostCount = true,
                    PostCount = 367,
                    Type = FbItemType.Thread,
                    ShowForumColor = false,
                    Description = "Senaste inlägg av XXXX 2017-05-01"
                },
                new FbItem()
                {
                    Name = "stora tråden om beatles",
                    ShowPostCount = true,
                    PostCount = 12819,
                    Type = FbItemType.Thread,
                    ShowForumColor = false,
                    Description = "Senaste inlägg av XXXX 2017-05-01"
                },
                new FbItem()
                {
                    Name = "Rickenbacker?",
                    ShowPostCount = true,
                    PostCount = 4,
                    Type = FbItemType.Thread,
                    ShowForumColor = false,
                    Description = "Senaste inlägg av XXXX 2017-05-01"
                }

            };
        }
    }
}

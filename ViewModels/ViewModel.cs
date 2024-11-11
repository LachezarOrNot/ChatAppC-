using ChatApp.Commands;
using ChatApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatApp.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
       
        private readonly ResourceDictionary dictionary = Application.LoadComponent(new Uri("/ChatApp;component/Assets/icons.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;

        #region MainWindow

        #region Properties
        public string ContactName { get; set; }
        public byte[] ContactPhoto { get; set; }
        public string LastSeen { get; set; }

        #region Search Chats
        protected bool _isSearchBoxOpen;
        public bool IsSearchBoxOpen
        {
            get => _isSearchBoxOpen;
            set {
                if (_isSearchBoxOpen == value)
                    return;

                _isSearchBoxOpen = value;


                if (_isSearchBoxOpen == false)
                    //Clear Search Box
                    SearchText = string.Empty;
                OnPropertyChanged("IsSearchBoxOpen");
                OnPropertyChanged("SearchText");
            }
        }
        protected string LastSearchText { get; set; }
        protected string mSearchText { get; set; }
        public string SearchText
        {
            get => mSearchText;
            set
            {

               
                if (mSearchText == value)
                    return;

              
                mSearchText = value;

              
                if (string.IsNullOrEmpty(SearchText))
                    Search();
            }
        }

       
        private ObservableCollection<MoreOptionsMenu> _windowMoreOptionsMenuList;
        public ObservableCollection<MoreOptionsMenu> WindowMoreOptionsMenuList
        {
            get
            {
                return _windowMoreOptionsMenuList;
            }
            set
            {
                _windowMoreOptionsMenuList = value;
            }
        }

      
        private ObservableCollection<MoreOptionsMenu> _attachmentOptionsMenuList;
        public ObservableCollection<MoreOptionsMenu> AttachmentOptionsMenuList
        {
            get
            {
                return _attachmentOptionsMenuList;
            }
            set
            {
                _attachmentOptionsMenuList = value;
            }
        }
        #endregion
        #endregion

        #region Logics

        #region Window: More options Popup
        void WindowMoreOptionsMenu()
        {
            WindowMoreOptionsMenuList = new ObservableCollection<MoreOptionsMenu>()
            {
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["newgroup"],
                 MenuText="New Group"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["newbroadcast"],
                 MenuText="New Broadcast"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["starredmessages"],
                 MenuText="Starred Messages"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["settings"],
                 MenuText="Settings"
                },
            };
            OnPropertyChanged("WindowMoreOptionsMenuList");
        }
        void ConversationScreenMoreOptionsMenu()
        {
           
            WindowMoreOptionsMenuList = new ObservableCollection<MoreOptionsMenu>()
            {
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["allmedia"],
                 MenuText="All Media"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["wallpaper"],
                 MenuText="Change Wallpaper"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["report"],
                 MenuText="Report"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["block"],
                 MenuText="Block"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["clearchat"],
                 MenuText="Clear Chat"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["exportchat"],
                 MenuText="Export Chat"
                },
            };
            OnPropertyChanged("WindowMoreOptionsMenuList");
        }
        void AttachmentOptionsMenu()
        {
           
            AttachmentOptionsMenuList = new ObservableCollection<MoreOptionsMenu>()
            {
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["docs"],
                 MenuText="Docs",
                 BorderStroke="#3F3990",
                 Fill="#CFCEEC"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["camera"],
                    MenuText="Camera",
                    BorderStroke="#2C5A71",
                    Fill="#C5E7F8"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["gallery"],
                    MenuText="Gallery",
                    BorderStroke="#EA2140",
                    Fill="#F3BEBE"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["audio"],
                    MenuText="Audio",
                    BorderStroke="#E67E00",
                    Fill="#F7D5AC"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["location"],
                    MenuText="Location",
                    BorderStroke="#28C58F",
                    Fill="#E3F5EF"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["contact"],
                    MenuText="Contact",
                    BorderStroke="#0093E0",
                    Fill="#DDF1FB"
                }
            };
            OnPropertyChanged("AttachmentOptionsMenuList");
        }
        #endregion


        public void OpenSearchBox()
        {
            IsSearchBoxOpen = true;
        }
        public void ClearSearchBox()
        {
            if (!string.IsNullOrEmpty(SearchText))
                SearchText = string.Empty;
            else
                CloseSearchBox();
        }
        public void CloseSearchBox() => IsSearchBoxOpen = false;

        public void Search()
        {
           
            if ((string.IsNullOrEmpty(LastSearchText) && string.IsNullOrEmpty(SearchText)) || string.Equals(LastSearchText, SearchText))
                return;

            
            if (string.IsNullOrEmpty(SearchText) || Chats == null || Chats.Count <= 0)
            {
                FilteredChats = new ObservableCollection<ChatListData>(Chats ?? Enumerable.Empty<ChatListData>());
                OnPropertyChanged("FilteredChats");

                FilteredPinnedChats = new ObservableCollection<ChatListData>(PinnedChats ?? Enumerable.Empty<ChatListData>());
                OnPropertyChanged("FilteredPinnedChats");
               
                LastSearchText = SearchText;

                return;
            }

          


            FilteredChats = new ObservableCollection<ChatListData>(
                Chats.Where(
                    chat => chat.ContactName.ToLower().Contains(SearchText) //if ContactName Contains SearchText then add it in filtered chat list
                    ||
                    chat.Message != null && chat.Message.ToLower().Contains(SearchText) //if Message Contains SearchText then add it in filtered chat list
                    ));
            OnPropertyChanged("FilteredChats");

          
            FilteredPinnedChats = new ObservableCollection<ChatListData>(
            PinnedChats.Where(
                pinnedchat => pinnedchat.ContactName.ToLower().Contains(SearchText) //if ContactName Contains SearchText then add it in filtered chat list
                ||
                pinnedchat.Message != null && pinnedchat.Message.ToLower().Contains(SearchText) //if Message Contains SearchText then add it in filtered chat list
                ));

            OnPropertyChanged("FilteredPinnedChats");

           
            LastSearchText = SearchText;
        }
        #endregion

        #region Commands

        protected ICommand _windowsMoreOptionsCommand;
        public ICommand WindowsMoreOptionsCommand
        {
            get
            {
                if (_windowsMoreOptionsCommand == null)
                    _windowsMoreOptionsCommand = new CommandViewModel(WindowMoreOptionsMenu);
                return _windowsMoreOptionsCommand;
            }
            set
            {
                _windowsMoreOptionsCommand = value;
            }
        }

        protected ICommand _conversationScreenMoreOptionsCommand;
        public ICommand ConversationScreenMoreOptionsCommand
        {
            get
            {
                if (_conversationScreenMoreOptionsCommand == null)
                    _conversationScreenMoreOptionsCommand = new CommandViewModel(ConversationScreenMoreOptionsMenu);
                return _conversationScreenMoreOptionsCommand;
            }
            set
            {
                _conversationScreenMoreOptionsCommand = value;
            }
        }
        protected ICommand _attachmentOptionsCommand;
        public ICommand AttachmentOptionsCommand
        {
            get
            {
                if (_attachmentOptionsCommand == null)
                    _attachmentOptionsCommand = new CommandViewModel(AttachmentOptionsMenu);
                return _attachmentOptionsCommand;
            }
            set
            {
                _attachmentOptionsCommand = value;
            }
        }
       
        protected ICommand _openSearchCommand;
        public ICommand OpenSearchCommand
        {
            get
            {
                if (_openSearchCommand == null)
                    _openSearchCommand = new CommandViewModel(OpenSearchBox);
                return _openSearchCommand;
            }
            set
            {
                _openSearchCommand = value;
            }
        }

       
        protected ICommand _clearSearchCommand;
        public ICommand ClearSearchCommand
        {
            get
            {
                if (_clearSearchCommand == null)
                    _clearSearchCommand = new CommandViewModel(ClearSearchBox);
                return _clearSearchCommand;
            }
            set
            {
                _clearSearchCommand = value;
            }
        }

       
        protected ICommand _closeSearchCommand;
        public ICommand CloseSearchCommand
        {
            get
            {
                if (_closeSearchCommand == null)
                    _closeSearchCommand = new CommandViewModel(CloseSearchBox);
                return _closeSearchCommand;
            }
            set
            {
                _closeSearchCommand = value;
            }
        }

      
        protected ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new CommandViewModel(Search);
                return _searchCommand;
            }
            set
            {
                _searchCommand = value;
            }
        }
        #endregion
        #endregion

        #region Status Thumbs

        #region Properties
        public ObservableCollection<StatusDataModel> statusThumbsCollection { get; set; }
        #endregion

        #region Logics
        void LoadStatusThumbs()
        {
           
            statusThumbsCollection = new ObservableCollection<StatusDataModel>()
            {
               
            new StatusDataModel
            {
                IsMeAddStatus=true
            },
            new StatusDataModel
            {
              ContactName="Mike",
               ContactPhoto=new Uri("/assets/1.png", UriKind.RelativeOrAbsolute),
                 StatusImage=new Uri("/assets/5.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus=false
            },
            new StatusDataModel
            {
              ContactName="Steve",
               ContactPhoto=new Uri("/assets/2.jpg", UriKind.RelativeOrAbsolute),
                 StatusImage=new Uri("/assets/8.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus=false
            },
            new StatusDataModel
            {
              ContactName="Will",
               ContactPhoto=new Uri("/assets/3.png", UriKind.RelativeOrAbsolute),
                 StatusImage=new Uri("/assets/5.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus=false
            },

            new StatusDataModel
            {
              ContactName="John",
               ContactPhoto=new Uri("/assets/4.png", UriKind.RelativeOrAbsolute),
                 StatusImage=new Uri("/assets/3.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus=false
            },
            };
            OnPropertyChanged("statusThumbsCollection");
        }
        #endregion

        #endregion

        #region Chats List
        #region Properties
        public ObservableCollection<ChatListData> mChats;
        public ObservableCollection<ChatListData> mPinnedChats;
        public ObservableCollection<ChatListData> Chats
        {
            get => mChats;
            set
            {
                
                if (mChats == value)
                    return;

                
                mChats = value;

                
                FilteredChats = new ObservableCollection<ChatListData>(mChats);
                OnPropertyChanged("Chats");
                OnPropertyChanged("FilteredChats");
            }
        }
        public ObservableCollection<ChatListData> PinnedChats
        {
            get => mPinnedChats;
            set
            {
              
                if (mPinnedChats == value)
                    return;

              
                mPinnedChats = value;

               
                FilteredPinnedChats = new ObservableCollection<ChatListData>(mPinnedChats);
                OnPropertyChanged("PinnedChats");
                OnPropertyChanged("FilteredPinnedChats");
            }
        }

        protected ObservableCollection<ChatListData> _archivedChats;
        public ObservableCollection<ChatListData> ArchivedChats
        {
            get => _archivedChats; set
            {
                _archivedChats = value;
                OnPropertyChanged();
            }
        }

      
        public ObservableCollection<ChatListData> FilteredChats { get; set; }
        public ObservableCollection<ChatListData> FilteredPinnedChats { get; set; }

        protected int ChatPosition { get; set; }
        #endregion

        #region Logics
        void LoadChats()
        {
           
            if (Chats == null)
                Chats = new ObservableCollection<ChatListData>();

           
            connection.Open();

           
            ObservableCollection<ChatListData> temp = new ObservableCollection<ChatListData>();

            using (SqlCommand command = new SqlCommand("select * from contacts p left join (select a.*, row_number() over(partition by a.contactname order by a.id desc) as seqnum from conversations a ) a on a.ContactName = p.contactname and a.seqnum = 1 order by a.Id desc", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                   
                    string lastItem = string.Empty;
                    string newItem = string.Empty;

                    while (reader.Read())
                    {
                        string time = string.Empty;
                        string lastmessage = string.Empty;

                       
                        if (!string.IsNullOrEmpty(reader["MsgReceivedOn"].ToString()))
                        {
                            time = Convert.ToDateTime(reader["MsgReceivedOn"].ToString()).ToString("ddd hh:mm tt");
                            lastmessage = reader["ReceivedMsgs"].ToString();
                        }

                       
                        if (!string.IsNullOrEmpty(reader["MsgSentOn"].ToString()))
                        {
                            time = Convert.ToDateTime(reader["MsgSentOn"].ToString()).ToString("ddd hh:mm tt");
                            lastmessage = reader["SentMsgs"].ToString();
                        }

                        
                        if (string.IsNullOrEmpty(lastmessage))
                            lastmessage = "Start new conversation";

                       
                        ChatListData chat = new ChatListData()
                        {
                            ContactPhoto = (byte[])reader["photo"],
                            ContactName = reader["contactname"].ToString(),
                            Message = lastmessage,
                            LastMessageTime = time
                        };

                       
                        newItem = reader["contactname"].ToString();

                       
                        if (lastItem != newItem)
                            temp.Add(chat);

                        lastItem = newItem;
                    }
                }
            }
          
            Chats = temp;

          
            OnPropertyChanged("Chats");
              
                OnPropertyChanged();
        }
        #endregion

        #region Commands
        
        protected ICommand _getSelectedChatCommand;
        public ICommand GetSelectedChatCommand => _getSelectedChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                
                ContactName = v.ContactName;
                OnPropertyChanged("ContactName");

             
                ContactPhoto = v.ContactPhoto;
                OnPropertyChanged("ContactPhoto");

                LoadChatConversation(v);
            }
        });

      
        protected ICommand _pinChatCommand;
        public ICommand PinChatCommand => _pinChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!FilteredPinnedChats.Contains(v))
                {
                  
                    PinnedChats.Add(v);
                    FilteredPinnedChats.Add(v);
                    OnPropertyChanged("PinnedChats");
                    OnPropertyChanged("FilteredPinnedChats");
                    v.ChatIsPinned = true;


                   
                    ChatPosition = Chats.IndexOf(v);
                    Chats.Remove(v);
                    FilteredChats.Remove(v);
                    OnPropertyChanged("Chats");
                    OnPropertyChanged("FilteredChats");


                   
                    if (ArchivedChats != null)
                    {
                        if (ArchivedChats.Contains(v))
                        {
                            ArchivedChats.Remove(v);
                            v.ChatIsArchived = false;
                        }
                    }
                }
            }
        });

       
        protected ICommand _unPinChatCommand;
        public ICommand UnPinChatCommand => _unPinChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!FilteredChats.Contains(v))
                {
                   
                    Chats.Add(v);
                    FilteredChats.Add(v);

                  
                    Chats.Move(Chats.Count-1, ChatPosition);
                    FilteredChats.Move(Chats.Count-1, ChatPosition);

                  
                    OnPropertyChanged("Chats");
                    OnPropertyChanged("FilteredChats");

                   
                    PinnedChats.Remove(v);
                    FilteredPinnedChats.Remove(v);
                    OnPropertyChanged("PinnedChats");
                    OnPropertyChanged("FilteredPinnedChats");
                    v.ChatIsPinned = false;
                }
            }
        });

        
        protected ICommand _archiveChatCommand;
        public ICommand ArchiveChatCommand => _archiveChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!ArchivedChats.Contains(v))
                {
                           

                    
                    ArchivedChats.Add(v);
                    v.ChatIsArchived = true;
                    v.ChatIsPinned = false;

                   
                    Chats.Remove(v);
                    FilteredChats.Remove(v);
                    PinnedChats.Remove(v);
                    FilteredPinnedChats.Remove(v);

                  
                    OnPropertyChanged("Chats");
                    OnPropertyChanged("FilteredChats");
                    OnPropertyChanged("PinnedChats");
                    OnPropertyChanged("FilteredPinnedChats");
                    OnPropertyChanged("ArchivedChats");
                }
            }
        });
        
        protected ICommand _UnArchiveChatCommand;
        public ICommand UnArchiveChatCommand => _UnArchiveChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!FilteredChats.Contains(v) && !Chats.Contains(v))
                {
                    Chats.Add(v);
                    FilteredChats.Add(v);
                }
                ArchivedChats.Remove(v);
                v.ChatIsArchived = false;
                v.ChatIsPinned = false;


                OnPropertyChanged("Chats");
                OnPropertyChanged("FilteredChats");
                OnPropertyChanged("ArchivedChats");
            }
        });

        #endregion

        #endregion

        #region Conversations

        #region Properties
        protected bool _isConversationSearchBoxOpen;
        public bool IsConversationSearchBoxOpen
        {
            get => _isConversationSearchBoxOpen;
            set
            {
                if (_isConversationSearchBoxOpen == value)
                    return;

                _isConversationSearchBoxOpen = value;


                if (_isConversationSearchBoxOpen == false)
                   
                    SearchConversationText = string.Empty;
                OnPropertyChanged("IsConversationSearchBoxOpen");
                OnPropertyChanged("SearchConversationText");
            }
        }

        protected ObservableCollection<ChatConversation> mConversations;
        public ObservableCollection<ChatConversation> Conversations
        {
            get => mConversations;
            set
            {
               
                if (mConversations == value)
                    return;

               
                mConversations = value;

               
                FilteredConversations = new ObservableCollection<ChatConversation>(mConversations);
                OnPropertyChanged("Conversations");
                OnPropertyChanged("FilteredConversations");
            }
        }

       
        public ObservableCollection<ChatConversation> FilteredConversations { get; set; }

       
        protected string messageText;
        public string MessageText
        {
            get => messageText;
            set
            {
                messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        protected string LastSearchConversationText;
        protected string mSearchConversationText;
        public string SearchConversationText
        {
            get => mSearchConversationText;
            set
            {

                
                if (mSearchConversationText == value)
                    return;

              
                mSearchConversationText = value;

              
                if (string.IsNullOrEmpty(SearchConversationText))
                    SearchInConversation();
            }
        }

        public bool FocusMessageBox { get; set; }
        public bool IsThisAReplyMessage { get; set; }
        public string MessageToReplyText { get; set; }
        #endregion

        #region Logics
        protected bool _isSearchConversationBoxOpen;
        public bool IsSearchConversationBoxOpen
        {
            get => _isSearchConversationBoxOpen;
            set
            {
                if (_isSearchConversationBoxOpen == value)
                    return;

                _isSearchConversationBoxOpen = value;


                if (_isSearchConversationBoxOpen == false)
                  
                    SearchConversationText = string.Empty;
                OnPropertyChanged("IsSearchConversationBoxOpen");
                OnPropertyChanged("SearchConversationText");
            }
        }
        public void OpenConversationSearchBox()
        {
            IsSearchConversationBoxOpen = true;
        }
        public void ClearConversationSearchBox()
        {
            if (!string.IsNullOrEmpty(SearchConversationText))
                SearchConversationText = string.Empty;
            else
                CloseConversationSearchBox();
        }
        public void CloseConversationSearchBox() => IsSearchConversationBoxOpen = false;

        void LoadChatConversation(ChatListData chat)
        {
         
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            if (Conversations == null)
                Conversations = new ObservableCollection<ChatConversation>();
            Conversations.Clear();
            FilteredConversations.Clear();
            using (SqlCommand com = new SqlCommand("select * from conversations where ContactName=@ContactName", connection))
            {
                com.Parameters.AddWithValue("@ContactName", chat.ContactName);
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       

                        string MsgReceivedOn = !string.IsNullOrEmpty(reader["MsgReceivedOn"].ToString()) ?
                            Convert.ToDateTime(reader["MsgReceivedOn"].ToString()).ToString("MMM dd, hh:mm tt") : "";

                        string MsgSentOn = !string.IsNullOrEmpty(reader["MsgSentOn"].ToString()) ?
    Convert.ToDateTime(reader["MsgSentOn"].ToString()).ToString("MMM dd, hh:mm tt") : "";

                        var conversation = new ChatConversation()
                        {
                            ContactName = reader["ContactName"].ToString(),
                            ReceivedMessage = reader["ReceivedMsgs"].ToString(),
                            MsgReceivedOn = MsgReceivedOn,
                            SentMessage = reader["SentMsgs"].ToString(),
                            MsgSentOn = MsgSentOn,
                            IsMessageReceived = string.IsNullOrEmpty(reader["ReceivedMsgs"].ToString()) ? false : true
                        };
                        Conversations.Add(conversation);
                        OnPropertyChanged("Conversations");
                        FilteredConversations.Add(conversation);
                        OnPropertyChanged("FilteredConversations");

                        chat.Message = !string.IsNullOrEmpty(reader["ReceivedMsgs"].ToString()) ? reader["ReceivedMsgs"].ToString() : reader["SentMsgs"].ToString();
                    }
                }
            }
          
            MessageToReplyText = string.Empty;
            OnPropertyChanged("MessageToReplyText");
        }

        void SearchInConversation()
        {
           
            if ((string.IsNullOrEmpty(LastSearchConversationText) && string.IsNullOrEmpty(SearchConversationText)) || string.Equals(LastSearchConversationText, SearchConversationText))
                return;

         
            if (string.IsNullOrEmpty(SearchConversationText) || Conversations == null || Conversations.Count <= 0)
            {
                FilteredConversations = new ObservableCollection<ChatConversation>(Conversations ?? Enumerable.Empty<ChatConversation>());
                OnPropertyChanged("FilteredConversations");

             
                LastSearchConversationText = SearchConversationText;

                return;
            }

           

            FilteredConversations = new ObservableCollection<ChatConversation>(
                Conversations.Where(chat => chat.ReceivedMessage.ToLower().Contains(SearchConversationText) || chat.SentMessage.ToLower().Contains(SearchConversationText)));
            OnPropertyChanged("FilteredConversations");

           
            LastSearchConversationText = SearchConversationText;
        }

        public void CancelReply()
        {
            IsThisAReplyMessage = false;
           
            MessageToReplyText = string.Empty;
            OnPropertyChanged("MessageToReplyText");
        }

        public void SendMessage()
        {
           
            if (!string.IsNullOrEmpty(MessageText))
            {
                var conversation = new ChatConversation()
                {
                    ReceivedMessage = MessageToReplyText,
                    SentMessage = MessageText,
                    MsgSentOn = DateTime.Now.ToString("MMM dd, hh:mm tt"),
                    MessageContainsReply = IsThisAReplyMessage
                };

               
                FilteredConversations.Add(conversation);
                Conversations.Add(conversation);

                UpdateChatAndMoveUp(Chats, conversation);
                UpdateChatAndMoveUp(PinnedChats, conversation);
                UpdateChatAndMoveUp(FilteredChats, conversation);
                UpdateChatAndMoveUp(FilteredPinnedChats, conversation);
                UpdateChatAndMoveUp(ArchivedChats, conversation);

              
                MessageText = string.Empty;
                IsThisAReplyMessage = false;
                MessageToReplyText = string.Empty;               

                
                OnPropertyChanged("FilteredConversations");
                OnPropertyChanged("Conversations");
                OnPropertyChanged("MessageText");
                OnPropertyChanged("IsThisAReplyMessage");
                OnPropertyChanged("MessageToReplyText");
            }
        }

       
        protected void UpdateChatAndMoveUp(ObservableCollection<ChatListData> chatList, ChatConversation conversation)
        {
           
            var chat = chatList.FirstOrDefault(x => x.ContactName == ContactName);

           
            if (chat != null)
            {
               
                chat.Message = MessageText;
                chat.LastMessageTime = conversation.MsgSentOn;

               
                chatList.Move(chatList.IndexOf(chat), 0);

              
                OnPropertyChanged("Chats");
                OnPropertyChanged("PinnedChats");
                OnPropertyChanged("FilteredChats");
                OnPropertyChanged("FilteredPinnedChats");
                OnPropertyChanged("ArchivedChats");
            }
        }
        #endregion

        #region Commands
        
        protected ICommand _openConversationSearchCommand;
        public ICommand OpenConversationSearchCommand
        {
            get
            {
                if (_openConversationSearchCommand == null)
                    _openConversationSearchCommand = new CommandViewModel(OpenConversationSearchBox);
                return _openConversationSearchCommand;
            }
            set
            {
                _openConversationSearchCommand = value;
            }
        }

       
        protected ICommand _clearConversationSearchCommand;
        public ICommand ClearConversationSearchCommand
        {
            get
            {
                if (_clearConversationSearchCommand == null)
                    _clearConversationSearchCommand = new CommandViewModel(ClearConversationSearchBox);
                return _clearConversationSearchCommand;
            }
            set
            {
                _clearConversationSearchCommand = value;
            }
        }

       
        protected ICommand _closeConversationSearchCommand;
        public ICommand CloseConversationSearchCommand
        {
            get
            {
                if (_closeConversationSearchCommand == null)
                    _closeConversationSearchCommand = new CommandViewModel(CloseConversationSearchBox);
                return _closeConversationSearchCommand;
            }
            set
            {
                _closeConversationSearchCommand = value;
            }
        }

        protected ICommand _searchConversationCommand;
        public ICommand SearchConversationCommand
        {
            get
            {
                if (_searchConversationCommand == null)
                    _searchConversationCommand = new CommandViewModel(SearchInConversation);
                return _searchConversationCommand;
            }
            set
            {
                _searchConversationCommand = value;
            }
        }

        protected ICommand _replyCommand;
        public ICommand ReplyCommand => _replyCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatConversation v)
            {
              
                if (v.IsMessageReceived)
                    MessageToReplyText = v.ReceivedMessage;
                
                else
                    MessageToReplyText = v.SentMessage;

              
                OnPropertyChanged("MessageToReplyText");

                //Set focus on Message Box when user clicks reply button
                FocusMessageBox = true;
                OnPropertyChanged("FocusMessageBox");

                //Flag this message as reply message
                IsThisAReplyMessage = true;
                OnPropertyChanged("IsThisAReplyMessage");
            }
        });

        protected ICommand _cancelReplyCommand;
        public ICommand CancelReplyCommand
        {
            get
            {
                if (_cancelReplyCommand == null)
                    _cancelReplyCommand = new CommandViewModel(CancelReply);
                return _cancelReplyCommand;
            }
            set
            {
                _cancelReplyCommand = value;
            }
        }

        protected ICommand _sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                if (_sendMessageCommand == null)
                    _sendMessageCommand = new CommandViewModel(SendMessage);
                return _sendMessageCommand;
            }
            set
            {
                _sendMessageCommand = value;
            }
        }
        #endregion
        #endregion

        #region ContactInfo
        #region Properties
        protected bool _IsContactInfoOpen;
        public bool IsContactInfoOpen
        {
            get => _IsContactInfoOpen;
            set
            {
                _IsContactInfoOpen = value;
                OnPropertyChanged("IsContactInfoOpen");
            }
        }
        #endregion

        #region Logics
        public void OpenContactInfo() => IsContactInfoOpen = true;
        public void CloseContactInfo() => IsContactInfoOpen = false;
        #endregion

        #region Commands
       
        protected ICommand _openContactInfoCommand;
        public ICommand OpenContactinfoCommand
        {
            get
            {
                if (_openContactInfoCommand == null)
                    _openContactInfoCommand = new CommandViewModel(OpenContactInfo);
                return _openContactInfoCommand;
            }
            set
            {
                _openContactInfoCommand = value;
            }
        }

       
        protected ICommand _closeontactInfoCommand;
        public ICommand CloseContactinfoCommand
        {
            get
            {
                if (_closeontactInfoCommand == null)
                    _closeontactInfoCommand = new CommandViewModel(CloseContactInfo);
                return _closeontactInfoCommand;
            }
            set
            {
                _closeontactInfoCommand = value;
            }
        }
        #endregion
        #endregion

        SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Projects\ChatApp\Database\Database1.mdf;Integrated Security=True");
        public ViewModel()
        {
            LoadStatusThumbs();
            LoadChats();
            PinnedChats = new ObservableCollection<ChatListData>();
            ArchivedChats = new ObservableCollection<ChatListData>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

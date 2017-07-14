namespace FlashbackUwp.ViewModels
{
    public class ComposePrivateMessageViewModel : FlashbackViewModelBase
    {
        private string _subject;

        public string Subject
        {
            get { return _subject; }
            set { Set(ref _subject, value); }
        }

        private string _to;

        public string To
        {
            get { return _to; }
            set { Set(ref _to, value); }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        private bool _isQuote;

        public bool IsQuote
        {
            get { return _isQuote; }
            set { Set(ref _isQuote, value); }
        }

        private string _id;

        public string Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        private string _postToken;

        public string PostToken
        {
            get { return _postToken; }
            set { Set(ref _postToken, value); }
        }

        public ComposePrivateMessageViewModel()
        {
            
        }
    }
}

﻿using NTMiner.Language;
using System.Linq;
using System.Windows;

namespace NTMiner.Vms {
    public class LoginWindowViewModel : ViewModelBase {
        private string _hostAndPort;
        private string _loginName;
        private string _message;
        private Visibility _messageVisible = Visibility.Collapsed;
        private ILang _selectedLanguage;

        public LoginWindowViewModel() {
            this._selectedLanguage = LangSet.Instance.First();
            this._hostAndPort = $"{Server.MinerServerHost}:{Server.MinerServerPort.ToString()}";
            this._loginName = "admin";
        }

        public LangSet LangSet {
            get { return LangSet.Instance; }
        }

        public ILang SelectedLanguage {
            get => _selectedLanguage;
            set {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

        public string HostAndPort {
            get => _hostAndPort;
            set {
                _hostAndPort = value;
                OnPropertyChanged(nameof(HostAndPort));
            }
        }

        public string LoginName {
            get => _loginName;
            set {
                _loginName = value;
                OnPropertyChanged(nameof(LoginName));
            }
        }
        public string Message {
            get => _message;
            set {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public Visibility MessageVisible {
            get => _messageVisible;
            set {
                _messageVisible = value;
                OnPropertyChanged(nameof(MessageVisible));
            }
        }
    }
}

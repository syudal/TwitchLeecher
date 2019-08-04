using System;
using System.IO;
using TwitchLeecher.Core.Enums;
using TwitchLeecher.Shared.Helpers;
using TwitchLeecher.Shared.IO;
using TwitchLeecher.Shared.Notification;

namespace TwitchLeecher.Core.Models
{
    public class Preferences : BindableBase
    {
        #region Fields

        private Version _version;

        private bool _appCheckForUpdates;

        private bool _appShowDonationButton;

        private RangeObservableCollection<string> _searchFavouriteChannels;

        private string _searchChannelName;

        private VideoType _searchVideoType;

        private LoadLimitType _searchLoadLimitType;

        private int _searchLoadLastDays;

        private int _searchLoadLastVods;

        private bool _searchOnStartup;

        private string _downloadTempFolder;

        private string _downloadFolder;

        private string _downloadFileName;

        private bool _downloadSubfoldersForFav;

        private bool _downloadRemoveCompleted;

        private bool _downloadDisableConversion;

        private bool _miscUseExternalPlayer;

        private string _miscExternalPlayer;

        #endregion Fields

        #region Properties

        public Version Version
        {
            get
            {
                return _version;
            }
            set
            {
                SetProperty(ref _version, value);
            }
        }

        public bool AppCheckForUpdates
        {
            get
            {
                return _appCheckForUpdates;
            }
            set
            {
                SetProperty(ref _appCheckForUpdates, value);
            }
        }

        public bool AppShowDonationButton
        {
            get
            {
                return _appShowDonationButton;
            }
            set
            {
                SetProperty(ref _appShowDonationButton, value);
            }
        }

        public bool MiscUseExternalPlayer
        {
            get
            {
                return _miscUseExternalPlayer;
            }
            set
            {
                SetProperty(ref _miscUseExternalPlayer, value);
            }
        }

        public string MiscExternalPlayer
        {
            get
            {
                return _miscExternalPlayer;
            }

            set
            {
                SetProperty(ref _miscExternalPlayer, value);
            }
        }

        public RangeObservableCollection<string> SearchFavouriteChannels
        {
            get
            {
                if (_searchFavouriteChannels == null)
                {
                    _searchFavouriteChannels = new RangeObservableCollection<string>();
                }

                return _searchFavouriteChannels;
            }
        }

        public string SearchChannelName
        {
            get
            {
                return _searchChannelName;
            }
            set
            {
                SetProperty(ref _searchChannelName, value);
            }
        }

        public VideoType SearchVideoType
        {
            get
            {
                return _searchVideoType;
            }
            set
            {
                SetProperty(ref _searchVideoType, value);
            }
        }

        public LoadLimitType SearchLoadLimitType
        {
            get
            {
                return _searchLoadLimitType;
            }
            set
            {
                SetProperty(ref _searchLoadLimitType, value);
            }
        }

        public int SearchLoadLastDays
        {
            get
            {
                return _searchLoadLastDays;
            }
            set
            {
                SetProperty(ref _searchLoadLastDays, value);
            }
        }

        public int SearchLoadLastVods
        {
            get
            {
                return _searchLoadLastVods;
            }
            set
            {
                SetProperty(ref _searchLoadLastVods, value);
            }
        }

        public bool SearchOnStartup
        {
            get
            {
                return _searchOnStartup;
            }
            set
            {
                SetProperty(ref _searchOnStartup, value);
            }
        }

        public string DownloadTempFolder
        {
            get
            {
                return _downloadTempFolder;
            }
            set
            {
                SetProperty(ref _downloadTempFolder, value);
            }
        }

        public string DownloadFolder
        {
            get
            {
                return _downloadFolder;
            }
            set
            {
                SetProperty(ref _downloadFolder, value);
            }
        }

        public string DownloadFileName
        {
            get
            {
                return _downloadFileName;
            }
            set
            {
                SetProperty(ref _downloadFileName, value);
            }
        }

        public bool DownloadSubfoldersForFav
        {
            get
            {
                return _downloadSubfoldersForFav;
            }
            set
            {
                SetProperty(ref _downloadSubfoldersForFav, value);
            }
        }

        public bool DownloadRemoveCompleted
        {
            get
            {
                return _downloadRemoveCompleted;
            }
            set
            {
                SetProperty(ref _downloadRemoveCompleted, value);
            }
        }

        public bool DownloadDisableConversion
        {
            get
            {
                return _downloadDisableConversion;
            }
            set
            {
                SetProperty(ref _downloadDisableConversion, value);
            }
        }

        #endregion Properties

        #region Methods

        public override void Validate(string propertyName = null)
        {
            base.Validate(propertyName);

            string currentProperty = nameof(MiscExternalPlayer);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (MiscUseExternalPlayer)
                {
                    if (string.IsNullOrWhiteSpace(_miscExternalPlayer))
                    {
                        AddError(currentProperty, "외부 플레이어를 지정해주세요!");
                    }
                    else if (!_miscExternalPlayer.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        AddError(currentProperty, "파일 이름은 '.exe'로 끝나야합니다!");
                    }
                    else if (!File.Exists(_miscExternalPlayer))
                    {
                        AddError(currentProperty, "지정된 파일이 없습니다!");
                    }
                }
            }

            currentProperty = nameof(SearchChannelName);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchOnStartup && string.IsNullOrWhiteSpace(_searchChannelName))
                {
                    AddError(currentProperty, "'시작 시 검색'이 활성화된 경우 기본 채널 이름을 지정해야 합니다!");
                }
            }

            currentProperty = nameof(SearchLoadLastDays);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchLoadLimitType == LoadLimitType.Timespan && (_searchLoadLastDays < 1 || _searchLoadLastDays > 999))
                {
                    AddError(currentProperty, "값은 1에서 999 사이여야 합니다!");
                }
            }

            currentProperty = nameof(SearchLoadLastVods);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchLoadLimitType == LoadLimitType.LastVods && (_searchLoadLastVods < 1 || _searchLoadLastVods > 999))
                {
                    AddError(currentProperty, "값은 1에서 999 사이여야 합니다!");
                }
            }

            currentProperty = nameof(DownloadTempFolder);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (string.IsNullOrWhiteSpace(DownloadTempFolder))
                {
                    AddError(currentProperty, "임시 다운로드 폴더를 지정하세요!");
                }
            }

            currentProperty = nameof(DownloadFolder);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (string.IsNullOrWhiteSpace(_downloadFolder))
                {
                    AddError(currentProperty, "기본 다운로드 폴더를 지정하세요!");
                }
            }

            currentProperty = nameof(DownloadFileName);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (string.IsNullOrWhiteSpace(_downloadFileName))
                {
                    AddError(currentProperty, "다운로드 파일 이름을 지정하세요!");
                }
                else if (_downloadFileName.Contains(".") || FileSystem.FilenameContainsInvalidChars(_downloadFileName))
                {
                    string invalidChars = new string(Path.GetInvalidFileNameChars());

                    AddError(currentProperty, $"파일 이름에 ({invalidChars}.)와 같은 불가능한 문자가 포함되어 있습니다!");
                }
            }
        }

        public Preferences Clone()
        {
            Preferences clone = new Preferences()
            {
                Version = Version,
                AppCheckForUpdates = AppCheckForUpdates,
                AppShowDonationButton = AppShowDonationButton,
                MiscUseExternalPlayer = MiscUseExternalPlayer,
                MiscExternalPlayer = MiscExternalPlayer,
                SearchChannelName = SearchChannelName,
                SearchVideoType = SearchVideoType,
                SearchLoadLimitType = SearchLoadLimitType,
                SearchLoadLastDays = SearchLoadLastDays,
                SearchLoadLastVods = SearchLoadLastVods,
                SearchOnStartup = SearchOnStartup,
                DownloadTempFolder = DownloadTempFolder,
                DownloadFolder = DownloadFolder,
                DownloadFileName = DownloadFileName,
                DownloadSubfoldersForFav = DownloadSubfoldersForFav,
                DownloadRemoveCompleted = DownloadRemoveCompleted,
                DownloadDisableConversion = DownloadDisableConversion
            };

            clone.SearchFavouriteChannels.AddRange(SearchFavouriteChannels);

            return clone;
        }

        #endregion Methods
    }
}
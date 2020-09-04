using System;
using TwitchLeecher.Core.Enums;
using TwitchLeecher.Shared.Notification;

namespace TwitchLeecher.Core.Models
{
    public class SearchParameters : BindableBase
    {
        #region Fields

        private SearchType _searchType;
        private VideoType _videoType;

        private string _channel;
        private string _urls;
        private string _ids;

        private LoadLimitType _loadLimitType;

        private DateTime? _loadFrom;
        private DateTime? _loadFromDefault;
        private DateTime? _loadTo;
        private DateTime? _loadToDefault;

        private int _loadLastVods;

        #endregion Fields

        #region Constructors

        public SearchParameters(SearchType searchType)
        {
            _searchType = searchType;
        }

        #endregion Constructors

        #region Properties

        public SearchType SearchType
        {
            get
            {
                return _searchType;
            }
            set
            {
                SetProperty(ref _searchType, value);
            }
        }

        public VideoType VideoType
        {
            get
            {
                return _videoType;
            }
            set
            {
                SetProperty(ref _videoType, value);
            }
        }

        public string Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                SetProperty(ref _channel, value);
            }
        }

        public string Urls
        {
            get
            {
                return _urls;
            }
            set
            {
                SetProperty(ref _urls, value);
            }
        }

        public string Ids
        {
            get
            {
                return _ids;
            }
            set
            {
                SetProperty(ref _ids, value);
            }
        }

        public LoadLimitType LoadLimitType
        {
            get
            {
                return _loadLimitType;
            }
            set
            {
                SetProperty(ref _loadLimitType, value);
            }
        }

        public DateTime? LoadFrom
        {
            get
            {
                return _loadFrom;
            }
            set
            {
                SetProperty(ref _loadFrom, value);
            }
        }

        public DateTime? LoadFromDefault
        {
            get
            {
                return _loadFromDefault;
            }
            set
            {
                SetProperty(ref _loadFromDefault, value);
            }
        }

        public DateTime? LoadTo
        {
            get
            {
                return _loadTo;
            }
            set
            {
                SetProperty(ref _loadTo, value);
            }
        }

        public DateTime? LoadToDefault
        {
            get
            {
                return _loadToDefault;
            }
            set
            {
                SetProperty(ref _loadToDefault, value);
            }
        }

        public int LoadLastVods
        {
            get
            {
                return _loadLastVods;
            }
            set
            {
                SetProperty(ref _loadLastVods, value);
            }
        }

        #endregion Properties

        #region Methods

        public override void Validate(string propertyName = null)
        {
            base.Validate(propertyName);

            string currentProperty = nameof(Channel);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchType == SearchType.Channel && string.IsNullOrWhiteSpace(_channel))
                {
                    AddError(currentProperty, "채널 이름을 입력해주세요!");
                }
            }

            currentProperty = nameof(LoadFrom);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchType == SearchType.Channel && _loadLimitType == LoadLimitType.Timespan)
                {
                    if (!_loadFrom.HasValue)
                    {
                        AddError(currentProperty, "날짜를 입력해주세요!");
                    }
                    else
                    {
                        DateTime minimum = new DateTime(2010, 01, 01);

                        if (_loadFrom.Value.Date < minimum.Date)
                        {
                            AddError(currentProperty, "날짜는 '" + minimum.ToShortDateString() + "'보다 미래여야합니다!");
                        }

                        if (_loadFrom.Value.Date > DateTime.Now.Date)
                        {
                            AddError(currentProperty, "오늘보다 미래의 날짜로 지정할 수 없습니다!");
                        }
                    }
                }
            }

            currentProperty = nameof(LoadTo);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchType == SearchType.Channel && _loadLimitType == LoadLimitType.Timespan)
                {
                    if (!_loadTo.HasValue)
                    {
                        AddError(currentProperty, "날짜를 입력해주세요!");
                    }
                    else
                    {
                        if (_loadTo.Value.Date > DateTime.Now.Date)
                        {
                            AddError(currentProperty, "오늘보다 미래의 날짜로 지정할 수 없습니다!");
                        }

                        if (_loadFrom.HasValue && _loadFrom.Value.Date > _loadTo.Value.Date)
                        {
                            AddError(currentProperty, "날짜는 '" + _loadFrom.Value.ToShortDateString() + "'보다 미래여야합니다!");
                        }
                    }
                }
            }

            currentProperty = nameof(LoadLastVods);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchType == SearchType.Channel && _loadLimitType == LoadLimitType.LastVods)
                {
                    if (_loadLastVods < 1 || _loadLastVods > 999)
                    {
                        AddError(currentProperty, "값은 1에서 999 사이여야 합니다!");
                    }
                }
            }

            currentProperty = nameof(Urls);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchType == SearchType.Urls)
                {
                    if (string.IsNullOrWhiteSpace(_urls))
                    {
                        AddError(currentProperty, "하나 이상의 트위치 비디오 URL을 입력해주세요.");
                    }
                    else
                    {
                        void AddUrlError()
                        {
                            AddError(currentProperty, "잘못된 URL이 있습니다.");
                        }

                        string[] urls = _urls.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        if (urls.Length > 0)
                        {
                            foreach (string url in urls)
                            {
                                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri validUrl))
                                {
                                    AddUrlError();
                                    break;
                                }

                                string[] segments = validUrl.Segments;

                                if (segments.Length < 2)
                                {
                                    AddUrlError();
                                    break;
                                }

                                bool validId = false;

                                for (int i = 0; i < segments.Length; i++)
                                {
                                    if (segments[i].Equals("video/", StringComparison.OrdinalIgnoreCase) || segments[i].Equals("videos/", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (segments.Length > (i + 1))
                                        {
                                            string idStr = segments[i + 1];

                                            if (!string.IsNullOrWhiteSpace(idStr))
                                            {
                                                idStr = idStr.Trim(new char[] { '/' });

                                                if (int.TryParse(idStr, out int idInt) && idInt > 0)
                                                {
                                                    validId = true;
                                                    break;
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }

                                if (!validId)
                                {
                                    AddUrlError();
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            currentProperty = nameof(Ids);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchType == SearchType.Ids)
                {
                    if (string.IsNullOrWhiteSpace(_ids))
                    {
                        AddError(currentProperty, "하나 이상의 Twitch 비디오 고유 번호를 입력해주세요!");
                    }
                    else
                    {
                        string[] ids = _ids.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        if (ids.Length > 0)
                        {
                            foreach (string id in ids)
                            {
                                if (!int.TryParse(id, out int idInt) || idInt <= 0)
                                {
                                    AddError(currentProperty, "잘못된 고유 번호가 있습니다.");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public SearchParameters Clone()
        {
            return new SearchParameters(_searchType)
            {
                VideoType = _videoType,
                Channel = _channel,
                Urls = _urls,
                Ids = _ids,
                LoadLimitType = _loadLimitType,
                LoadFrom = _loadFrom,
                LoadFromDefault = _loadFromDefault,
                LoadTo = _loadTo,
                LoadToDefault = _loadToDefault,
                LoadLastVods = _loadLastVods
            };
        }

        #endregion Methods
    }
}
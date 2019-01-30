using System;
using System.IO;
using TwitchLeecher.Shared.Extensions;
using TwitchLeecher.Shared.IO;
using TwitchLeecher.Shared.Notification;

namespace TwitchLeecher.Core.Models
{
    public class DownloadParameters : BindableBase
    {
        #region Fields

        private readonly TwitchVideo _video;
        private readonly VodAuthInfo _vodAuthInfo;

        private TwitchVideoQuality _quality;

        private string _folder;
        private string _filename;

        private bool _cropStart;
        private bool _cropEnd;

        private TimeSpan _cropStartTime;
        private TimeSpan _cropEndTime;

        #endregion Fields

        #region Constructors

        public DownloadParameters(TwitchVideo video, VodAuthInfo vodAuthInfo, TwitchVideoQuality quality, string folder, string filename)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentNullException(nameof(folder));
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            _video = video ?? throw new ArgumentNullException(nameof(video));
            _quality = quality ?? throw new ArgumentNullException(nameof(quality));
            _vodAuthInfo = vodAuthInfo ?? throw new ArgumentNullException(nameof(vodAuthInfo));

            _folder = folder;
            _filename = filename;

            _cropEndTime = video.Length;
        }

        #endregion Constructors

        #region Properties

        public TwitchVideo Video
        {
            get
            {
                return _video;
            }
        }

        public TwitchVideoQuality Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                SetProperty(ref _quality, value, nameof(Quality));
            }
        }

        public VodAuthInfo VodAuthInfo
        {
            get
            {
                return _vodAuthInfo;
            }
        }

        public string Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                SetProperty(ref _folder, value, nameof(Folder));
                FirePropertyChanged(nameof(FullPath));
            }
        }

        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                SetProperty(ref _filename, value, nameof(Filename));
                FirePropertyChanged(nameof(FullPath));
            }
        }

        public string FullPath
        {
            get
            {
                return Path.Combine(_folder, _filename);
            }
        }

        public bool CropStart
        {
            get
            {
                return _cropStart;
            }
            set
            {
                SetProperty(ref _cropStart, value, nameof(CropStart));
                FirePropertyChanged(nameof(CroppedLength));
            }
        }

        public TimeSpan CropStartTime
        {
            get
            {
                return _cropStartTime;
            }
            set
            {
                SetProperty(ref _cropStartTime, value, nameof(CropStartTime));
                FirePropertyChanged(nameof(CroppedLength));
            }
        }

        public bool CropEnd
        {
            get
            {
                return _cropEnd;
            }
            set
            {
                SetProperty(ref _cropEnd, value, nameof(CropEnd));
                FirePropertyChanged(nameof(CroppedLength));
            }
        }

        public TimeSpan CropEndTime
        {
            get
            {
                return _cropEndTime;
            }
            set
            {
                SetProperty(ref _cropEndTime, value, nameof(CropEndTime));
                FirePropertyChanged(nameof(CroppedLength));
            }
        }

        public TimeSpan CroppedLength
        {
            get
            {
                if (!_cropStart && !_cropEnd)
                {
                    return _video.Length;
                }
                else if (!_cropStart && _cropEnd)
                {
                    return _cropEndTime;
                }
                else if (_cropStart && !_cropEnd)
                {
                    return _video.Length - _cropStartTime;
                }
                else
                {
                    return _cropEndTime - _cropStartTime;
                }
            }
        }

        public string CroppedLengthStr
        {
            get
            {
                return CroppedLength.ToDaylessString();
            }
        }

        #endregion Properties

        #region Methods

        public override void Validate(string propertyName = null)
        {
            base.Validate(propertyName);

            string currentProperty = nameof(Quality);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_quality == null)
                {
                    AddError(currentProperty, "영상 품질을 선택해주세요!");
                }
            }

            currentProperty = nameof(Folder);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (string.IsNullOrWhiteSpace(_folder))
                {
                    AddError(currentProperty, "폴더를 지정해주세요!");
                }
            }

            currentProperty = nameof(Filename);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (string.IsNullOrWhiteSpace(_filename))
                {
                    AddError(currentProperty, "파일 이름을 지정해주세요!");
                }
                else if (!_filename.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                {
                    AddError(currentProperty, "파일의 이름은 '.mp4'로 끝나야 합니다!");
                }
                else if (FileSystem.FilenameContainsInvalidChars(_filename))
                {
                    AddError(currentProperty, "파일 이름에 잘못된 문자가 포함되어 있습니다!");
                }
            }

            currentProperty = nameof(CropStartTime);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_cropStart)
                {
                    TimeSpan videoLength = _video.Length;

                    if (_cropStartTime < TimeSpan.Zero || _cropStartTime > videoLength)
                    {
                        AddError(currentProperty, "'" + TimeSpan.Zero.ToString() + "' 와 '" + videoLength.ToDaylessString() + "'의 사이 값을 입력해주세요!");
                    }
                    else if (CroppedLength.TotalSeconds < 5)
                    {
                        AddError(currentProperty, "잘라낸 비디오는 5초 이상이어야 합니다.");
                    }
                }
            }

            currentProperty = nameof(CropEndTime);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_cropEnd)
                {
                    TimeSpan videoLength = _video.Length;

                    if (_cropEndTime < TimeSpan.Zero || _cropEndTime > videoLength)
                    {
                        AddError(currentProperty, "'" + TimeSpan.Zero.ToString() + "' 와 '" + videoLength.ToDaylessString() + "'의 사이 값을 입력해주세요!");
                    }
                    else if (_cropStart && (_cropEndTime <= _cropStartTime))
                    {
                        AddError(currentProperty, "끝나는 시간이 시작 시간보다는 미래여야 합니다!");
                    }
                    else if (CroppedLength.TotalSeconds < 5)
                    {
                        AddError(currentProperty, "잘라낸 비디오는 5초 이상이어야 합니다.");
                    }
                }
            }
        }

        #endregion Methods
    }
}
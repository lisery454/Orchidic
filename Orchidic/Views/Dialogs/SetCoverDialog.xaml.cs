using Orchidic.Utils;
using TagLib;
using TagLib.Id3v2;
using File = System.IO.File;

namespace Orchidic.Views.Dialogs;

public partial class SetCoverDialog : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public partial class SetCoverDialog
{
    private string _audioPath = string.Empty;

    public string AudioPath
    {
        get => _audioPath;
        private set
        {
            if (_audioPath == value) return;
            _audioPath = value;
            OnPropertyChanged();
        }
    }

    private string _imagePath = string.Empty;

    public string ImagePath
    {
        get => _imagePath;
        private set
        {
            if (_imagePath == value) return;
            _imagePath = value;
            OnPropertyChanged();
        }
    }

    public SetCoverDialog()
    {
        WindowCornerRestorer.ApplyRoundCorner(this);
        InitializeComponent();
        DataContext = this;
    }

    private void SelectAudio_OnClick(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "请选择一个音频文件",
            Filter = "音频文件|*.mp3;*.wav;*.flac;*.aac", // 可自定义音频格式
            Multiselect = false // 只允许选一个
        };

        var result = dlg.ShowDialog();

        if (result == true)
        {
            var filePath = dlg.FileName;
            AudioPath = filePath;
        }
    }

    private void SelectImage_OnClick(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "请选择一张图片",
            Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif", // 支持的图片格式
            Multiselect = false // 只允许选择一个
        };

        var result = dlg.ShowDialog();

        if (result == true)
        {
            var filePath = dlg.FileName;
            ImagePath = filePath;
        }
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        if (File.Exists(ImagePath) && File.Exists(AudioPath))
        {
            SetCover();
        }

        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        
        Close();
    }

    private static byte[] ImageToByteArray(System.Drawing.Image imageIn)
    {
        using var ms = new MemoryStream();
        imageIn.Save(ms, imageIn.RawFormat);
        return ms.ToArray();
    }

    private void SetCover()
    {
        try
        {
            using var music = TagLib.File.Create(AudioPath);
            using var image = System.Drawing.Image.FromFile(ImagePath);
            var byteArray = ImageToByteArray(image);
            var byteArrayLength = byteArray.Length;

            var pic = new AttachmentFrame
            {
                Data = new ByteVector(byteArray, byteArrayLength),
                Description = "",
                EncryptionId = -1,
                Filename = null,
                Flags = FrameFlags.None,
                GroupId = -1,
                TextEncoding = StringType.UTF16,
                Type = PictureType.Other
            };


            music.Tag.Pictures = [pic];
            music.Save();
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine($"{e.FileName} file not find");
        }
        catch (CorruptFileException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
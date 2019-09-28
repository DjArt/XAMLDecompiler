using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Build.ILTasks.ReducerEngine;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Search;
using System.Threading.Tasks;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace XAMLDecompiler
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        XBFConventer Conventer;

        public MainPage()
        {
            this.InitializeComponent();
            Conventer = new XBFConventer();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.ViewMode = PickerViewMode.List;
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder pickedFolder = await folderPicker.PickSingleFolderAsync();
            StorageFolder outFolder = await folderPicker.PickSingleFolderAsync();
            List<StorageFile> xbfList = (await pickedFolder.GetFilesAsync(CommonFileQuery.OrderByName)).Where(x => x.FileType == ".xbf").ToList();
            foreach (StorageFile picked in xbfList)
            {
                StorageFolder folder = await RecreatePath(outFolder, picked.Path.Substring(pickedFolder.Path.Length).Split('\\').Skip(1).SkipLast(1));
                var file = await folder.CreateFileAsync(picked.Name + ".xaml", CreationCollisionOption.ReplaceExisting);
                await Conventer.Convert(picked, file);
            }
        }

        private async Task<StorageFolder> RecreatePath(StorageFolder folder, IEnumerable<string> parts)
        {
            folder = await folder.CreateFolderAsync(parts.First(), CreationCollisionOption.OpenIfExists);
            parts = parts.Skip(1);
            return parts.Count() > 0 ? await RecreatePath(folder, parts) : folder;
        }
    }
}

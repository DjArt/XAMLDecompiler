using Microsoft.Build.ILTasks.ReducerEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml;
using Windows.Storage;
using Windows.Storage.Streams;

namespace XAMLDecompiler
{
    public sealed class XBFConventer
    {
        private ConstructorInfo ReaderConstructor;
        private MethodInfo ReadXBFFileMethod;

        public XBFConventer()
        {
            Type engineType = typeof(AnalysisEngine);
            Type readerType = engineType.Assembly.GetType("Xbf2Xaml.XBFReader");
            ReaderConstructor = readerType.GetConstructor(new[] { typeof(string), engineType });
            ReadXBFFileMethod = readerType.GetMethod("ReadXBFFile");
        }

        public async Task Convert(StorageFile input, StorageFile output)
        {
            StorageFile copy = await input.CopyAsync(ApplicationData.Current.TemporaryFolder);
            object reader = ReaderConstructor.Invoke(new object[] { copy.Path, null });
            object xaml = ReadXBFFileMethod.Invoke(reader, new object[0]);
            await copy.DeleteAsync();
            await FileIO.WriteTextAsync(output, xaml.ToString());
        }
    }
}

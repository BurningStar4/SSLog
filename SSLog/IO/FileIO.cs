using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSLog.LogHandling;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
/**
 * This class will handle reading and writing of files.
 * 
 * https://code.google.com/p/protobuf-csharp-port/ - eventual serialization method once PC functionality is ready.
 * */

namespace SSLog.IO
{
    sealed class FileIO
    {
        private static String lastFile ="";

        //TODO: Write to file
        public static Tuple<Boolean, String> SaveLog(ObservableCollection<LogEntryDataModel> aLog, Boolean SaveAs)
        {
            Tuple<Boolean, String> statusNameTuple;

            string filename = "";
            // Call file dialog.
            if (SaveAs)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "LogName";
                dlg.DefaultExt = "sslog";
                dlg.Filter = "SSLog Files (*.sslog)|*.sslog";
                // Open save file dialog

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    filename = dlg.FileName;
                }
                else
                {
                    // Return error message
                    statusNameTuple = new Tuple<bool, string>(false, "FILENOTSAVED");
                    return statusNameTuple;
                }
            }
            else
            {
                filename = lastFile;
            }

            // Serialize
            IFormatter formatter = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();

            ss.AddSurrogate(typeof(ObservableCollection<LogEntryDataModel>), new StreamingContext(StreamingContextStates.All), new ObservableCollectionSerializationSurrogate<LogEntryDataModel>());
            formatter.SurrogateSelector = ss;

            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, aLog);
            stream.Close();
            statusNameTuple = new Tuple<bool, string>(true, filename);
            lastFile = filename;

            return statusNameTuple;
        }

        //TODO: ERROR HANDLING OF SHIT FILES
        public static Tuple<ObservableCollection<LogEntryDataModel>, String> LoadLog(String aFilepath)
        {
            String filename;

            if (String.IsNullOrWhiteSpace(aFilepath))
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "SSLog Files (*.sslog)|*.sslog";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == false)
                {
                    return new Tuple<ObservableCollection<LogEntryDataModel>, String>(null, "CANCELLED");
                }
                filename = dlg.FileName;
            }
            else
            {
                filename = aFilepath;
            }

            if (!String.IsNullOrWhiteSpace(filename))
            {
                IFormatter formatter = new BinaryFormatter();
                SurrogateSelector ss = new SurrogateSelector();
                ss.AddSurrogate(typeof(ObservableCollection<LogEntryDataModel>), new StreamingContext(StreamingContextStates.All), new ObservableCollectionSerializationSurrogate<LogEntryDataModel>());
                formatter.SurrogateSelector = ss;

                Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                try
                {
                   ObservableCollection<LogEntryDataModel> openedCollection = formatter.Deserialize(stream) as ObservableCollection<LogEntryDataModel>;
                   stream.Close();
                   lastFile = filename;
                   return new Tuple<ObservableCollection<LogEntryDataModel>, String>(openedCollection, filename); //openedCollection;
                }
                catch (ArgumentException e)
                {
                    // Bad/Corrupted/Incompatible sslog file.
                    return new Tuple<ObservableCollection<LogEntryDataModel>, String>(null, "BADFILE"); 
                }
            }
            else
            {
                return new Tuple<ObservableCollection<LogEntryDataModel>, String>(null, "BADFILE"); 
            }
        }
    }
}
